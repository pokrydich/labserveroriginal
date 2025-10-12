namespace LabServer.Server.Models;

using LabServer.Server.Service;

public interface IGitIntegrated
{
    IGitLab GitLab
    {
        get;
    }
    void InjectGitLab(IGitLab gitLab);
}