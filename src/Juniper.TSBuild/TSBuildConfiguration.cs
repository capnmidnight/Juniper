using Juniper.TSBuild;

namespace Juniper.Services
{
    public static class TSBuildConfiguration
    {
        public static WebApplicationBuilder ConfigureBuildSystem<BuildConfigT>(this WebApplicationBuilder builder)
            where BuildConfigT : IBuildConfig, new()
        {
            if (builder.Environment.IsDevelopment())
            {
                builder.Services
                    .AddSingleton<BuildSystemService<BuildConfigT>>()
                    .AddSingleton<IBuildSystemService>(serviceProvider =>
                        serviceProvider.GetRequiredService<BuildSystemService<BuildConfigT>>())
                    .AddHostedService(serviceProvider =>
                        serviceProvider.GetRequiredService<BuildSystemService<BuildConfigT>>());
            }
            return builder;
        }

        public static async Task BuildReady(this WebApplication app)
        {
            if(app.Environment.IsDevelopment())
            {
                var buildService = app.Services.GetRequiredService<IBuildSystemService>();
                await buildService.Ready;
            }
        }
    }
}
