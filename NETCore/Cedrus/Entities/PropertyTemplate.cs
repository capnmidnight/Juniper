using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;

using Juniper.Data;

using Microsoft.EntityFrameworkCore;

namespace Juniper.Cedrus.Entities;

[Index(nameof(Id), nameof(Name), nameof(EntityTypeId))]
[DebuggerDisplay($"{{{nameof(Id)}}}: PropertyTemplate = {{{nameof(Name)}}}")]
public class PropertyTemplate : INamed
{
    public static void Configure(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PropertyTemplate>(e =>
        {
            e.HasMany(p => p.PropertyTypes)
                .WithMany(d => d.Templates);
        });
    }

    [Key]
    public int Id { get; set; }

    public required string Name { get; set; }

    [ForeignKey(nameof(EntityType))]
    public int EntityTypeId { get; set; }

    [AutoIncludeNavigation]
    [DeleteBehavior(DeleteBehavior.Cascade)]
    public required EntityType EntityType { get; set; }

    [AutoIncludeNavigation]
    public ICollection<PropertyType> PropertyTypes { get; set; } = new HashSet<PropertyType>();
}