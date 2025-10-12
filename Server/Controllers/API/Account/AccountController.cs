namespace LabServer.Server.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;

using LabServer.Server.Models;
using LabServer.Shared.Models;
using LabServer.Server.Hubs;

[Route("api/[controller]")]
[ApiController]
public class AccountsController : Controller
{
    private readonly UserManager<UserModel> _userManager;
    private readonly RoleManager<RoleModel> _roleManager;
    private readonly IHubContext<DataHub> _dataHub;
    public AccountsController(UserManager<UserModel> userManager, RoleManager<RoleModel> roleManager, IHubContext<DataHub> dataHub)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _dataHub = dataHub;
    }
    [HttpPost]
    public async Task<ApiRequestResult> Post([FromBody]RegisterModel model)
    {
        RoleModel adminRole = await _roleManager.FindByNameAsync(UserRoles.Administrator.ToString());
        if (adminRole == null)
        {
            var res = await _roleManager.CreateAsync(new RoleModel
            {
                Name = UserRoles.Administrator.ToString(),
            });
        }
        System.Boolean firstUser =  _userManager.Users.Count() == 0;
        var newUser = new UserModel
        { 
            UserName = model.Email,
            Email = model.Email,
        };
        var result = await _userManager.CreateAsync(newUser, model.Password);
        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(error => error.Description);
            return ApiRequestResult.Failure(System.String.Join('.', errors));
        }
        if (firstUser)
        {
            await _userManager.AddToRoleAsync(newUser, UserRoles.Administrator.ToString());
        }

        await _dataHub.SendUpdateRaw(new UserData
        {
            Id = newUser.Id,
            Email = newUser.Email,
            Roles = await _userManager.GetRolesAsync(newUser)
        });
        return ApiRequestResult.Success();
    }
}