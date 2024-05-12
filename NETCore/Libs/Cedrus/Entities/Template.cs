using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;

using Juniper.Data;

using Microsoft.EntityFrameworkCore;

namespace Juniper.Cedrus.Entities;

[DebuggerDisplay($"{{{nameof(Id)}}}: Template = {{{nameof(Name)}}}")]
[AlternateKey(nameof(Name))]
[Index(nameof(Id), nameof(Name), nameof(EntityTypeId))]
public class Template : INamed
{
    public static void Configure(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Template>(e =>
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