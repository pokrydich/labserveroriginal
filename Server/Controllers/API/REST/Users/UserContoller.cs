namespace LabServer.Server.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using LabServer.Shared.Models;
using LabServer.Server.Hubs;
using LabServer.Server.Data;
using LabServer.Server.Models;

[Route("api/rest/[controller]")]
[ApiController]
public class UsersController : Controller
{
    private readonly LabsContext _context;
    private readonly UserManager<UserModel> _userManager;
    private readonly RoleManager<RoleModel> _roleManager;
    private readonly IHubContext<DataHub> _dataHub;


    public UsersController(LabsContext context, IHubContext<DataHub> dataHub, 
                UserManager<UserModel> userManager, RoleManager<RoleModel> roleManager)
    {
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
        _dataHub = dataHub;
    }

    [HttpGet("authstatus")]
    [Authorize]
    public async Task<ApiRequestResult<UserData>> GetAuthStatus()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return ApiRequestResult.Failure<UserData>("userApiErrorUserNotFound");
        }

        var roles = await _userManager.GetRolesAsync(user);

        return ApiRequestResult.Success(new UserData
        {
            Id = user.Id,
            Email = user.Email,
            Roles = roles
        });
    }

    [HttpGet]
    [Authorize(Roles = "Administrator")]
    public async Task<ApiRequestResult<IEnumerable<UserData>>> GetUsers()
    {
        var users = await _context.Users.ToListAsync();
        List<UserData> result = new List<UserData>();
        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            result.Add(new UserData
            {
                Id = user.Id,
                Email = user.Email,
                Roles = roles
            });
        }
        return ApiRequestResult.Success<IEnumerable<UserData>>(result);
    }

    [HttpGet("{userId}")]
    [Authorize(Roles = "Administrator")]
    public async Task<ApiRequestResult<UserData>> GetOneUser(System.Int64 userId)
    {
        var user = await _context.Users.SingleOrDefaultAsync(u => u.Id == userId);
        if (user == null)
            return ApiRequestResult.Failure<UserData>("not found");
        var roles = await _userManager.GetRolesAsync(user);
        return ApiRequestResult.Success<UserData>(new UserData
        {
            Id = user.Id,
            Email = user.Email,
            Roles = roles
        });
    }

    [HttpDelete("{userId}/roles")]
    [Authorize(Roles = "Administrator")]
    public async Task<ApiRequestResult> RemoveRole(System.Int64 userId, [FromQuery] UserRoles role)
    {
        var user = await _context.Users.SingleOrDefaultAsync(u => u.Id == userId);
        if (user == null)
        {
            return ApiRequestResult.Failure("User not found");
        }
        var isInRole = await _userManager.IsInRoleAsync(user, role.ToString());
        if (!isInRole)
        {
            return ApiRequestResult.Failure("User doesn't have specified role");
        }
        await _userManager.RemoveFromRoleAsync(user, role.ToString());

        await _dataHub.SendUpdateRaw(new UserData
        {
            Id = user.Id,
            Email = user.Email,
            Roles = await _userManager.GetRolesAsync(user)
        });
        return ApiRequestResult.Success();
    }

    [HttpGet("{userId}/roles")]
    [Authorize(Roles = "Administrator")]
    public async Task<ApiRequestResult> AddRole(System.Int64 userId, [FromQuery] UserRoles role)
    {
        var user = await _context.Users.SingleOrDefaultAsync(u => u.Id == userId);
        if (user == null)
        {
            return ApiRequestResult.Failure("User not found");
        }
        if (await _userManager.IsInRoleAsync(user, role.ToString()))
        {
            return ApiRequestResult.Failure("Role already assigned");
        }
        var targetRole = await _roleManager.FindByNameAsync(role.ToString());
        if (targetRole == null)
        {
            await _roleManager.CreateAsync(new RoleModel
            {
                Name = role.ToString()
            });
        }
        await _userManager.AddToRoleAsync(user, role.ToString());

        await _dataHub.SendUpdateRaw(new UserData
        {
            Id = user.Id,
            Email = user.Email,
            Roles = await _userManager.GetRolesAsync(user)
        });
        return ApiRequestResult.Success();
    }
}