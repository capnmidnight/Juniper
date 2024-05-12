using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Juniper.Data;

[AttributeUsage(AttributeTargets.Property,  AllowMultiple = false)]
public class AutoIncludeNavigationAttribute : AbstractModelBuilderAttribute
{
    public override void Apply(ModelBuilder builder, Type type, PropertyInfo prop)
    {
        builder
            .Entity(type)
            .Navigation(prop.Name)
            .AutoInclude();
    }
}