namespace Juniper.Data;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class JsonConversionAttribute : JsonConversionAttribute<object>
{
}