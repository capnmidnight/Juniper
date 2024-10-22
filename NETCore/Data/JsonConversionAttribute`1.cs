using System.Reflection;

using Microsoft.EntityFrameworkCore;

using System.Text.Json;

namespace Juniper.Data;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class JsonConversionAttribute<T> : AbstractModelBuilderAttribute
{
    private static readonly JsonSerializerOptions settings = new()
    {
        DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
    };
    public override void Apply(ModelBuilder builder, Type type, PropertyInfo prop)
    {
        builder
            .Entity(type)
            .Property<T>(prop.Name)
            .HasConversion(
                v => Serialize(v),
                v => Deserialize(v)!
            );
    }

    private static string Serialize(T v)
    {
        return JsonSerializer.Serialize(v, settings);
    }

    private static T? Deserialize(string v)
    {
        return JsonSerializer.Deserialize<T>(v, settings);
    }
}