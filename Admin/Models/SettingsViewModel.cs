namespace Admin.Models;

public class SettingsViewModel
{
    public string Email { get; set; } = string.Empty;
    public string? CurrentPassword { get; set; }
    public string? NewPassword { get; set; }
    public string? ConfirmPassword { get; set; }
}
