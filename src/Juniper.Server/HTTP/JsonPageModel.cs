using System.Text.Json;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Hosting;

namespace Juniper.HTTP
{
    public abstract class JsonPageModel : PageModel
    {
        private readonly JsonSerializerOptions serializerOptions;

        protected JsonPageModel(IHostEnvironment env)
        {
            Environment = env;

            serializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = Environment.IsDevelopment()
            };
        }

        protected IHostEnvironment Environment { get; }

        protected IActionResult Json<T>(T value)
        {
            return JsonBlobResult.Create(value, serializerOptions);
        }
    }
}
