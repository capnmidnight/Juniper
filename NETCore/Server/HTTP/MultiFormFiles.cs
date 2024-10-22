using Microsoft.AspNetCore.Http;

using System.ComponentModel.DataAnnotations;

namespace Juniper.HTTP;

public class MultiFormFiles
{
    [Required]
    [Display(Name = "Files")]
    public IFormFile[]? FormFiles { get; set; }
}
