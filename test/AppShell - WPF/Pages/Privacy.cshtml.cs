using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Juniper.AppShell.Pages
{
    public class PrivacyModel : PageModel
    {
        private readonly ILogger logger;
        private readonly IAppShell appShell;

        public PrivacyModel(ILogger<PrivacyModel> logger, IAppShell appShell)
        {
            this.logger = logger;
            this.appShell = appShell;
        }

        public async Task OnGetAsync()
        {
            var source = await appShell.GetSourceAsync();
            var next = new Uri(source, "/");
            logger.LogInformation("Redirecting to {url}", next);
            await appShell.SetSourceAsync(next);
        }
    }
}