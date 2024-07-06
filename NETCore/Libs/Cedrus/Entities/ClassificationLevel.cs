using Juniper.Data;

using Microsoft.EntityFrameworkCore;

using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace Juniper.Cedrus.Entities;

[DebuggerDisplay($"{{{nameof(Id)}}}: Level = {{{nameof(Name)}}}")]
[AlternateKey(nameof(Name))]
[Index(nameof(Id), nameof(Name), nameof(Description))]
public class ClassificationLevel : INamed, ISequenced
{
    [Key]
    public int Id { get; set; }

    public required string Name { get; set; }

    public required string Description { get; set; }

    public ClassificationLevel() { }
}
