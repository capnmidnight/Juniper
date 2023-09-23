// Ignore Spelling: Sql

using Microsoft.EntityFrameworkCore;

using System.Reflection;

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
    public static void Apply<ContextT>(this ContextT db, ModelBuilder builder)
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
}