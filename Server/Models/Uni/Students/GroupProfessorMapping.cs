namespace LabServer.Server.Models.Uni;

using Microsoft.EntityFrameworkCore.Infrastructure;

using IndexAttribute = Microsoft.EntityFrameworkCore.IndexAttribute;

[Index(nameof(Id), IsUnique = true)]
[Index(nameof(EntityId), nameof(UserId), IsUnique = true)]
public class GroupProfessorMapping : AccessMapping<GroupModel>
{
    public GroupProfessorMapping() { }
    public GroupProfessorMapping(ILazyLoader lazyLoader) : base(lazyLoader) { }
}