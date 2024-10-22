using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;

using Juniper.Data;

using Microsoft.EntityFrameworkCore;

namespace Juniper.Cedrus.Entities;

[DebuggerDisplay($"{{{nameof(Id)}}}: ValidValue [{{{nameof(PropertyTypeName)}}}] = {{{nameof(Value)}}}")]
[Index(nameof(Id), nameof(Value), nameof(PropertyTypeId))]
public class PropertyTypeValidValue : ISequenced
{
    [Key]
    public int Id { get; set; }

    public required string Value { get; set; }

    [ForeignKey(nameof(PropertyType))]
    public int PropertyTypeId { get; set; }

    [AutoIncludeNavigation]
    [DeleteBehavior(DeleteBehavior.Cascade)]
    public required PropertyType PropertyType { get; set; }

    internal string PropertyTypeName => PropertyType.Name;
}