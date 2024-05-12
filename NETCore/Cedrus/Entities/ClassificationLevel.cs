using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Runtime.Serialization;

using Juniper.Data;

using Microsoft.EntityFrameworkCore;

namespace Juniper.Cedrus.Entities;

[DebuggerDisplay($"{{{nameof(Id)}}}: Level = {{{nameof(Name)}}}")]
[AlternateKey(nameof(Name))]
[Index(nameof(Id), nameof(Name), nameof(Description))]
public class ClassificationLevel : INamed, ISerializable
{
    [Key]
    public int Id { get; set; }

    public required string Name { get; set; }

    public required string Description { get; set; }

    public ClassificationLevel() { }

    public ClassificationLevel(SerializationInfo info, StreamingContext context)
    {
        Id = info.GetInt32(nameof(Id));
        Name = info.GetValue<string>(nameof(Name)) ?? "Unknown";
        Description = info.GetValue<string>(nameof(Description)) ?? "Unknown";
    }

    public void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        info.AddValue(nameof(Id), Id);
        info.AddValue(nameof(Name), Name);
        info.AddValue(nameof(Description), Description);
    }
}
