// Ignore Spelling: Configurator
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HostFiltering;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Juniper.AppShell;

public static class AppShellConfiguration
{
    private static bool NoWebView => Environment.GetCommandLineArgs().Contains("--no-webview");
    /// <summary>
    /// Registers a service that opens a window constructed by the provided <typeparamref name="AppShellWindowFactoryT"/> factory.
    /// </summary>
    /// <typeparam name="AppShellWindowFactoryT">A concrete instance of the <see cref="IAppShellFactory"/> interface </typeparam>
    /// <param name="appBuilder"></param>
    /// <returns><paramref name="appBuilder"/></returns>
    public static WebApplicationBuilder ConfigureJuniperAppShell<AppShellWindowFactoryT>(this WebApplicationBuilder appBuilder, AppShellOptions? options = null)
        where AppShellWindowFactoryT : IAppShellFactory, new()
    {
        if (NoWebView)
        {
            return appBuilder;
        }

        appBuilder.Services.Configure<AppShellOptions>(appBuilder.Configuration.GetSection(AppShellOptions.AppShell));
        if (options is not null)
        {
            appBuilder.Services.PostConfigure<AppShellOptions>(oldOpts =>
            {
                oldOpts.SplashScreenPath = options.SplashScreenPath ?? oldOpts.SplashScreenPath;
                if(options.Window is not null)
                {
                    var oldWindow = oldOpts.Window;
                    oldOpts.Window = options.Window;
                    oldOpts.Window.Title = options.Window.Title ?? oldWindow?.Title;

                    if (options.Window.Size is not null)
                    {
                        var oldSize = oldWindow?.Size;
                        oldOpts.Window.Size = options.Window.Size;
                        oldOpts.Window.Size.Width = options.Window.Size.Width ?? oldSize?.Width;
                        oldOpts.Window.Size.Height = options.Window.Size.Height ?? oldSize?.Height;
                    }
                }
            });
        }

        appBuilder.Services.Configure<HostFilteringOptions>(options =>
        {
            options.AllowedHosts = new[] { "127.0.0.1" };
        });

        appBuilder.WebHost.UseUrls("http://127.0.0.1:0");

        appBuilder.Services
            // Give DI the class it needs to create
            .AddSingleton<AppShellService<AppShellWindowFactoryT>>()
            // Give DI an alias that other DI consumers can use to request the service without
            // knowing the specific type of `AppShellWindowFactorT`
            .AddSingleton<IAppShellService>(serviceProvider =>
                serviceProvider.GetRequiredService<AppShellService<AppShellWindowFactoryT>>())
            // Give DI an alias that other DI consumers can use to request the app shell without
            // knowing the specific type of `AppShellWindowFactorT`
            .AddSingleton<IAppShell>(serviceProvider =>
                serviceProvider.GetRequiredService<AppShellService<AppShellWindowFactoryT>>())
            // Queue the service for execution after the server has started.
            .AddHostedService(serviceProvider =>
                serviceProvider.GetRequiredService<AppShellService<AppShellWindowFactoryT>>());

        return appBuilder;
    }
}
