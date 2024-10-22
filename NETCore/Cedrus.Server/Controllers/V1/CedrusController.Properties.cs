using System.Data;
using System.Text.Json;
using Juniper.Units;

using Microsoft.AspNetCore.Mvc;

namespace Juniper.Cedrus.Controllers.V1;

public partial class CedrusController
{
    [HttpPost($"properties/search")]
    [HttpHeader("Accept", "application/json")]
    public Task<IActionResult> GetPropertiesAsync([FromBody] NameOrId[]? searchParams)
    {
        var memo = new Memoizer();
        return WithUserAsync(user =>
        {
            var entityTypes = searchParams.OfTypeStamp("entityType");
            var entities = searchParams.OfTypeStamp("entity");
            var propertyTypes = searchParams.OfTypeStamp("propertyType");
            return Json(from p in db.GetProperties(user, entityTypes, entities, propertyTypes).AsEnumerable()
                        select p.Memo(memo, () => new PropertyModel(p, memo)));
        });
    }

    [HttpDelete($"properties/{{{nameof(propertyId)}:int}}")]
    public Task<IActionResult> DeletePropertyAsync([FromRoute] int propertyId) =>
        WithUserAsync(async (user) =>
        {
            var p = await db.GetPropertyAsync(propertyId, user);
            db.DeleteProperty(p);
            await db.SaveChangesAsync();
            return Ok();
        });


    public record SetPropertyInput(
        NameOrId Type,
        JsonElement Value,
        UnitOfMeasure? UnitOfMeasure,
        NameOrId? Reference
    );
    [HttpPost($"entities/{{{nameof(entityId)}:int}}/properties")]
    [HttpHeader("Accept", "application/json")]
    public Task<IActionResult> SetPropertyAsync([FromRoute] int entityId, [FromBody] SetPropertyInput input)
    {
        var memo = new Memoizer();
        return WithUserAsync(async user =>
        {
            var type = await db.GetPropertyTypeAsync(input.Type);
            var entity = await db.GetEntityAsync(entityId, user);
            var reference = await db.FindEntityAsync(user, input.Reference);
            var prop = await db.SetPropertyAsync(type!, entity, input.Value, input.UnitOfMeasure ?? Units.UnitOfMeasure.None, user, reference);
            await db.SaveChangesAsync();
            return Json(prop.Memo(memo, () => new PropertyModel(prop, memo)));
        });
    }
}
