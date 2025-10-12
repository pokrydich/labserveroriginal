namespace LabServer.Server.Models;

using Microsoft.EntityFrameworkCore.Infrastructure;

public abstract class PersonalModel<AM, E> : LazyLoadedModel, IPersonal, IRestricted where E : DBObjectModel 
                                              where AM : AccessMapping<E>, new()
{
    public PersonalModel() { }
    protected PersonalModel(ILazyLoader lazyLoader) : base(lazyLoader) { }

    private ICollection<AM>? _accessMappings;
    public ICollection<AM> AccessMappings
    {
        get => Loader.Load(this, ref _accessMappings) ?? throw new NotImplementedException();
        set => _accessMappings = value;
    }

    public IEnumerable<System.Int64> AllowedIDs => AccessMappings.Select(am => am.UserId);

    public void SetOwner(UserModel user)
    {
        if (AccessMappings.SingleOrDefault(am => am.Owner) is AccessMapping<E> currentOwner)
        {
            if (currentOwner.UserId == user.Id)
                return;
            currentOwner.Owner = false;
        }
        if (AccessMappings.SingleOrDefault(pm => pm.UserId == user.Id) is AccessMapping<E> targetMapping)
        {
            targetMapping.Owner = true;
            return;
        }
        else
        {
            AccessMappings.Add(new AM
            {
                EntityId = Id,
                UserId = user.Id,
                Owner = true
            });
            return;
        }
    }
}