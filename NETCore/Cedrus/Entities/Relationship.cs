using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;

using Juniper.Data;

using Microsoft.EntityFrameworkCore;

namespace Juniper.Cedrus.Entities;

[DebuggerDisplay($"{{{nameof(Id)}}}: Relationship [{{{nameof(RelationshipTypeName)}}}] = {{{nameof(ParentName)}}} => {{{nameof(ChildName)}}}")]
[Index(nameof(Id), nameof(TypeId), nameof(ParentId), nameof(ChildId))]
public class Relationship : ICreationTracked, ISequenced
{
    public static void Configure(ModelBuilder modelBuilder) =>
        modelBuilder.Entity<Relationship>(e =>
        {
            e.HasOne(p => p.Parent).WithMany(d => d.Children);
            e.HasOne(p => p.Child).WithMany(d => d.Parents);
        });

    [Key]
    public int Id { get; set; }

    [ForeignKey(nameof(Type))]
    public int TypeId { get; set; }

    [AutoIncludeNavigation]
    [DeleteBehavior(DeleteBehavior.Cascade)]
    public required RelationshipType Type { get; set; }

    [ForeignKey(nameof(Parent))]
    public int ParentId { get; set; }

    [AutoIncludeNavigation]
    [DeleteBehavior(DeleteBehavior.Cascade)]
    public required Entity Parent { get; set; }

    [ForeignKey(nameof(Child))]
    public int ChildId { get; set; }

    [AutoIncludeNavigation]
    [DeleteBehavior(DeleteBehavior.Cascade)]
    public required Entity Child { get; set; }

    [ForeignKey(nameof(PropertyEntity))]
    public int? PropertyEntityId { get; set; }

    [AutoIncludeNavigation]
    [DeleteBehavior(DeleteBehavior.SetNull)]
    public Entity? PropertyEntity { get; set; }

    [ForeignKey(nameof(User))]
    public int UserId { get; set; }

    [AutoIncludeNavigation]
    [DeleteBehavior(DeleteBehavior.NoAction)]
    public required CedrusUser User { get; set; }

    public DateTime CreatedOn { get; set; } = DateTime.Now;

    internal string RelationshipTypeName => Type.Name;
    internal string ParentName => Parent.DisplayName;
    internal string ChildName => Child.DisplayName;
}
