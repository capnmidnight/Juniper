// Ignore Spelling: Configurator
using Juniper.Services;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HostFiltering;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Juniper.AppShell;

public static class AppShellConfiguration
{
#if NO_APP_SHELL
    public static readonly bool NoAppShell = true;
#else
    public static bool NoAppShell => Environment.GetCommandLineArgs().Contains("--no-appshell");
#endif
    /// <summary>
    /// Registers a service that opens a window constructed by the provided <typeparamref name="AppShellFactoryT"/> factory.
    /// </summary>
    /// <typeparam name="AppShellFactoryT">A concrete instance of the <see cref="IAppShellFactory"/> interface </typeparam>
    /// <param name="appBuilder"></param>
    /// <returns><paramref name="appBuilder"/></returns>
    public static WebApplicationBuilder AddJuniperAppShell<AppShellFactoryT>(this WebApplicationBuilder appBuilder, AppShellOptions? options = null)
        where AppShellFactoryT : class, IAppShellFactory, new()
    {
        if (NoAppShell)
        {
            return appBuilder;
        }

        appBuilder.Services
            .AddSingleton<PortDiscoveryService>()
            .AddSingleton<IPortDiscoveryService>(serviceProvider =>
                serviceProvider.GetRequiredService<PortDiscoveryService>())
            .AddHostedService(serviceProvider =>
                serviceProvider.GetRequiredService<PortDiscoveryService>());

        // We have to manually instruct the system to search for the AppShellController
        // because the optimized Release build would otherwise elide the code, as there
        // would be no physical reference to it anywhere and would look like dead-code.
        appBuilder.AddPart<AppShellController>();

        // <Normalize the options>
        var configSection = appBuilder.Configuration.GetSection(AppShellOptions.AppShell);
        appBuilder.Services.Configure<AppShellOptions>(configSection);
        if (options is not null)
        {
            appBuilder.Services.PostConfigure<AppShellOptions>(oldOpts =>
            {
                oldOpts.SplashScreenPath = options.SplashScreenPath ?? oldOpts.SplashScreenPath;
                oldOpts.ApplicationURI = options.ApplicationURI ?? oldOpts.ApplicationURI;
                if (options.Window is not null)
                {
                    var oldWindow = oldOpts.Window;
                    oldOpts.Window = options.Window;
                    oldOpts.Window.Title = options.Window.Title ?? oldWindow?.Title;
                    oldOpts.Window.Fullscreen = options.Window.Fullscreen ?? oldWindow?.Fullscreen;
                    oldOpts.Window.Borderless = options.Window.Borderless ?? oldWindow?.Borderless;
                    oldOpts.Window.Maximized = options.Window.Maximized ?? oldWindow?.Maximized;

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
        // </Normalize the options>


        // Limit AppShell programs to listening on LocalHost, so they don't accidentally
        // open whole in the user's system.
        appBuilder.Services.Configure<HostFilteringOptions>(options =>
        {
            options.AllowedHosts = ["127.0.0.1"];
        });

        // Specifying port 0 here instructs the OS to assign a random port so we don't
        // clash with any other AppShell and/or Electron apps on the system.
        appBuilder.WebHost.UseUrls("http://127.0.0.1:0");

        appBuilder.Services

            // Make the service provider instantiate the factory...
            .AddSingleton<AppShellFactoryT>()
            // ... so that it's available for DI in the AppShellService.
            .AddSingleton<IAppShellFactory>(serviceProvider =>
                serviceProvider.GetRequiredService<AppShellFactoryT>())


            // Make the service provider instantiate the service...
            .AddSingleton<AppShellService>()
            // ... so that it's available for DI to other resources
            // so that it can forward method calls to the AppShell
            .AddSingleton<IAppShell>(serviceProvider =>
                serviceProvider.GetRequiredService<AppShellService>())
            .AddSingleton<IAppShellService>(serviceProvider =>
                serviceProvider.GetRequiredService<AppShellService>());

        return appBuilder;
    }
}
