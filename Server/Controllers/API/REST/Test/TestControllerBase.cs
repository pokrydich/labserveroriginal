namespace LabServer.Server.Controllers;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;

using LabServer.Server.Hubs;
using LabServer.Server.Models;
using LabServer.Server.Models.Uni;
using LabServer.Server.Service;
using LabServer.Shared.Models.Uni;

public class TestsController : BaseRestController<TestModel, TestData>
{
    public TestsController(IDBStorage<TestModel> storage, UserManager<UserModel> userManager, IHubContext<DataHub> dataHub) 
        : base(storage, userManager, dataHub)
    {
    }
}