using System.ComponentModel.DataAnnotations;

namespace Juniper.HTTP;

public class SingleFormFile
{
    [Required]
    [Display(Name = "File")]
    public IFormFile? FormFile { get; set; }
}
