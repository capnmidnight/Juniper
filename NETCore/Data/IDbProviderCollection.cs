using Microsoft.EntityFrameworkCore;

namespace Juniper.Data;

public interface IDbProviderCollection
{
    IDbProviderConfigurator Get<T>() where T : DbContext;
    void Set<T>(IDbProviderConfigurator configurator) where T : DbContext;
}

public class DbProviderCollection : IDbProviderCollection
{
    private readonly Dictionary<Type, IDbProviderConfigurator> configurations = new();

    public IDbProviderConfigurator Get<T>()
        where T : DbContext =>
        configurations[typeof(T)];

    public void Set<T>(IDbProviderConfigurator configurator)
        where T : DbContext =>
        configurations[typeof(T)] = configurator;
}