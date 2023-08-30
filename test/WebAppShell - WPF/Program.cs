using Juniper.Services;
using Juniper.AppShell.WPF;

WebApplication
    .CreateBuilder(args)
    .ConfigureJuniperWebApplication()
    .ConfigureJuniperAppShell<AppShellFactory>()
    .Build()
    .ConfigureJuniperRequestPipeline()
    .Start();