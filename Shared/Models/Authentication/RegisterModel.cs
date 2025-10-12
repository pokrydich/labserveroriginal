namespace LabServer.Shared.Models;

using System.ComponentModel.DataAnnotations;

public class RegisterModel
{
    [Required]
    [EmailAddress]
    public System.String Email { get; set; } = System.String.Empty;

    [Required]
    [StringLength(100, ErrorMessage = "The {0} must be at least {2} at max {1} characters long.", MinimumLength = 6)]
    [DataType(DataType.Password)]
    public System.String Password { get; set; } = System.String.Empty;

    [DataType(DataType.Password)]
    [Display(Name = "Confirm password")]
    [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
    public System.String ConfirmPassword { get; set; } = System.String.Empty;
}