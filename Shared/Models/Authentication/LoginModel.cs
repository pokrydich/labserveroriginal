namespace LabServer.Shared.Models;

using System.ComponentModel.DataAnnotations;

public class LoginModel
{
    [Required]
    public System.String Email { get; set; } = System.String.Empty;
    [Required]
    public System.String Password { get; set; } = System.String.Empty;
    public System.Boolean RememberMe { get; set; } = false;
}