using Juniper.Processes;
using Juniper.Services;
using Juniper.TSBuild;

using Microsoft.AspNetCore.HttpLogging;


namespace Juniper.Examples
{
    public class Startup
    {
        private readonly IWebHostEnvironment env;
        private readonly IConfiguration config;

        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            config = configuration;
            env = environment;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        BuildSystem? build;
        public void ConfigureServices(IServiceCollection services)
        {
            var config = new DefaultConfiguration.Options();

            services.ConfigureDefaultServices(env, config);

            if (env.IsDevelopment())
            {
                try
                {
                    build = new BuildSystem(BuildConfig.GetBuildConfig());

                    build.WatchAsync().Wait();
                }
                catch (BuildSystemProjectRootNotFoundException exp)
                {
                    Console.WriteLine("WARNING: {0}", exp.Message);
                }

                services.AddHttpLogging(opts =>
                {
                    opts.LoggingFields = HttpLoggingFields.All;
                    opts.RequestHeaders.Add("host");
                    opts.RequestHeaders.Add("user-agent");
                });
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, ILogger<Startup> logger)
        {
            if (env.IsDevelopment())
            {
                app.UseIPBanList("::ffff:10.20.22.108");
            }

            app.ConfigureRequestPipeline(env, config, logger)
                .UseHttpLogging();
        }
    }
}
