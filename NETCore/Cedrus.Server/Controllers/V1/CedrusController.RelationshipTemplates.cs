using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Juniper.Cedrus.Controllers.V1;

public partial class CedrusController
{
    [HttpGet("templates/relationships")]
    [HttpGet($"entities/types/{{{nameof(entityTypeId)}:int}}/templates/relationships")]
    [HttpHeader("Accept", "application/json")]
    public Task<IActionResult> GetRelationshipTemplatesAsync([FromRoute] int? entityTypeId)
    {
        var memo = new Memoizer();
        return WithErrorHandlingAsync(async () =>
            Json(from temp in (await db.GetRelationshipTemplatesAsync(entityTypeId)).AsEnumerable()
                 select temp.Memo(memo, () => new RelationshipTemplateModel(temp, memo))));
    }

    [HttpDelete($"templates/relationships/{{{nameof(templateId)}:int}}")]
    public Task<IActionResult> DeleteRelationshipTemplateAsync([FromRoute] int templateId) =>
        WithErrorHandlingAsync(async () =>
        {
            var template = await db.GetRelationshipTemplateAsync(templateId);
            db.DeleteRelationshipTemplate(template);
            await db.SaveChangesAsync();
            return Ok();
        });

    public record SetRelationshipTemplateInput(
        string Name,
        NameOrId RelationshipType,
        NameOrId? PropertyEntityType,
        NameOrId[]? AllowedEntityTypes
    );
    [HttpPost($"entities/types/{{{nameof(entityTypeId)}:int}}/templates/relationships")]
    [HttpHeader("Accept", "application/json")]
    public Task<IActionResult> SetRelationshipTemplatesAsync([FromRoute] int entityTypeId, [FromBody] SetRelationshipTemplateInput input) =>
        WithErrorHandlingAsync(async () =>
        {
            var entityType = await db.GetEntityTypeAsync(new NameOrId("entityType", Id: entityTypeId));
            var relationshipType = await db.GetRelationshipTypeAsync(input.RelationshipType);
            var propertyEntityType = input.PropertyEntityType is null ? null : await db.GetEntityTypeAsync(input.PropertyEntityType);
            var allowedEntityTypes = await db.FindEntityTypes(input.AllowedEntityTypes).ToArrayAsync();
            var template = await db.SetRelationshipTemplateAsync(
                entityType,
                input.Name,
                propertyEntityType,
                relationshipType,
                allowedEntityTypes
            );
            await db.SaveChangesAsync();
            return Json(new RelationshipTemplateModel(template));
        });
}
