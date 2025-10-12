namespace LabServer.Server.Models;

using LabServer.Shared.Models;

public interface IDataModel<DM> where DM : DataModel
{
    DM ToData(DataConversionOption conversionOption = DataConversionOption.Default);
    void Update(DM data);
}