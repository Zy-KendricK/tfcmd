using System.ComponentModel.DataAnnotations;

namespace Admin.Models;

public class LoginViewModel
{
    [Required(ErrorMessage = "Email or username is required")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    public bool RememberMe { get; set; }
}
