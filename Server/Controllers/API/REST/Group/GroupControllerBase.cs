namespace LabServer.Server.Controllers;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

using LabServer.Server.Models;
using LabServer.Server.Models.Uni;
using LabServer.Server.Service;
using LabServer.Shared.Models.Uni;
using LabServer.Shared.Models;
using Microsoft.AspNetCore.SignalR;
using LabServer.Server.Hubs;

public partial class GroupsController : BaseRestController<GroupModel, GroupData>
{
    private IDBStorage<StudentModel> _studentsStore;
    private readonly IEmailSender _emailSender;

    public GroupsController(IDBStorage<GroupModel> storage, UserManager<UserModel> userManager, 
                        IHubContext<DataHub> dataHub,
                        IDBStorage<StudentModel> studentsStore,
                        IEmailSender emailSender) : base(storage, userManager, dataHub)
    {
        _studentsStore = studentsStore;
        _emailSender = emailSender;
    }

    [HttpGet("sync")]
    public async Task<ApiRequestResult> SyncAll()
    {
        var groups = await _getAll();

        HashSet<System.String> warrnings = new HashSet<string>();
        foreach (var group in groups)
        {
            if (group.GitLabGroupId == null)
            {
                var syncResult = await group.SyncWithGitLab();
                if (!syncResult.Ok)
                {
                    warrnings.Add($"Couldn't sync group '{group.Id}': {syncResult.Error.Message}");
                    continue;
                }

                await DataHub.SendUpdate<GroupData, GroupModel>(group);
            }
        }
        await Storage.ApplyChangesAsync();
        return ApiRequestResult.Success(warrnings);
    }
}