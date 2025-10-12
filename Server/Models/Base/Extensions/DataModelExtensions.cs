using LabServer.Shared.Models;

namespace LabServer.Server.Models;

public static class DataModelExtensions
{
    public static IEnumerable<DM> ToData<D, DM>(this IEnumerable<D> datas, 
        DataConversionOption conversionOption = DataConversionOption.Default) where DM : DataModel 
                                                                            where D : IDataModel<DM>
    {
        return datas.Select(d => d.ToData(conversionOption));
    }
}