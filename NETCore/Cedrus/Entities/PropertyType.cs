using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

using Juniper.Data;
using Juniper.Units;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Juniper.Cedrus.Entities;

[DebuggerDisplay($"{{{nameof(Id)}}}: PropertyType = {{{nameof(Name)}}}")]
[AlternateKey(nameof(Name))]
[Index(nameof(Id), nameof(Name))]
public class PropertyType : INamed
{
    [Key]
    public int Id { get; set; }

    public required string Name { get; set; }

    [SqlConversion<EnumToStringConverter<DataType>>]
    public required DataType Type { get; set; }

    [SqlConversion<EnumToStringConverter<StorageType>>]
    public required StorageType Storage { get; set; }

    [SqlConversion<EnumToStringConverter<UnitsCategory>>]
    public required UnitsCategory UnitsCategory { get; set; }

    public required string Description { get; set; }

    public ICollection<Property> Properties { get; set; } = new HashSet<Property>();

    public ICollection<PropertyTypeValidValue> ValidValues { get; set; } = new HashSet<PropertyTypeValidValue>();

    public ICollection<PropertyTemplate> Templates { get; set; } = new HashSet<PropertyTemplate>();
}