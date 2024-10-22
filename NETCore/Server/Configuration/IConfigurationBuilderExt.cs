using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

using System.Reflection;

namespace Juniper.Configuration;

public static class IConfigurationBuilderExt
{
    public static IConfigurationBuilder AddAssemblyUserSecrets(this IConfigurationBuilder configBuilder, IHostEnvironment env)
    {
#if DEBUG
        if (!env.IsDevelopment())
        {
            var assembly = Assembly.GetEntryAssembly();
            if (assembly is not null)
            {
                configBuilder.AddUserSecrets(assembly);
            }
        }
#endif
        return configBuilder;
    }
}
