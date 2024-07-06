using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;

using Juniper.Data;

using Microsoft.EntityFrameworkCore;

namespace Juniper.Cedrus.Entities;

[DebuggerDisplay($"{{{nameof(Id)}}}: Caveat = {{{nameof(Name)}}}")]
[AlternateKey(nameof(Name))]
[Index(nameof(Id), nameof(Name), nameof(Description))]
public class ClassificationCaveat : INamed
{
    [Key]
    public int Id { get; set; }

    public required string Name { get; set; }

    public required string Description { get; set; }

    [ForeignKey(nameof(Level))]
    public int LevelId { get; set; }

    [DeleteBehavior(DeleteBehavior.Cascade)]
    public required ClassificationLevel Level { get; set; }

    public ICollection<Classification> Classifications { get; set; } = new HashSet<Classification>();
}
