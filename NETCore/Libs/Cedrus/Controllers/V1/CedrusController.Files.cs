using Juniper.Cedrus.Entities;
using Juniper.HTTP;
using Juniper.Units;

using Microsoft.AspNetCore.Mvc;

namespace Juniper.Cedrus.Controllers.V1;

public partial class CedrusController
{
    [HttpGet("files")]
    public Task<IActionResult> GetFilesAsync() =>
        WithUserAsync(user =>
            Json(from f in db.GetFiles(user)
                 select new FileModel(f)));

    private Task<IActionResult> WithFileAsync(Guid guid, Func<FileAsset, IActionResult> act) =>
        WithUserAsync(async user =>
        {
            var file = await db.GetFileAsync(guid, user);
            return act(file);
        });

    [HttpHead($"files/{{{nameof(guid)}}}")]
    public Task<IActionResult> GetFileInfoAsync([FromRoute] Guid guid) =>
        WithFileAsync(guid, file =>
            new FileInfoResult(
                file.Length,
                file.MediaType,
                file.Guid.ToString(),
                file.Name,
                (int)Days.Seconds(1),
                Request.Headers.Range.FirstOrDefault(),
                logger
            )
        );

    [HttpGet($"files/{{{nameof(guid)}}}")]
    public Task<IActionResult> GetFileData([FromRoute] Guid guid) =>
        WithFileAsync(guid, file =>
            new DbFileResult(
                db.Database,
                file.Length,
                file.MediaType,
                file.Guid.ToString(),
                file.Name,
                (int)Days.Seconds(1),
                Request.Headers.Range.FirstOrDefault(),
                logger,
                file.ConstructDataQuery
            )
        );
}
