// Ignore Spelling: Sql

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
