namespace LabServer.Server.Models;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Infrastructure;

using LabServer.Server.Service;
using LabServer.Server.Models.Uni;
using LabServer.Shared.Models;

/// <summary>
/// User AKA might be professor
/// </summary>
public class UserModel : IdentityUser<System.Int64>, IDataModel<UserData>
{
    public UserModel() { }
    private ILazyLoader? _lazyLoader;
    private IGitLab? _gitLab;
    public UserModel(ILazyLoader lazyLoader, IGitLab gitLab)
    {
        _lazyLoader = lazyLoader;
        _gitLab = gitLab;
    }
    public System.Int64? GitLabUserId { get; set; }
    private ICollection<GroupProfessorMapping>? _groupsMapping;
    public ICollection<GroupProfessorMapping> GroupsMapping
    {
        get => _lazyLoader.Load(this, ref _groupsMapping) ?? throw new NotImplementedException("error in lazy loading");
        set => _groupsMapping = value;
    }

    public UserData ToData(DataConversionOption conversionOption = DataConversionOption.Default) => new UserData
    {
        Email = Email
    };

    public void Update(UserData data)
    {
        throw new NotImplementedException();
    }
}