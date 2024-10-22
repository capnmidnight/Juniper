using Juniper.Cedrus.Adapters;
using Juniper.Cedrus.Entities;
using Juniper.HTTP;
using Juniper.Units;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Juniper.Cedrus.Controllers.V1;

public partial class CedrusController
{
    [HttpGet("files")]
    [HttpHeader("Accept", "application/json")]
    public Task<IActionResult> GetFilesAsync()
    {
        var memo = new Memoizer();
        return WithUserAsync(user =>
            Json(from f in db.GetFiles(user).AsEnumerable()
                 select f.Memo(memo, () => new FileModel(f, memo))));
    }

    public record UploadFilesInput(IFormFile[] Files);
    [HttpPost("files")]
    [HttpHeader("Accept", "application/json")]
    public Task<IActionResult> SaveFilesAsync([FromForm] UploadFilesInput input) =>
        WithUserAsync(async user =>
        {
            var files = new List<FileAsset>();
            foreach (var f in input.Files)
            {
                files.Add(await db.UpsertFileAsync(f, user));
            }
            await db.SaveChangesAsync();

            return Json(from f in files
                        select new FileModel(f));
        });

    private Task<IActionResult> WithFileAsync(Guid guid, Func<FileAsset, IActionResult> act) =>
        WithUserAsync(async user =>
        {
            var file = await db.GetFileByGuidAsync(guid, user);
            return act(file);
        });

    private Task<IActionResult> WithFileAsync(Guid guid, Func<FileAsset, Task<IActionResult>> act) =>
        WithUserAsync(async user =>
        {
            var file = await db.GetFileByGuidAsync(guid, user);
            return await act(file);
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
                Logger
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
                Logger,
                file.ConstructDataQuery
            )
        );

    [HttpDelete($"files/{{{nameof(guid)}}}")]
    public Task<IActionResult> DeleteFileData([FromRoute] Guid guid) =>
        WithFileAsync(guid, async file =>
        {
            db.DeleteFile(file);
            await db.SaveChangesAsync();
            return Ok();
        });
}
