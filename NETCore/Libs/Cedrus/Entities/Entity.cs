using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;

using Juniper.Data;

using Microsoft.EntityFrameworkCore;

namespace Juniper.Cedrus.Entities;

[DebuggerDisplay($"{{{nameof(Id)}}}: Entity = {{{nameof(DisplayName)}}}")]
[AlternateKey(nameof(Name))]
[Index(nameof(Id), nameof(Name), nameof(TypeId), nameof(ClassificationId))]
public class Entity : INamed, IClassificationMarked, ICreationTracked
{
    public static void Configure(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Entity>(e =>
        {
            e.HasMany(p => p.Parents).WithOne(d => d.Child);
            e.HasMany(p => p.Children).WithOne(d => d.Parent);

            e.HasMany(p => p.Properties).WithOne(d => d.Entity);
            e.HasMany(p => p.ReferedProperties).WithOne(d => d.ReferenceEntity);
        });
    }

    [Key]
    public int Id { get; set; }

    public required string Name { get; set; }

    [ForeignKey(nameof(EntityType))]
    public int TypeId { get; set; }

    [AutoIncludeNavigation]
    [DeleteBehavior(DeleteBehavior.Cascade)]
    public required EntityType Type { get; set; }

    [ForeignKey(nameof(Classification))]
    public int ClassificationId { get; set; }

    [AutoIncludeNavigation]
    [DeleteBehavior(DeleteBehavior.Cascade)]
    public required Classification Classification { get; set; }

    [ForeignKey(nameof(User))]
    public int UserId { get; set; }

    [AutoIncludeNavigation]
    [DeleteBehavior(DeleteBehavior.NoAction)]
    public required CedrusUser User { get; set; }

    public DateTime CreatedOn { get; set; } = DateTime.Now;

    public ICollection<Property> Properties { get; set; } = new HashSet<Property>();

    public ICollection<Property> ReferedProperties { get; set; } = new HashSet<Property>();

    public ICollection<Relationship> Parents { get; set; } = new HashSet<Relationship>();

    public ICollection<Relationship> Children { get; set; } = new HashSet<Relationship>();

    public string DisplayName =>
        Properties
            ?.Where(v => v.Type?.Name == "Name" && v.Start <= DateTime.Now && DateTime.Now < v.End)
            ?.Select(v => v.Value as string)
            ?.Where(v => !string.IsNullOrWhiteSpace(v))
            ?.FirstOrDefault()
        ?? Name;
}
