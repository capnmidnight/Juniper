// Ignore Spelling: Sql

namespace Juniper.Data;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class DataSeederAttribute : Attribute
{
    public int Order { get; }
    
    public DataSeederAttribute(int order = 0)
    {
        Order = order;
    }
}