using Microsoft.AspNetCore.Mvc;

namespace Juniper.Cedrus.Controllers.V1;

public partial class CedrusController
{
    [HttpGet("properties")]
    public Task<IActionResult> GetPropertiesAsync() =>
        WithUserAsync(user =>
            Json(from v in db.GetProperties(user)
                 select new PropertyModel(v)));


    [HttpDelete($"properties/{{{nameof(propertyId)}:int}}")]
    public Task<IActionResult> DeletePropertyAsync([FromRoute] int propertyId) =>
        WithUserAsync(async (user) =>
        {
            var p = await db.GetPropertyAsync(propertyId, user);
            db.EndProperty(p);
            await db.SaveChangesAsync();
            return Ok();
        });

    [HttpGet($"entities/{{{nameof(entityId)}:int}}/properties")]
    public Task<IActionResult> GetPropertiesAsync([FromRoute] int entityId) =>
        WithUserAsync(user =>
            Json(from v in db.GetProperties(user)
                 where v.EntityId == entityId
                 select new PropertyModel(v)));


    [HttpPost($"entities/{{{nameof(entityId)}:int}}/properties")]
    public Task<IActionResult> SetPropertyAsync([FromRoute] int entityId, [FromBody] SetPropertyInput input) =>
        WithUserAsync(async user =>
        {
            var type = await db.GetPropertyTypeAsync(input.Type);
            var entity = await db.GetEntityAsync(entityId, user);
            var classification = await db.GetClassificationAsync(input.Classification, user);
            var prop = db.SetProperty(type, entity, input.Value, input.UnitOfMeasure ?? Units.UnitOfMeasure.None, user, classification, input.Start);
            await db.SaveChangesAsync();
            return Json(new PropertyModel(prop));
        });
}
