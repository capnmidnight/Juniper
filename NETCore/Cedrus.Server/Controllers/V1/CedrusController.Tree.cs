using Microsoft.AspNetCore.Mvc;

namespace Juniper.Cedrus.Controllers.V1;

public partial class CedrusController
{

    [HttpPost("tree")]
    [HttpHeader("Accept", "application/json")]
    public Task<IActionResult> GetTree([FromBody] NameOrId[]? searchParams) =>
        WithUserAsync(async user =>
        {
            var entityTypes = searchParams.OfTypeStamp("entityType");
            var propertyTypes = searchParams.OfTypeStamp("propertyType");
            return Json(await db.GetTreeAsync(user, entityTypes, propertyTypes));
        });
}
