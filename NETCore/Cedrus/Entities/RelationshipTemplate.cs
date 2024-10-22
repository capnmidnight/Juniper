#pragma warning disable IDE0028 // Simplify collection initialization
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;

using Juniper.Data;

using Microsoft.EntityFrameworkCore;

namespace Juniper.Cedrus.Entities;

[Index(nameof(Id), nameof(Name), nameof(EntityTypeId), nameof(RelationshipTypeId))]
[DebuggerDisplay($"{{{nameof(Id)}}}: RelationshipTemplate = {{{nameof(Name)}}}")]
public class RelationshipTemplate : INamed
{
    public static void Configure(ModelBuilder modelBuilder) =>
        modelBuilder.Entity<RelationshipTemplate>(e =>
        {
            e.HasOne(p => p.EntityType)
                .WithMany(d => d.RelationshipTemplates);

            e.HasMany(p => p.AllowedEntityTypes)
                .WithMany(d => d.AllowedInRelationshipTemplates);

            e.HasOne(p => p.PropertyEntityType)
                .WithMany(d => d.PropertiesOf);
        });

    [Key]
    public int Id { get; set; }

    public required string Name { get; set; }

    [ForeignKey(nameof(EntityType))]
    public int EntityTypeId { get; set; }

    [AutoIncludeNavigation]
    [DeleteBehavior(DeleteBehavior.Cascade)]
    public required EntityType EntityType { get; set; }

    [ForeignKey(nameof(RelationshipType))]
    public int RelationshipTypeId { get; set; }

    [AutoIncludeNavigation]
    [DeleteBehavior(DeleteBehavior.Cascade)]
    public required RelationshipType RelationshipType { get; set; }

    [AutoIncludeNavigation]
    public ICollection<EntityType> AllowedEntityTypes { get; set; } = new HashSet<EntityType>();

    [ForeignKey(nameof(PropertyEntityType))]
    public int? PropertyEntityTypeId { get; set; }

    [AutoIncludeNavigation]
    [DeleteBehavior(DeleteBehavior.SetNull)]
    public EntityType? PropertyEntityType { get; set; }

    public IEnumerable<EntityType> GetAllAllowedEntityTypes()
    {
        IEnumerable<EntityType> getAll()
        {
            var queue = new Queue<EntityType>(AllowedEntityTypes);
            while (queue.Count > 0)
            {
                var here = queue.Dequeue();
                queue.AddRange(here.Children);
                yield return here;
            }
        }

        return getAll().Distinct();
    }
}
#pragma warning restore IDE0028 // Simplify collection initialization
