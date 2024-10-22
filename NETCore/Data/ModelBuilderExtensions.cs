// Ignore Spelling: Sql

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

using System.Reflection;

namespace Juniper.Data;

public static class ModelBuilderExtensions
{
    public static void ApplyModelConfigurators<ContextT>(this ModelBuilder builder)
        where ContextT : DbContext
    {
        var param = new[] { builder };

        foreach (var configurator
            in from type in typeof(ContextT).Assembly.GetTypes()
               from method in type.GetMethods(BindingFlags.Public | BindingFlags.Static)
               let parameters = method.GetParameters()
               where parameters.Length == 1
                    && parameters[0].ParameterType == typeof(ModelBuilder)
               select method)
        {
            configurator.Invoke(null, param);
        }
    }

    public static void ApplyTableAttributes<ContextT>(this ModelBuilder builder)
        where ContextT : DbContext
    {
        foreach (var (type, attr)
            in from type in typeof(ContextT).Assembly.GetTypes()
               from attr in type.GetCustomAttributes()
               where attr is AlternateKeyAttribute
               select (type, (AlternateKeyAttribute)attr))
        {
            builder
                .Entity(type)
                .HasAlternateKey(attr.PropertyNames);
        }
    }

    public static void ApplyColumnAttributes<ContextT>(this ModelBuilder builder)
        where ContextT : DbContext
    {
        var attrs = from type in typeof(ContextT).Assembly.GetTypes()
                    from prop in type.GetProperties()
                    from attr in prop.GetCustomAttributes()
                    where attr is AbstractModelBuilderAttribute
                    select (type, prop, (AbstractModelBuilderAttribute)attr);

        foreach (var (type, prop, attr) in attrs)
        {
            attr.Apply(builder, type, prop);
        }
    }

    public static void JuniperModelCreating<ContextT>(this ModelBuilder builder, DbContext db)
        where ContextT : DbContext
    {
        var configurators = db.GetService<IDbProviderCollection>();
        configurators.Get<ContextT>().OnModelCreating(builder);

        builder.ApplyModelConfigurators<ContextT>();
        builder.ApplyTableAttributes<ContextT>();
        builder.ApplyColumnAttributes<ContextT>();
    }
}
