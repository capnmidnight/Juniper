// Ignore Spelling: Sql

using Microsoft.EntityFrameworkCore;

using System.Reflection;

namespace Juniper.Data;

public static class ContextExtensions
{
    public static void ApplyModelConfigurators<ContextT>(this ModelBuilder builder)
        where ContextT : DbContext
    {
        foreach (var configurator
            in from type in typeof(ContextT).Assembly.GetTypes()
               let typeName = type.Name
               from method in type.GetMethods(BindingFlags.Public | BindingFlags.Static)
               let parameters = method.GetParameters()
               where parameters.FirstOrDefault()?.ParameterType == typeof(ModelBuilder)
               select method)
        {
            configurator.Invoke(null, new[] { builder });
        }
    }

    public static void ApplyTableAttributes<ContextT>(this ModelBuilder builder)
        where ContextT : DbContext
    {
        foreach (var (type, attrs)
            in from type in typeof(ContextT).Assembly.GetTypes()
               let attrs = type.GetCustomAttributes().ToArray()
               where attrs.Length > 0
               select (type, attrs))
        {
            foreach (var attr in attrs)
            {
                if (attr is AlternateKeyAttribute unique)
                {
                    builder
                        .Entity(type)
                        .HasAlternateKey(unique.PropertyNames);
                }
            }
        }
    }

    public static void ApplyColumnAttributes<ContextT>(this ModelBuilder builder)
        where ContextT : DbContext
    {
        foreach (var (type, prop, attrs)
            in from type in typeof(ContextT).Assembly.GetTypes()
               from prop in type.GetProperties()
               let attrs = prop.GetCustomAttributes().ToArray()
               where attrs.Length > 0
               select (type, prop, attrs))
        {
            foreach (var attr in attrs)
            {
                if (attr is DefaultValueSqlAttribute defValue)
                {
                    builder
                        .Entity(type)
                        .Property(prop.Name)
                        .HasDefaultValueSql(defValue.Value);
                }
            }
        }
    }

    public static void JuniperModelCreating<ContextT>(this ModelBuilder builder)
        where ContextT : DbContext
    {
        builder.ApplyModelConfigurators<ContextT>();
        builder.ApplyTableAttributes<ContextT>();
        builder.ApplyColumnAttributes<ContextT>();
    }
}