using System.Reflection;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Juniper.Data;

public static class DbContextExtensions
{
    public static void ApplySeeders<ContextT>(this ContextT context, IServiceProvider services, IConfiguration config, ILogger<ContextT> logger)
        where ContextT : DbContext
    {
        var signatures = new[]
        {
            Array.Empty<object>(),
            new object[]{ context },
            new object[]{ logger },
            new object[]{ services },
            new object[]{ config },
            new object[]{ logger, context },
            new object[]{ services, context },
            new object[]{ config, context },
            new object[]{ context, services },
            new object[]{ logger, services },
            new object[]{ config, services },
            new object[]{ context, config },
            new object[]{ logger, config },
            new object[]{ services, config },
            new object[]{ context, logger },
            new object[]{ services, logger },
            new object[]{ config, logger },
            new object[]{ logger, services, context },
            new object[]{ config, services, context },
            new object[]{ logger, config, context },
            new object[]{ services, config, context },
            new object[]{ services, logger, context },
            new object[]{ config, logger, context },
            new object[]{ logger, context, services },
            new object[]{ config, context, services },
            new object[]{ context, config, services },
            new object[]{ logger, config, services },
            new object[]{ context, logger, services },
            new object[]{ config, logger, services },
            new object[]{ logger, context, config },
            new object[]{ services, context, config },
            new object[]{ context, services, config },
            new object[]{ logger, services, config },
            new object[]{ context, logger, config },
            new object[]{ services, logger, config },
            new object[]{ services, context, logger },
            new object[]{ config, context, logger },
            new object[]{ context, services, logger },
            new object[]{ config, services, logger },
            new object[]{ context, config, logger },
            new object[]{ services, config, logger },
            new object[]{ logger, config, services, context },
            new object[]{ config, logger, services, context },
            new object[]{ logger, services, config, context },
            new object[]{ services, logger, config, context },
            new object[]{ config, services, logger, context },
            new object[]{ services, config, logger, context },
            new object[]{ logger, config, context, services },
            new object[]{ config, logger, context, services },
            new object[]{ logger, context, config, services },
            new object[]{ context, logger, config, services },
            new object[]{ config, context, logger, services },
            new object[]{ context, config, logger, services },
            new object[]{ logger, services, context, config },
            new object[]{ services, logger, context, config },
            new object[]{ logger, context, services, config },
            new object[]{ context, logger, services, config },
            new object[]{ services, context, logger, config },
            new object[]{ context, services, logger, config },
            new object[]{ config, services, context, logger },
            new object[]{ services, config, context, logger },
            new object[]{ config, context, services, logger },
            new object[]{ context, config, services, logger },
            new object[]{ services, context, config, logger },
            new object[]{ context, services, config, logger }
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
            foreach (var param in signatures)
            {
                if (parameters.Length == param.Length
                    && parameters
                        .Select((p, i) => p.ParameterType.IsAssignableFrom(param[i].GetType()))
                        .All(v => v))
                {
                    found = true;
                    seeder.Invoke(context, param);
                    context.SaveChanges();
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
