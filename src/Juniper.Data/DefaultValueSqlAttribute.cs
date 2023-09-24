// Ignore Spelling: Sql

using Microsoft.EntityFrameworkCore;

using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;

namespace Juniper.Data;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class DefaultValueSqlAttribute : Attribute
{
    public string Value { get; }

    public DefaultValueSqlAttribute(string value)
    {
        Value = value;
    }
}

public static class ContextExtensions
{
    public static void ApplyModelConfigurators<ContextT>(this ContextT db, ModelBuilder builder)
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

    public static void ApplyDefaultSqlValues<ContextT>(this ContextT db, ModelBuilder builder)
        where ContextT : DbContext
    {
        foreach (var (type, prop, attr)
            in from type in typeof(ContextT).Assembly.GetTypes()
               from prop in type.GetProperties()
               let attr = prop.GetCustomAttribute<DefaultValueSqlAttribute>()
               where attr is not null
               select (type, prop, attr))
        {
            builder
                .Entity(type)
                .Property(prop.Name)
                .HasDefaultValueSql(attr.Value);
        }
    }

    public static void JuniperModelCreating<ContextT>(this ContextT db, ModelBuilder builder)
        where ContextT : DbContext
    {
        db.ApplyModelConfigurators(builder);
        db.ApplyDefaultSqlValues(builder);
    }
}