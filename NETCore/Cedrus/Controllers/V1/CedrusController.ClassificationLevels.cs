using Microsoft.AspNetCore.Mvc;

namespace Juniper.Cedrus.Controllers.V1;

public partial class CedrusController
{
    [HttpGet("classifications/levels")]
    public Task<IActionResult> GetClassificationLevelsAsync() =>
        WithUserAsync(user =>
            Json(from level in db.GetClassificationLevels(user)
                 select new ClassificationLevelModel(level)));
}
