using System.Reflection;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Juniper.Data;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class SqlConversionAttribute<T> : AbstractModelBuilderAttribute
    where T : ValueConverter, new()
{
    public override void Apply(ModelBuilder builder, Type type, PropertyInfo prop)
    {
        builder
            .Entity(type)
            .Property(prop.Name)
            .HasConversion(new T());
    }
}
