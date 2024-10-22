using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Juniper.Data;

public class MediaTypeConverter : ValueConverter<MediaType, string>
{
    public static void AddMediaTypeSupport(ModelBuilder builder)
    {
        var converter = new MediaTypeConverter();

        foreach (var entityType in builder.Model.GetEntityTypes())
        {
            var properties = from p in entityType.ClrType.GetProperties()
                             where p.PropertyType == typeof(MediaType)
                             select p;
            foreach (var property in properties)
            {
                builder
                    .Entity(entityType.Name)
                    .Property(property.Name)
                    .HasConversion(converter);
            }
        }
    }


    public MediaTypeConverter() : base(
        v => v.ToString(), 
        s => MediaType.Parse(s), 
        null)
    {
    }
}
