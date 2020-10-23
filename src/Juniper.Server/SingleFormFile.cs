using Microsoft.AspNetCore.Http;

using System.ComponentModel.DataAnnotations;

namespace Juniper.Server.HTTP
{
    public class SingleFormFile
    {
        [Required]
        [Display(Name = "File")]
        public IFormFile FormFile { get; set; }
    }
}
