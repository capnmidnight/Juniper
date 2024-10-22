using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;

using Juniper.Data;

using Microsoft.EntityFrameworkCore;

namespace Juniper.Cedrus.Entities;

[DebuggerDisplay($"{{{nameof(Id)}}}: RelationshipType = {{{nameof(Name)}}}")]
[AlternateKey(nameof(Name))]
[Index(nameof(Id), nameof(Name), nameof(ChildRole))]
public class RelationshipType : INamed
{
    [Key]
    public int Id { get; set; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    [Column("ParentRole")]
    public string Name { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    /// <summary>
    /// What to call this relationship when looking from parent to child.
    /// </summary>
    [NotMapped]
    public required string ParentRole
    {
        get => Name;
        set => Name = value;
    }

    /// <summary>
    /// What to call this relationship when looking from child to parent.
    /// </summary>
    public required string ChildRole { get; set; }

    public ICollection<Relationship> Relationships { get; set; } = new HashSet<Relationship>();

    public ICollection<RelationshipTemplate> Templates { get; set; } = new HashSet<RelationshipTemplate>();
}