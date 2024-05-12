using Microsoft.AspNetCore.Mvc;

namespace Juniper.Cedrus.Controllers.V1;

public partial class CedrusController
{
    [HttpGet("classifications")]
    public Task<IActionResult> GetClassificationsAsync() =>
        WithUserAsync(user =>
            Json(from c in db.GetClassifications(user)
                 select new ClassificationModel(c)));

    [HttpPost("classifications")]
    public Task<IActionResult> SetClassificationAsync(SetClassificationInput input) =>
        WithUserAsync(async user =>
        {
            var level = await db.GetClassificationLevelAsync(input.Level, user);
            var caveats = await db.GetClassificationCaveatsAsync(input.Caveats, user);
            var classification = await db.SetClassificationAsync(level, caveats);
            await db.SaveChangesAsync();
            return Json(new ClassificationModel(classification));
        });

    [HttpDelete($"classifications/{{{nameof(classId)}:int}}")]
    public Task<IActionResult> DeleteClassificationAsync([FromRoute] int classId) =>
        WithUserAsync(async user =>
        {
            var classification = await db.GetClassificationAsync(classId, user);
            db.DeleteClassification(classification);
            await db.SaveChangesAsync();
            return Ok();
        });
}
