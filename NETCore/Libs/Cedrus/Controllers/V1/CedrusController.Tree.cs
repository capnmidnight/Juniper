using Microsoft.AspNetCore.Mvc;

namespace Juniper.Cedrus.Controllers.V1;

public partial class CedrusController
{

    [HttpGet("tree")]
    public Task<IActionResult> GetTree([FromQuery] string? entityTypes, [FromQuery] string? relTypes) =>
        WithUserAsync(async user =>
        {
            var ets = await db.FindEntityTypesAsync(entityTypes);
            var rts = await db.FindRelationshipTypesAsync(relTypes);
            return Json(db.GetTree(ets, rts, user));
        });
}
