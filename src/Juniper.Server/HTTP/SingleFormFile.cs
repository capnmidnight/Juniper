using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Http;

namespace Juniper.HTTP
{
    public class SingleFormFile
    {
        [Required]
        [Display(Name = "File")]
        public IFormFile FormFile { get; set; }
    }
}
