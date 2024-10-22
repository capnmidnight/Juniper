using System.Reflection;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Juniper.Data;

public static class IServiceScopeExt
{
    private static ILogger CreateLogger(IServiceScope scope, Type type)
    {
        var createLoggerMethod = typeof(IServiceProviderExtensions)
            .GetMethod(nameof(CreateLogger), BindingFlags.Static | BindingFlags.NonPublic)
            ?? throw new Exception("Can't find createLoggerMethod");

        var createLogger = createLoggerMethod.MakeGenericMethod(type);
        var logger = createLogger.Invoke(null, [scope]) as ILogger
            ?? throw new Exception("Couldn't create logger");

        return logger;
    }

    private static ILogger<T> CreateLogger<T>(IServiceScope scope) =>
        scope.ServiceProvider.GetRequiredService<ILogger<T>>();
}