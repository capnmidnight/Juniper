using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;

using Juniper.Data;
using Juniper.Units;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Juniper.Cedrus.Entities;

[DebuggerDisplay($"{{{nameof(Id)}}}: Property [{{{nameof(TypeName)}}}] = {{{nameof(Value)}}}")]
[Index(nameof(Id), nameof(TypeId), nameof(EntityId), nameof(Value))]
public class Property : ICreationTracked, ISequenced
{
    public static void Configure(ModelBuilder modelBuilder) =>
        modelBuilder.Entity<Property>(e =>
        {
            e.HasOne(p => p.User).WithMany(d => d.PropertiesCreated);
            e.HasOne(p => p.UpdatedByUser).WithMany(d => d.PropertiesUpdated);
            e.HasOne(p => p.Entity).WithMany(d => d.Properties);
            e.HasOne(p => p.Reference).WithMany(d => d.ReferenceFor);
        });

    [Key]
    public int Id { get; set; }

    [ForeignKey(nameof(Type))]
    public int TypeId { get; set; }

    [AutoIncludeNavigation]
    [DeleteBehavior(DeleteBehavior.Cascade)]
    public required PropertyType Type { get; set; }

    [ForeignKey(nameof(Entity))]
    public int EntityId { get; set; }

    [AutoIncludeNavigation]
    [DeleteBehavior(DeleteBehavior.Cascade)]
    public required Entity Entity { get; set; }

    [JsonConversion]
    public required object Value { get; set; }

    [ForeignKey(nameof(Reference))]
    public int? ReferenceEntityId { get; set; }

    [AutoIncludeNavigation]
    [DeleteBehavior(DeleteBehavior.SetNull)]
    public Entity? Reference { get; set; }

    [SqlConversion<EnumToStringConverter<UnitOfMeasure>>]
    public required UnitOfMeasure Units { get; set; }

    [ForeignKey(nameof(User))]
    public int UserId { get; set; }

    [AutoIncludeNavigation]
    [DeleteBehavior(DeleteBehavior.NoAction)]
    public required CedrusUser User { get; set; }

    public DateTime CreatedOn { get; set; } = DateTime.Now;

    public DateTime? UpdatedOn { get; set; } = DateTime.Now;

    [ForeignKey(nameof(UpdatedByUser))]
    public int? UpdatedBy { get; set; }

    [AutoIncludeNavigation]
    [DeleteBehavior(DeleteBehavior.NoAction)]
    public CedrusUser? UpdatedByUser { get; set; }

    internal string TypeName => Type.Name;
}