namespace Juniper.TSBuild
{
    public class BuildSystemService<BuildConfigT>: BackgroundService, IBuildSystemService
        where BuildConfigT : IBuildConfig, new()
    {
        private readonly BuildSystem<BuildConfigT> build;
        private readonly ILogger<BuildSystemService<BuildConfigT>> logger;

        public Task Ready { get; }

        public BuildSystemService(ILogger<BuildSystemService<BuildConfigT>> logger)
        {
            this.logger = logger;
            logger.LogInformation("Preparing build");
            build = new BuildSystem<BuildConfigT>();
            Ready = RunBuildAsync();
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            await Ready;
        }

        private async Task RunBuildAsync()
        {
            try
            {
                logger.LogInformation("Starting build...");
                await build.WatchAsync();
                logger.LogInformation("Build ready");
            }
            catch (Exception exp)
            {
                logger.LogError(exp, "Build failed");
                throw;
            }
        }
    }
}
