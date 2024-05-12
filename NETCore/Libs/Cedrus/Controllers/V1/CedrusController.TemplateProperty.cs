using Microsoft.AspNetCore.Mvc;

namespace Juniper.Cedrus.Controllers.V1;

public partial class CedrusController
{
    [HttpGet("templates/properties")]
    public IActionResult GetTemplateProperties() =>
        WithErrorHandling(() =>
            Json(from kv in db.GetTemplateProperties()
                 select new TemplatePropertyModel(kv.Key, kv.Value)));

    [HttpDelete($"templates/{{{nameof(templateId)}:int}}/properties/{{{nameof(propertyTypeId)}:int}}")]
    public Task<IActionResult> DeleteTemplatePropertyAsync([FromRoute] int templateId, [FromRoute] int propertyTypeId) =>
        WithErrorHandlingAsync(async () =>
        {
            var template = await db.GetTemplateAsync(templateId);
            var property = template.PropertyTypes.SingleOrDefault(p => p.Id == propertyTypeId)
                ?? throw new FileNotFoundException();
            db.RemovePropertyFromTemplate(template, property);
            await db.SaveChangesAsync();
            return Ok();
        });

    [HttpGet($"entities/types/{{{nameof(entityTypeId)}:int}}/templates/properties")]
    public IActionResult GetEntityTypeTemplateProperties([FromRoute] int entityTypeId) =>
        WithErrorHandling(() => 
            Json(from kv in db.GetTemplateProperties(entityTypeId)
                 select new TemplatePropertyModel(kv.Key, kv.Value)));
}
