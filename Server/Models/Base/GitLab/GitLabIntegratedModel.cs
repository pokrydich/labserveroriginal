namespace LabServer.Server.Models;

using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore.Infrastructure;

using LabServer.Server.Service;
using LabServer.Server.Data;

public class GitLabIntegratedModel : LazyLoadedModel, IGitIntegrated
{
    protected GitLabIntegratedModel() { }
    protected IGitLab? _gitLab;
    protected GitLabIntegratedModel(ILazyLoader lazyLoader, LabsContext labsContext) : base(lazyLoader)
        => _gitLab = labsContext.GitLabService;
    [NotMapped]
    [JsonIgnore]
    public IGitLab GitLab => _gitLab ?? throw new NotImplementedException("gitlab service was not injected here. Why?");

    public void InjectGitLab(IGitLab gitLab) => _gitLab = gitLab;
}