using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Juniper.TSBuild;

public class BuildSystemService<BuildConfigT> : IBuildSystemService
    where BuildConfigT : IBuildConfig, new()
{
    private readonly CancellationTokenSource serviceCancelled = new();
    private readonly BuildSystem<BuildConfigT> build = new();
    private readonly ILogger<BuildSystemService<BuildConfigT>> logger;
    public Task Started => build.Started;

    public Task Ready { get; }

    public BuildSystemService(IHostApplicationLifetime lifetime, ILogger<BuildSystemService<BuildConfigT>> logger)
    {
        this.logger = logger;
        Ready = RunBuildAsync();
        lifetime.ApplicationStopping.Register(Stop);
    }

    public event EventHandler NewBuildCompleted
    {
        add
        {
            build.NewBuildCompleted += value;
        }
        remove
        {
            build.NewBuildCompleted -= value;
        }
    }

    public void Stop()
    {
        serviceCancelled.Cancel();
    }

    private async Task RunBuildAsync()
    {
        try
        {
            logger.LogInformation("Starting build");
            await build.WatchAsync(true, serviceCancelled.Token);
            logger.LogInformation("Build ready");
        }
        catch (TaskCanceledException)
        {
            // do nothing
        }
        catch (Exception exp)
        {
            logger.LogError(exp, "Build failed");
        }
    }
}
