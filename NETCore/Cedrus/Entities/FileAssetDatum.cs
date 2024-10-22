using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Juniper.Data;

using Microsoft.EntityFrameworkCore;

namespace Juniper.Cedrus.Entities;

[Table("FileAssetData")]
public class FileAssetDatum
{
    [Key]
    [ForeignKey(nameof(FileAsset))]
    public int FileAssetId { get; set; }

    [AutoIncludeNavigation]
    [DeleteBehavior(DeleteBehavior.Cascade)]
    public required FileAsset FileAsset { get; set; }

    public required byte[] Blob { get; set; }
}