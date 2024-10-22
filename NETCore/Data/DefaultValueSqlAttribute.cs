// Ignore Spelling: Sql

using System.Reflection;

using Microsoft.EntityFrameworkCore;

namespace Juniper.Data;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class DefaultValueSqlAttribute : AbstractModelBuilderAttribute
{
    private string Value { get; }

    public DefaultValueSqlAttribute(string value)
    {
        Value = value;
    }

    public override void Apply(ModelBuilder builder, Type type, PropertyInfo prop)
    {
        builder
            .Entity(type)
            .Property(prop.Name)
            .HasDefaultValueSql(Value);
    }
}
