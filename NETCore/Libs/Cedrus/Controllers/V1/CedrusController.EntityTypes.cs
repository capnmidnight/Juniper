using Microsoft.AspNetCore.Mvc;

namespace Juniper.Cedrus.Controllers.V1;

public partial class CedrusController
{
    [HttpGet("entities/types")]
    public Task<IActionResult> GetEntityTypesAsync() =>
        WithErrorHandlingAsync(() =>
            Json(from e in db.EntityTypes
                 select new EntityTypeModel(e)));

    [HttpPost("entities/types")]
    public Task<IActionResult> SetEntityTypeAsync([FromBody] string name) =>
        WithErrorHandlingAsync(async () =>
        {
            if (name is null)
            {
                return BadRequest();
            }

            var et = db.SetEntityType(name);
            await db.SaveChangesAsync();
            return Json(new EntityTypeModel(et));
        });

    [HttpDelete($"entities/types/{{{nameof(entityTypeId)}:int}}")]
    public Task<IActionResult> DeleteEntityTypeAsync([FromRoute] int entityTypeId) =>
        WithErrorHandlingAsync(async () =>
        {
            var entityType = await db.GetEntityTypeAsync(entityTypeId);
            db.DeleteEntityType(entityType);
            await db.SaveChangesAsync();
            return Ok();
        });
}
