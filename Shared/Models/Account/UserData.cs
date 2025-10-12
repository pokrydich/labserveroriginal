namespace LabServer.Shared.Models;

public enum UserRoles
{
    Administrator,
    Professor,
    Assistant
}

public class UserData : DataModel
{
    public System.Int64 Id { get; set; }
    public System.String Email { get; set; } = System.String.Empty;
    public ICollection<System.String> Roles { get; set; } = new List<System.String>();

    public override void Update(DataModel other)
    {
        UserData update = other as UserData ?? throw new NotImplementedException();
        Email = update.Email;
        Roles = update.Roles;
    }
}