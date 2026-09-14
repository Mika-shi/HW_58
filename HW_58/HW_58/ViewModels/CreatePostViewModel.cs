using System.ComponentModel.DataAnnotations;

namespace HW_58.ViewModels;

public class CreatePostViewModel
{
    [Required(ErrorMessage = "Upload image")]
    public IFormFile? ImageFile { get; set; }

    [Required(ErrorMessage = "Enter description")]
    public string Description { get; set; } = "";
}