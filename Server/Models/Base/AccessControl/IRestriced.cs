namespace LabServer.Server.Models;

public interface IRestricted
{
    IEnumerable<System.Int64> AllowedIDs { get; }
}