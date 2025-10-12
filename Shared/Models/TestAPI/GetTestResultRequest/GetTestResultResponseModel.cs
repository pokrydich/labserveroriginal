namespace LabServer.Shared.Models.TestAPI;

public class GetTestResultResponseModel
{
    public System.Boolean TestCompleted { get; set; }
    // those two are mapped to TestRunModel fiedls
    public System.Boolean? Success { get; set; }
    public System.String? Message { get; set; }

}