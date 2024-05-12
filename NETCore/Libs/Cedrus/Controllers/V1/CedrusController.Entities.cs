using Microsoft.AspNetCore.Mvc;

namespace Juniper.Cedrus.Controllers.V1;

public partial class CedrusController
{
    [HttpGet("entities/full")]
    public Task<IActionResult> GetEntitiesAsync() =>
        WithUserAsync(user =>
            Json(from e in db.GetEntities(user)
                 select new EntityModel(e)));

    [HttpGet("entities")]
    public Task<IActionResult> GetEntitiesOnlyAsync() =>
        WithUserAsync(user =>
            Json(from e in db.GetEntitiesOnly(user)
                 select new EntityModel(e)));

    [HttpGet($"entities/{{{nameof(entityId)}:int}}")]
    public Task<IActionResult> GetEntityAsync([FromRoute] int entityId) =>
        WithUserAsync(async user =>
        {
            var entity = await db.GetEntityAsync(entityId, user);
            return Json(new EntityModel(entity));
        });

    [HttpPost("entities")]
    public Task<IActionResult> SetEntityAync([FromBody] SetEntityInput input) =>
        WithUserAsync(async user =>
        {
            var type = await db.GetEntityTypeAsync(input.Type);
            var classification = await db.GetClassificationAsync(input.Classification, user);
            var entity = db.SetEntity(type, input.Name, user, classification);
            await db.SaveChangesAsync();
            return Json(new EntityModel(entity));
        });

    [HttpPost("entities/search")]
    public Task<IActionResult> FindEntityAync([FromBody] FindEntityInput input) =>
        WithUserAsync(async user =>
        {
            var type = await db.GetEntityTypeAsync(input.Type);
            var entity = await db.GetEntityAsync(type, input.Name, user);
            return Json(new EntityModel(entity));
        });

    [HttpDelete($"entities/{{{nameof(entityId)}:int}}")]
    public Task<IActionResult> DeleteEntityAsync([FromRoute] int entityId) =>
        WithUserAsync(async user =>
        {
            var entity = await db.GetEntityAsync(entityId, user);
            db.DeleteEntity(entity);
            await db.SaveChangesAsync();
            return Ok();
        });
}
