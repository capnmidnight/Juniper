using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Juniper.Data;

[AttributeUsage(AttributeTargets.Property,  AllowMultiple = false)]
public class AutoIncludeNavigationAttribute : AbstractModelBuilderAttribute
{
    public override void Apply(ModelBuilder builder, Type type, PropertyInfo prop)
    {
        if(!prop.PropertyType.IsClass && !prop.PropertyType.IsInterface)
        {
            Console.Error.WriteLine("Navigation property is being defined for a property whose type is not a reference type ({0}:{1}). This is likely to be a mistake.", type.Name, prop.Name);
        }
        builder
            .Entity(type)
            .Navigation(prop.Name)
            .AutoInclude();
    }
}