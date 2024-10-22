using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;

using Juniper.Data;

using Microsoft.EntityFrameworkCore;

namespace Juniper.Cedrus.Entities;

[DebuggerDisplay($"{{{nameof(Id)}}}: EntityType = {{{nameof(Name)}}}")]
[AlternateKey(nameof(Name))]
[Index(nameof(Id), nameof(Name))]
public class EntityType : INamed, IParentChained<EntityType>
{
    public static void Configure(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<EntityType>(e =>
        {
            e.HasOne(p => p.Parent)
                .WithMany(d => d.Children)
                .HasForeignKey(p => p.ParentId);
        });
    }

    [Key]
    public int Id { get; set; }
    public required string Name { get; set; }

    public required bool IsPrimary { get; set; }

    [ForeignKey(nameof(Parent))]
    public int? ParentId { get; set; }

    [DeleteBehavior(DeleteBehavior.SetNull)]
    public EntityType? Parent { get; set; }

    public ICollection<EntityType> Children { get; set; } = new HashSet<EntityType>();

    public ICollection<PropertyTemplate> PropertyTemplates { get; set; } = new HashSet<PropertyTemplate>();

    public ICollection<RelationshipTemplate> RelationshipTemplates { get; set; } = new HashSet<RelationshipTemplate>();

    public ICollection<RelationshipTemplate> PropertiesOf { get; set; } = new HashSet<RelationshipTemplate>();

    public ICollection<RelationshipTemplate> AllowedInRelationshipTemplates { get; set; } = new HashSet<RelationshipTemplate>();

    public ICollection<Entity> Entities { get; set; } = new HashSet<Entity>();
}