using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Common;
using System.Diagnostics;

using Juniper.Data;

using Microsoft.EntityFrameworkCore;

namespace Juniper.Cedrus.Entities;

/// <summary>
/// Storing files in the database allows us to move the database
/// file around without having to worry about dragging along any
/// other files.
/// </summary>
[DebuggerDisplay($"{{{nameof(Id)}}}: File = {{{nameof(Name)}}} ({{{nameof(MediaType)}}})")]
[Index(nameof(Id), nameof(Name), nameof(Length), nameof(MediaType))]
public class FileAsset : INamed, ICreationTracked
{
    public int Id { get; set; }

    public Guid Guid { get; set; } = Guid.NewGuid();

    public required string Name { get; set; }

    public required long Length { get; set; }

    public required MediaType MediaType { get; set; }

    [ForeignKey(nameof(User))]
    public int UserId { get; set; }

    [AutoIncludeNavigation]
    [DeleteBehavior(DeleteBehavior.NoAction)]
    public required CedrusUser User { get; set; }

    public DateTime CreatedOn { get; set; } = DateTime.Now;

    public FileAssetDatum? Datum { get; set; }

    public FileAssetSearchIndex? SearchIndex { get; set; }

    public string LinkPath => $"/api/cedrus/v1/files/{Guid}";

    public void ConstructDataQuery(DbCommand cmd)
    {
        var idParam = cmd.CreateParameter();
        idParam.Value = Id;
        idParam.ParameterName = nameof(Id);
        idParam.DbType = System.Data.DbType.Int32;
        cmd.Parameters.Add(idParam);
        cmd.CommandText = $"SELECT \"Blob\" FROM \"FileAssetData\" WHERE \"FileAssetId\" = @{nameof(Id)}";
    }
}