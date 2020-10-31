using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Http;

namespace Juniper.Server.HTTP
{
    public class SingleFormFile
    {
        [Required]
        [Display(Name = "File")]
        public IFormFile FormFile { get; set; }
    }
}
