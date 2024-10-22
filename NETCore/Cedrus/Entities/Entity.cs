#pragma warning disable IDE0028 // Simplify collection initialization
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;

using Juniper.Data;

using Microsoft.EntityFrameworkCore;

namespace Juniper.Cedrus.Entities;

[DebuggerDisplay($"{{{nameof(Id)}}}: Entity = {{{nameof(DisplayName)}}}")]
[AlternateKey(nameof(Name))]
[Index(nameof(Id), nameof(Name), nameof(TypeId))]
public class Entity : INamed, ICreationTracked
{
    public static void Configure(ModelBuilder modelBuilder) =>
        modelBuilder.Entity<Entity>(e =>
        {
            e.HasOne(p => p.PropertiesOf).WithOne(d => d.PropertyEntity);
            e.HasOne(p => p.User).WithMany(d => d.EntitiesCreated);
            e.HasOne(p => p.ReviewedByUser).WithMany(d => d.EntitiesReviewed);
        });

    [Key]
    public int Id { get; set; }

    public required string Name { get; set; }

    [ForeignKey(nameof(Type))]
    public int TypeId { get; set; }

    [AutoIncludeNavigation]
    [DeleteBehavior(DeleteBehavior.Cascade)]
    public required EntityType Type { get; set; }

    public Relationship? PropertiesOf { get; set; }

    [ForeignKey(nameof(User))]
    public int UserId { get; set; }

    [AutoIncludeNavigation]
    [DeleteBehavior(DeleteBehavior.NoAction)]
    public required CedrusUser User { get; set; }

    public DateTime CreatedOn { get; set; } = DateTime.Now;

    [AutoIncludeNavigation]
    [DeleteBehavior(DeleteBehavior.NoAction)]
    public CedrusUser? ReviewedByUser { get; set; }

    public DateTime? ReviewedOn { get; set; }

    [ForeignKey(nameof(ReviewedByUser))]
    public int? ReviewedBy { get; set; }

    public ICollection<Property> Properties { get; set; } = new HashSet<Property>();

    public ICollection<Property> ReferenceFor { get; set; } = new HashSet<Property>();

    public ICollection<Relationship> Parents { get; set; } = new HashSet<Relationship>();

    public ICollection<Relationship> Children { get; set; } = new HashSet<Relationship>();

    public string DisplayName =>
        Properties
            ?.Where(v => v.Type?.Name == "Name")
            ?.Select(v => v.Value as string)
            ?.Where(v => !string.IsNullOrWhiteSpace(v))
            ?.Select(v => v!.RemoveQuotes())
            ?.FirstOrDefault()
        ?? Name;
}
#pragma warning restore IDE0028 // Simplify collection initialization
