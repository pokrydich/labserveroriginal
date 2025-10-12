namespace LabServer.Server.Controllers;

using System.Collections.Concurrent;
using System.Reflection;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

using Microsoft.EntityFrameworkCore;

using LabServer.Server.Models;
using LabServer.Server.Service;
using LabServer.Shared.Models;
using Microsoft.AspNetCore.SignalR;
using LabServer.Server.Hubs;

public enum RestEntityStatus
{
    Ready = 0,
    NotFound,
    Restricted
}

[Authorize(Roles = "Professor,Assistant")]
[Route("api/rest/[controller]")]
[ApiController]
public class BaseRestController<D, DM> : Controller where D : DBObjectModel, IDataModel<DM>
                                                where DM : DataModel
{
    private IDBStorage<D> _storage;
    protected IDBStorage<D> Storage => _storage;
    private readonly UserManager<UserModel> _userManager;
    private readonly IHubContext<DataHub> _dataHub;
    private static readonly ConcurrentDictionary<Type, ConstructorInfo?> _constructorInitMap = new ConcurrentDictionary<Type, ConstructorInfo?>();
    protected IHubContext<DataHub> DataHub => _dataHub;

    public BaseRestController(IDBStorage<D> storage, UserManager<UserModel> userManager, IHubContext<DataHub> dataHub)
    {
        _storage = storage;
        _userManager = userManager;
        _dataHub = dataHub;
    }

    protected async Task<IEnumerable<D>> _getAll()
    {
        if (typeof(IRestricted).IsAssignableFrom(typeof(D)))
        {
            // enforce access control
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return new List<D>();
            var allowedObjects = _storage.GetAll().Where(dbo => (dbo as IRestricted).AllowedIDs.Contains(user.Id));
            return allowedObjects;
        }
        return _storage.GetAll();
    }

    [HttpGet]
    public async Task<ApiRequestResult<IEnumerable<DM>>> Get([FromQuery] DataConversionOption include = DataConversionOption.Default)
    {
        var all = await _getAll();
        return ApiRequestResult.Success<IEnumerable<DM>>(all.Select(dbo => dbo.ToData(include)));
    }

    [HttpPost]
    public async Task<ApiRequestResult<DM>> Create([FromBody] DM dataModel)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            return ApiRequestResult.Failure<DM>("access denied");

        ConstructorInfo? createConstructor = null;
        if (!_constructorInitMap.TryGetValue(typeof(D), out createConstructor))
        {
            if (typeof(ICreatable<DM>).IsAssignableFrom(typeof(D)))
                createConstructor = typeof(D).GetConstructor(new Type[] { });
            if (!_constructorInitMap.TryAdd(typeof(D), createConstructor))
                throw new NotImplementedException("shouldn't be here");
        }
        if (createConstructor == null)
            return ApiRequestResult.Failure<DM>($"'{typeof(DM).Name}' is not creatable");

        var dbModel = createConstructor.Invoke(new System.Object[] {}) as D ?? throw new NotImplementedException("a check was made");
        (dbModel as ICreatable<DM> ?? throw new NotImplementedException("shouldn't be here a check was made")).Init(dataModel);
        await _storage.Add(dbModel);

        try
        {
            await _storage.ApplyChangesAsync();
            if (dbModel is IPersonal personalModel)
                personalModel.SetOwner(user);
            await _storage.ApplyChangesAsync();
        }
        catch (DbUpdateException)
        {
            return ApiRequestResult.Failure<DM>("error inserting a new object. constrains failed");
        }

        await _dataHub.SendUpdate<DM, D>(dbModel);

        return ApiRequestResult.Success<DM>(dbModel.ToData());
    }

    protected async Task<Tuple<RestEntityStatus, D?>> _getOneInternal(System.Int64 id)
    {
        var one = await _storage.GetById(id);
        if (one != null)
        {
            if (one is IRestricted restricted)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null || !restricted.AllowedIDs.Contains(user.Id))
                    return new Tuple<RestEntityStatus, D?>(RestEntityStatus.Restricted, one);
            }
            return new Tuple<RestEntityStatus, D?>(RestEntityStatus.Ready, one);
        }
        return new Tuple<RestEntityStatus, D?>(RestEntityStatus.NotFound, null);
    }

    [HttpGet("{id}")]
    public async Task<ApiRequestResult<DM>> GetOne(System.Int64 id, DataConversionOption include = DataConversionOption.Default)
    {
        var oneLookup = await _getOneInternal(id);
        return oneLookup.Item1 switch
        {
            RestEntityStatus.Ready => ApiRequestResult.Success<DM>(oneLookup.Item2.ToData(include)),
            RestEntityStatus.NotFound => ApiRequestResult.Failure<DM>($"{typeof(D).Name} not found"),
            RestEntityStatus.Restricted => ApiRequestResult.Failure<DM>("access denied"),
            _ => throw new NotImplementedException("invalid enum value")
        };
    }

    [HttpPut("{id}")]
    public async Task<ApiRequestResult<DM>> Update(System.Int64 id, [FromBody] DM data)
    {
        var oneLookup = await _getOneInternal(id);
        if (oneLookup.Item1 == RestEntityStatus.Ready && oneLookup.Item2 != null)
        {
            oneLookup.Item2.Update(data);
            try
            {
                await _storage.ApplyChangesAsync();
            }
            catch (DbUpdateException)
            {
                return ApiRequestResult.Failure<DM>("error updating data. constraint failed");
            }

            await _dataHub.SendUpdate<DM, D>(oneLookup.Item2);
            return ApiRequestResult.Success<DM>(oneLookup.Item2.ToData());
        }
        return _returnBadResult<DM>(oneLookup.Item1);
    }

    protected ApiRequestResult<T> _returnBadResult<T>(RestEntityStatus status) => status switch
    {
        RestEntityStatus.NotFound => ApiRequestResult.Failure<T>($"not found"),
        RestEntityStatus.Restricted => ApiRequestResult.Failure<T>("access denied"),
        _ => throw new NotImplementedException("invalid enum value")
    };

    protected ApiRequestResult _returnBadResult(RestEntityStatus status) => status switch
    {
        RestEntityStatus.NotFound => ApiRequestResult.Failure($"not found"),
        RestEntityStatus.Restricted => ApiRequestResult.Failure("access denied"),
        _ => throw new NotImplementedException("invalid enum value")
    };

}