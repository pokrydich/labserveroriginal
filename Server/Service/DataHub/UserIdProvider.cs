namespace LabServer.Server.Service;

using LabServer.Server.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;

public class UserIdProvider : IUserIdProvider
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    public UserIdProvider(IServiceScopeFactory serviceScopeFactory) 
        => _serviceScopeFactory = serviceScopeFactory;

    public System.String? GetUserId(HubConnectionContext connection)
    {
        var scope = _serviceScopeFactory.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<UserModel>>();

        var user = userManager.GetUserAsync(connection.User).GetAwaiter().GetResult();
        return user != null ? user.Id.ToString() : "anon";
    }
}