using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Juniper.TSBuild;

public class BuildSystemService<BuildConfigT> : IBuildSystemService
    where BuildConfigT : IBuildConfig, new()
{
    private readonly CancellationTokenSource serviceCancelled = new();
    private readonly ILogger<BuildSystemService<BuildConfigT>> logger;
    private readonly BuildSystem<BuildConfigT> build;

    public Task Started => build.Started;

    private readonly TaskCompletionSource ready = new();
    public Task Ready => ready.Task;
    public Task Complete {get;}

    public BuildSystemService(IHostApplicationLifetime lifetime, ILogger<BuildSystemService<BuildConfigT>> logger)
    {
        this.logger = logger;
        build = new BuildSystem<BuildConfigT>(logger);
        lifetime.ApplicationStopping.Register(Stop);
        Complete = RunBuildAsync();
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
            ready.TrySetResult();
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
