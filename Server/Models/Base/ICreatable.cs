namespace LabServer.Server.Models;

using LabServer.Shared.Models;

public interface ICreatable<DM> : IDataModel<DM> where DM : DataModel
{
    void Init(DM data);
}