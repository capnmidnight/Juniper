using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;

using Juniper.Data;

using Microsoft.EntityFrameworkCore;

namespace Juniper.Cedrus.Entities;

[DebuggerDisplay($"{{{nameof(Id)}}}: Relationship [{{{nameof(RelationshipTypeName)}}}] = {{{nameof(ParentName)}}} => {{{nameof(ChildName)}}}")]
[Index(nameof(Id), nameof(TypeId), nameof(ParentId), nameof(ChildId), nameof(ClassificationId), nameof(Start), nameof(End))]
public class Relationship : ITimeSeries<RelationshipType>, IClassificationMarked, ICreationTracked
{
    [Key]
    public int Id { get; set; }

    [ForeignKey(nameof(RelationshipType))]
    public int TypeId { get; set; }

    [AutoIncludeNavigation]
    [DeleteBehavior(DeleteBehavior.Cascade)]
    public required RelationshipType Type { get; set; }

    [ForeignKey(nameof(Entity))]
    public int ParentId { get; set; }

    [AutoIncludeNavigation]
    [DeleteBehavior(DeleteBehavior.Cascade)]
    public required Entity Parent { get; set; }

    [ForeignKey(nameof(Entity))]
    public int ChildId { get; set; }

    [AutoIncludeNavigation]
    [DeleteBehavior(DeleteBehavior.Cascade)]
    public required Entity Child { get; set; }

    [ForeignKey(nameof(Classification))]
    public int ClassificationId { get; set; }

    [AutoIncludeNavigation]
    [DeleteBehavior(DeleteBehavior.Cascade)]
    public required Classification Classification { get; set; }

    public required DateTime Start { get; set; }

    public DateTime End { get; set; } = DateTime.MaxValue;

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
