using Microsoft.AspNetCore.Mvc;

namespace Juniper.Cedrus.Controllers.V1;

public partial class CedrusController
{
    [HttpGet("templates")]
    public Task<IActionResult> GetTemplatesAsync() =>
        WithErrorHandlingAsync(() =>
            Json(from temp in db.GetTemplates()
                 select new TemplateModel(temp)));

    [HttpGet($"templates/{{{nameof(templateId)}:int}}")]
    public Task<IActionResult> DeleteTemplateAsync([FromRoute] int templateId) =>
        WithErrorHandlingAsync(async () =>
        {
            var template = await db.GetTemplateAsync(templateId);
            db.DeleteTemplate(template);
            await db.SaveChangesAsync();
            return Ok();
        });

    [HttpGet($"entities/types/{{{nameof(entityTypeId)}:int}}/templates")]
    public Task<IActionResult> GetTemplatesAsync([FromRoute] int entityTypeId) =>
        WithErrorHandlingAsync(() =>
            Json(from temp in db.GetTemplates(entityTypeId)
                 select new TemplateModel(temp)));
}
