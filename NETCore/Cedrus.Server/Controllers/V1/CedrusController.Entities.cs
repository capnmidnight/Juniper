using Juniper.HTTP;
using Juniper.Units;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using System.Text.Json;

namespace Juniper.Cedrus.Controllers.V1;

public partial class CedrusController
{
    [HttpGet($"entities/{{{nameof(entityId)}:int}}")]
    [HttpHeader("Accept", "application/json")]
    public Task<IActionResult> GetEntityAsync([FromRoute] int entityId) =>
        WithUserAsync(async user =>
        {
            var entity = await db.GetEntityAsync(entityId, user);
            return Json(new EntityModel(entity));
        });

    public record SetEntityInput(NameOrId Type, string Name);
    [HttpPost("entities")]
    [HttpHeader("Accept", "application/json")]
    public Task<IActionResult> SetEntityAync([FromBody] SetEntityInput input) =>
        WithUserAsync(async user =>
        {
            var type = await db.GetEntityTypeAsync(input.Type);
            var entity = await db.SetEntityAsync(type, input.Name, user);
            await db.SaveChangesAsync();
            return Json(new EntityModel(entity));
        });

    [HttpPost("entities/search")]
    [HttpHeader("Accept", "application/json")]
    public Task<IActionResult> GetEntitiesAsync([FromBody] NameOrId[]? searchParams)
    {
        var memo = new Memoizer();
        return WithUserAsync(user =>
        {
            var entityFilters = searchParams.OfTypeStamp("entity");
            var entityTypeFilters = searchParams.OfTypeStamp("entityType");
            var entities = db.FindEntities(user, entityTypeFilters, entityFilters);
            return Json(from entity in entities.AsEnumerable()
                        select entity.Memo(memo, () => new EntityModel(entity, memo)));
        });
    }

    [HttpDelete($"entities/{{{nameof(entityId)}:int}}")]
    public Task<IActionResult> DeleteEntityAsync([FromRoute] int entityId) =>
        WithUserAsync(async user =>
        {
            var entity = await db.GetEntityAsync(entityId, user);
            db.DeleteEntity(entity);
            await db.SaveChangesAsync();
            return Ok();
        });

    record ReviewParts(DateTime ReviewedOn, UserModel ReviewedBy);
    [HttpPost("entities/review")]
    public Task<IActionResult> MarkEntityReview([FromBody] NameOrId entityParam) =>
        WithUserAsync(async user =>
        {
            var entity = await db.GetEntityAsync(entityParam, user);
            db.MarkEntityReviewed(entity, user);
            await db.SaveChangesAsync();
            return Json(new ReviewParts(
                entity.ReviewedOn!.Value,
                new UserModel(entity.ReviewedByUser!)
            ));
        });


    [HttpGet($"references/{{{nameof(entityId)}:int}}")]
    public Task<IActionResult> ProxyPropertyReferenceAsync([FromRoute] int entityId) =>
        WithUserAsync(async (user) =>
        {
            var refProperties = await db.GetProperties(user, entities: [
                new NameOrId("entity", entityId)
            ], propertyTypes: [
                new NameOrId("propertyType", db.ReferenceFilePropertyType.Id),
                new NameOrId("propertyType", db.ReferenceLinkPropertyType.Id)
            ]).ToDictionaryAsync(p => p.TypeId);

            if (refProperties.TryGetValue(db.ReferenceFilePropertyType.Id, out var fileProp))
            {
                if (fileProp.Value is JsonElement json2)
                {
                    var guid = json2.GetString()
                    ?? throw new FileNotFoundException();
                    return await GetFileData(new Guid(guid));
                }
                else
                {
                    throw new FileNotFoundException();
                }
            }

            if (!refProperties.TryGetValue(db.ReferenceLinkPropertyType.Id, out var linkProp))
            {
                throw new InvalidOperationException("Can only proxy file protocol reference paths.");
            }

            if (linkProp.Value is JsonElement json)
            {
                var path = json.GetString()
                ?? throw new FileNotFoundException();

                var uri = new Uri(path);
                var file = new FileInfo(uri.LocalPath);
                if (!file.Exists)
                {
                    throw new FileNotFoundException("Could not find file", path);
                }

                return new StreamFileResult(
                    file,
                    null,
                    (int)Days.Seconds(1),
                    Request.Headers.Range.FirstOrDefault(),
                    Logger);
            }

            throw new FileNotFoundException();
        });
}
