﻿using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Juniper.AppShell.Pages
{
    public class PrivacyModel : PageModel
    {
        private readonly ILogger logger;

        public PrivacyModel(ILogger<PrivacyModel> logger)
        {
            this.logger = logger;
        }

        public void OnGet()
        {
        }
    }
}