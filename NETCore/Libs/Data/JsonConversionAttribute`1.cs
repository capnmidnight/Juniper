using System.Reflection;

using Microsoft.EntityFrameworkCore;

using Newtonsoft.Json;

namespace Juniper.Data;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class JsonConversionAttribute<T> : AbstractModelBuilderAttribute
{
    private static readonly JsonSerializerSettings settings = new (){ NullValueHandling = NullValueHandling.Ignore };
    public override void Apply(ModelBuilder builder, Type type, PropertyInfo prop)
    {
        builder
            .Entity(type)
            .Property<T>(prop.Name)
            .HasConversion(
                v => JsonConvert.SerializeObject(v, settings),
                v => JsonConvert.DeserializeObject<T>(v, settings)!
            );
    }
}