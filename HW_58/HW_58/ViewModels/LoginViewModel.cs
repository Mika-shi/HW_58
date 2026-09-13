using System.ComponentModel.DataAnnotations;

namespace HW_58.ViewModels;

public class LoginViewModel
{
    [Required(ErrorMessage = "Enter login or email")]
    public string LoginOrEmail { get; set; } = "";

    [Required(ErrorMessage = "Enter password")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = "";

    public bool RememberMe { get; set; }

    public string? ReturnUrl { get; set; }
}