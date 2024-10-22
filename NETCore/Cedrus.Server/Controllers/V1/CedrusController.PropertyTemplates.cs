using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Juniper.Cedrus.Controllers.V1;

public partial class CedrusController
{
    [HttpGet("templates/properties")]
    [HttpGet($"entities/types/{{{nameof(entityTypeId)}:int}}/templates/properties")]
    [HttpHeader("Accept", "application/json")]
    public Task<IActionResult> GetPropertyTemplatesAsync([FromRoute] int? entityTypeId)
    {
        var memo = new Memoizer();
        return WithErrorHandlingAsync(async () =>
            Json(from temp in (await db.GetPropertyTemplatesAsync(entityTypeId)).AsEnumerable()
                 select temp.Memo(memo, () => new PropertyTemplateModel(temp, memo))));
    }

    [HttpDelete($"templates/properties/{{{nameof(templateId)}:int}}")]
    public Task<IActionResult> DeletePropertyTemplateAsync([FromRoute] int templateId) =>
        WithErrorHandlingAsync(async () =>
        {
            var template = await db.GetPropertyTemplateAsync(templateId);
            db.DeletePropertyTemplate(template);
            await db.SaveChangesAsync();
            return Ok();
        });

    public record SetPropertyTemplateInput(string Name, NameOrId[]? PropertyTypes);
    [HttpPost($"entities/types/{{{nameof(entityTypeId)}:int}}/templates/properties")]
    [HttpHeader("Accept", "application/json")]
    public Task<IActionResult> SetPropertyTemplatesAsync([FromRoute] int entityTypeId, [FromBody] SetPropertyTemplateInput input) =>
        WithErrorHandlingAsync(async () =>
        {
            var entityType = await db.GetEntityTypeAsync(new NameOrId("entityType", Id: entityTypeId));
            var propertyTypes = await db.FindPropertyTypes(input.PropertyTypes).ToArrayAsync();
            var template = await db.SetPropertyTemplateAsync(entityType, input.Name, propertyTypes);
            await db.SaveChangesAsync();
            return Json(new PropertyTemplateModel(template));
        });
}
