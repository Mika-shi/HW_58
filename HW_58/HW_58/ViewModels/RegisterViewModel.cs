using System.ComponentModel.DataAnnotations;
using HW_58.Models.Enums;

namespace HW_58.ViewModels;

public class RegisterViewModel
{
    [Required(ErrorMessage = "Enter login")]
    public string Login { get; set; } = "";

    [Required(ErrorMessage = "Enter email")]
    [EmailAddress(ErrorMessage = "Enter correct email")]
    public string Email { get; set; } = "";

    [Required(ErrorMessage = "Upload avatar")]
    public IFormFile? AvatarFile { get; set; }

    [Required(ErrorMessage = "Enter password")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = "";

    [Required(ErrorMessage = "Confirm password")]
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Passwords do not match")]
    public string ConfirmPassword { get; set; } = "";

    [Required(ErrorMessage = "Enter full name")]
    public string FullName { get; set; } = "";

    public string Bio { get; set; } = "";

    public string PhoneNumber { get; set; } = "";

    public Gender Gender { get; set; } = Gender.NotSpecified;
}