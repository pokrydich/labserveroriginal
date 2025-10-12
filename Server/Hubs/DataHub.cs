namespace LabServer.Server.Hubs;

using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;

using LabServer.Server.Models;
using LabServer.Shared.Models;

[Authorize]
public class DataHub : Hub
{
}

public static class DataHubExtension
{
    public static async Task SendUpdate<DM, M>(this IHubContext<DataHub> clientsHub, M model, 
                                DataConversionOption conversionOption = DataConversionOption.Default) where DM : DataModel
                                                                                                      where M : IDataModel<DM>
    {
        var toSend = model.ToData(conversionOption);
        if (model is IRestricted restrictedData)
        {
            await clientsHub.Clients.Users(restrictedData.AllowedIDs.Select(id => id.ToString())).SendAsync(typeof(DM).Name, toSend);
        }
        else
        {
            await clientsHub.Clients.All.SendAsync(typeof(DM).Name, toSend);
        }
    }

    public static async Task SendUpdateRaw<DM>(this IHubContext<DataHub> clientsHub, DM data) where DM : DataModel
    {
        await clientsHub.Clients.All.SendAsync(typeof(DM).Name, data);
    }
}