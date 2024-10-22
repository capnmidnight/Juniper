using System.Reflection;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Juniper.Data;

public static class DbContextExtensions
{
    public static async Task ApplySeedersAsync<ContextT>(this ContextT context, IServiceProvider services, IConfiguration config, ILogger<ContextT> logger)
        where ContextT : DbContext
    {
        var signatures = new object[][]
        {
            [],
            [context],
            [logger],
            [services],
            [config],
            [logger, context],
            [services, context ],
            [config, context ],
            [context, services ],
            [logger, services ],
            [config, services ],
            [context, config ],
            [logger, config ],
            [services, config ],
            [context, logger ],
            [services, logger ],
            [config, logger ],
            [logger, services, context ],
            [config, services, context ],
            [logger, config, context ],
            [services, config, context ],
            [services, logger, context ],
            [config, logger, context ],
            [logger, context, services ],
            [config, context, services ],
            [context, config, services ],
            [logger, config, services ],
            [context, logger, services ],
            [config, logger, services ],
            [logger, context, config ],
            [services, context, config ],
            [context, services, config ],
            [logger, services, config ],
            [context, logger, config ],
            [services, logger, config ],
            [services, context, logger ],
            [config, context, logger ],
            [context, services, logger ],
            [config, services, logger ],
            [context, config, logger ],
            [services, config, logger ],
            [logger, config, services, context ],
            [config, logger, services, context ],
            [logger, services, config, context ],
            [services, logger, config, context ],
            [config, services, logger, context ],
            [services, config, logger, context ],
            [logger, config, context, services ],
            [config, logger, context, services ],
            [logger, context, config, services ],
            [context, logger, config, services ],
            [config, context, logger, services ],
            [context, config, logger, services ],
            [logger, services, context, config ],
            [services, logger, context, config ],
            [logger, context, services, config ],
            [context, logger, services, config ],
            [services, context, logger, config ],
            [context, services, logger, config ],
            [config, services, context, logger ],
            [services, config, context, logger ],
            [config, context, services, logger ],
            [context, config, services, logger ],
            [services, context, config, logger ],
            [context, services, config, logger]
        };

        var contextType = context.GetType();

        var assemblies = new[]
        {
            contextType.Assembly,
            Assembly.GetEntryAssembly(),
            Assembly.GetExecutingAssembly(),
            Assembly.GetCallingAssembly()
        }.Distinct();

        var nonContextMethods = from assembly in assemblies
                                from type in assembly.GetTypes()
                                from method in type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)
                                select method;

        var contextMethods = contextType.GetMethods(BindingFlags.Public | BindingFlags.Instance);

        var methods = from method in nonContextMethods.Union(contextMethods)
                      let attr = method.GetCustomAttribute<DataSeederAttribute>()
                      where attr is not null
                      orderby attr.Order
                      select method;

        foreach (var seeder in methods)
        {
            var parameters = seeder.GetParameters();
            var found = false;
            foreach (var args in signatures)
            {
                if (parameters.Length == args.Length
                    && parameters
                        .Select((p, i) => p.ParameterType.IsAssignableFrom(args[i].GetType()))
                        .All(v => v))
                {
                    found = true;
                    var result = seeder.Invoke(context, args);
                    if (result is Task task)
                    {
                        await task;
                    }
                    await context.SaveChangesAsync();
                    break;
                }
            }

            if (!found)
            {
                logger.LogWarning("{TypeName}::{MethodName} defines a seeder method that does not have the right parameter signature. Must be ({ContextTName}, {IServiceProviderName}, {ILoggerName})",
                    seeder.DeclaringType?.Name ?? "{Anon-Type}",
                    seeder.Name,
                    nameof(ContextT),
                    nameof(IServiceProvider),
                    nameof(ILogger));
                continue;
            }
        }
    }
}
