using Microsoft.AspNetCore.Mvc;

namespace Juniper.Cedrus.Controllers.V1;

public partial class CedrusController
{
    [HttpGet("tags")]
    public Task<IActionResult> GetTagsAsync() =>
        WithErrorHandlingAsync(() =>
            Json(db.GetTagsAsync()));

    [HttpGet($"entities/{{{nameof(entityId)}:int}}/tags")]
    public Task<IActionResult> GetTagsOnEntity([FromRoute] int entityId) =>
        WithUserAsync(async (user) =>
        {
            var entity = await db.GetEntityAsync(entityId, user);
            return await Json(db.GetTagsOnEntityAsync(entity));
        });

    [HttpPost($"entities/{{{nameof(entityId)}:int}}/tags")]
    public Task<IActionResult> TagEntityAsync([FromRoute] int entityId, [FromBody] SetTagInput input) =>
        WithUserAsync(async (user) =>
        {
            var entity = await db.GetEntityAsync(entityId, user);
            var tag = db.TagEntity(entity, input.Name, input.Description, user);
            await db.SaveChangesAsync();
            return Json(new EntityModel(tag));
        });
}
