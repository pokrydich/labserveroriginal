namespace LabServer.Shared.Models.TestAPI;

public class GetTestResultRequestModel
{
    // (SourceProjectId, CommitHash) should serve as the tracker on TestServer side
    public System.Int64 SourceProjectId { get; set; }
    public System.String CommitHash { get; set; } = System.String.Empty;
}