using Juniper.AppShell;
using Juniper.TSBuild;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Juniper;

public static class WebApplicationExt
{
    /// <summary>
    /// A task that resolves when the ESBuild process has finished at least once.
    /// </summary>
    /// <param name="app"></param>
    /// <returns></returns>
    public static async Task BuildAsync(this WebApplication app)
    {
        var appShellService = app.Services.GetService<IAppShellService>();
#if DEBUG
        var buildService = app.Services.GetService<IBuildSystemService>();
        IReadiable?[] services = [
            buildService,
            appShellService
        ];

        var tasks = services
            .Where(s => s?.Ready is not null)
            .Select(s => s!.Ready)
            .ToArray();

        await Task.WhenAll(tasks);
#else
        await (appShellService?.Ready ?? Task.CompletedTask);
#endif
    }

    /// <summary>
    /// Waits for the ESBuild process to finish at least once before starting the web server
    /// </summary>
    /// <param name="app"></param>
    /// <returns></returns>
    public static async Task BuildAndRunAsync(this WebApplication app)
    {
        await app.BuildAsync();
        await app.RunAsync();
    }

    /// <summary>
    /// Waits for the ESBuild process to finish at least once before starting the web server
    /// </summary>
    /// <param name="app"></param>
    /// <returns></returns>
    public static async Task BuildAndStartAsync(this WebApplication app)
    {
        await app.BuildAsync();
        await app.StartAsync();
    }
}