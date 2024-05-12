using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;

using Juniper.Data;

using Microsoft.EntityFrameworkCore;

namespace Juniper.Cedrus.Entities;

[DebuggerDisplay($"{{{nameof(Id)}}}: Classification = {{{nameof(Name)}}}")]
[Index(nameof(Id), nameof(LevelId), nameof(ParentId))]
public class Classification : ISequenced, IParentChained<Classification>
{
    public static void Configure(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Classification>(e =>
        {
            e.HasMany(p => p.Caveats).WithMany(d => d.Classifications);
            e.HasOne(p => p.Parent).WithMany(d => d.Children);
        });
    }
    public int Id { get; set; }

    [ForeignKey(nameof(Classification))]
    public int LevelId { get; set; }

    [AutoIncludeNavigation]
    [DeleteBehavior(DeleteBehavior.Cascade)]
    public required ClassificationLevel Level { get; set; }

    [ForeignKey(nameof(Parent))]
    public int? ParentId { get; set; }

    [DeleteBehavior(DeleteBehavior.SetNull)]
    public Classification? Parent { get; set; }

    public ICollection<Classification> Children { get; set; } = new HashSet<Classification>();

    [AutoIncludeNavigation]
    public ICollection<ClassificationCaveat> Caveats { get; set; } = new HashSet<ClassificationCaveat>();

    public string CaveatsString => 
        Caveats
            .OrderBy(cv => cv.Name)
            .Select(c => c.Name)
            .ToArray()
            .Join("/");

    public string CaveatsDescriptionString => 
        Caveats
            .OrderBy(cv => cv.Name)
            .Select(c => c.Description)
            .ToArray()
            .Join(", ");

    public string Name => 
        Caveats
            .Select(c => c.Name)
            .Order()
            .Prepend(Level.Name)
            .ToArray()
            .Join("/");

    public string Description => 
        Caveats.Count == 0
            ? Level.Description
            : $"{Level.Description}: {CaveatsString}";
}