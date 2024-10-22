using Microsoft.AspNetCore.Mvc;

namespace Juniper.Cedrus.Controllers.V1;

public partial class CedrusController
{
    [HttpGet("tags")]
    [HttpHeader("Accept", "application/json")]
    public Task<IActionResult> GetTagsAsync() =>
        WithErrorHandlingAsync(() =>
            Json(db.GetTagsAsync()));

    [HttpGet($"entities/{{{nameof(entityId)}:int}}/tags")]
    [HttpHeader("Accept", "application/json")]
    public Task<IActionResult> GetTagsOnEntity([FromRoute] int entityId) =>
        WithUserAsync(async (user) =>
        {
            var entity = await db.GetEntityAsync(entityId, user);
            return await Json(db.GetTagsOnEntityAsync(entity));
        });

    [HttpPost($"entities/{{{nameof(entityId)}:int}}/tags")]
    [HttpHeader("Accept", "application/json")]
    public Task<IActionResult> TagEntityAsync([FromRoute] int entityId, [FromBody] string tagName)
    {
        var memo = new Memoizer();
        return WithUserAsync(async (user) =>
        {
            var entity = await db.GetEntityAsync(entityId, user);
            var tag = await db.TagEntityAsync(entity, tagName, user);
            await db.SaveChangesAsync();
            return Json(tag.Memo(memo, () => new EntityModel(tag, memo)));
        });
    }
}
