using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;

using Juniper.Data;
using Juniper.Units;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Juniper.Cedrus.Entities;

[DebuggerDisplay($"{{{nameof(Id)}}}: Property [{{{nameof(TypeName)}}}] = {{{nameof(Value)}}}")]
[Index(nameof(Id), nameof(TypeId), nameof(EntityId), nameof(Start), nameof(End), nameof(Value))]
public class Property : ITimeSeries<PropertyType>, IClassificationMarked, ICreationTracked
{
    [Key]
    public int Id { get; set; }

    [ForeignKey(nameof(PropertyType))]
    public int TypeId { get; set; }

    [AutoIncludeNavigation]
    [DeleteBehavior(DeleteBehavior.Cascade)]
    public required PropertyType Type { get; set; }

    [ForeignKey(nameof(Entity))]
    public int EntityId { get; set; }

    [AutoIncludeNavigation]
    [DeleteBehavior(DeleteBehavior.Cascade)]
    public required Entity Entity { get; set; }

    [ForeignKey(nameof(Classification))]
    public int ClassificationId { get; set; }

    [AutoIncludeNavigation]
    [DeleteBehavior(DeleteBehavior.Cascade)]
    public required Classification Classification { get; set; }

    [JsonConversion]
    public required object Value { get; set; }

    [ForeignKey(nameof(ReferenceEntity))]
    public int? ReferenceEntityId { get; set; }

    [AutoIncludeNavigation]
    [DeleteBehavior(DeleteBehavior.Cascade)]
    public Entity? ReferenceEntity { get; set; }

    [SqlConversion<EnumToStringConverter<UnitOfMeasure>>]
    public required UnitOfMeasure Units { get; set; }

    public required DateTime Start { get; set; }

    public DateTime End { get; set; } = DateTime.MaxValue;

    [ForeignKey(nameof(User))]
    public int UserId { get; set; }

    [AutoIncludeNavigation]
    [DeleteBehavior(DeleteBehavior.NoAction)]
    public required CedrusUser User { get; set; }

    public DateTime CreatedOn { get; set; } = DateTime.Now;

    internal string TypeName => Type.Name;
}