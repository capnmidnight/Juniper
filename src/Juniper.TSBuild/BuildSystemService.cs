using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Juniper.TSBuild;

public class BuildSystemService<BuildConfigT> : BackgroundService, IBuildSystemService
    where BuildConfigT : IBuildConfig, new()
{
    private readonly CancellationTokenSource canceller = new();
    private readonly BuildSystem<BuildConfigT> build;
    private readonly ILogger<BuildSystemService<BuildConfigT>> logger;

    public Task Ready { get; }
    private Timer? timer;

    public BuildSystemService(ILogger<BuildSystemService<BuildConfigT>> logger)
    {
        this.logger = logger;
        logger.LogInformation("Preparing build");
        build = new BuildSystem<BuildConfigT>();
        Ready = RunBuildAsync(canceller.Token);
        canceller.Token.Register(() =>
            timer?.Dispose());
    }

    public void Stop()
    {
        canceller.Cancel();
    }

    protected override async Task ExecuteAsync(CancellationToken appCancellationToken)
    {
        var appCancellation = new TaskCompletionSource();
        appCancellationToken.Register(appCancellation.SetResult);
        await Task.WhenAny(Ready, appCancellation.Task);
        if (appCancellationToken.IsCancellationRequested)
        {
            canceller.Cancel();
        }
    }

    private async Task RunBuildAsync(CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Starting build...");
            timer = await build.WatchAsync(cancellationToken, true);
            logger.LogInformation("Build ready");
        }
        catch (Exception exp)
        {
            logger.LogError(exp, "Build failed");
            throw;
        }
    }
}
