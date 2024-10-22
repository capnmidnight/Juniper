using System.ComponentModel.DataAnnotations;

using Juniper.Data;

using Microsoft.EntityFrameworkCore;

namespace Juniper.Cedrus.Entities;

[Index(nameof(FileAssetId), nameof(SearchableText))]
public class FileAssetSearchIndex
{
    [Key]
    public int FileAssetId { get; set; }

    [AutoIncludeNavigation]
    [DeleteBehavior(DeleteBehavior.Cascade)]
    public required FileAsset File { get; set; }

    public required string SearchableText { get; set; }
}