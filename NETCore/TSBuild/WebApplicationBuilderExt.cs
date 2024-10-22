using Juniper.TSBuild;

using Microsoft.AspNetCore.Builder;

namespace Juniper;

public static class WebApplicationBuilderExt
{
    /// <summary>
    /// Adds an ESBuild-based TypeScript project compilation system to the currently running
    /// application. The compilers will run in a separate process, but report all progress
    /// back to this process.
    /// </summary>
    /// <typeparam name="BuildConfigT"></typeparam>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static WebApplicationBuilder AddJuniperBuildSystem<BuildConfigT>(this WebApplicationBuilder builder)
        where BuildConfigT : IBuildConfig, new()
    {
#if DEBUG
        builder.Services.AddJuniperBuildSystem<BuildConfigT>(builder.Environment);
#endif
        return builder;
    }
}
