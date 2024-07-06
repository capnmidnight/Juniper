using Microsoft.AspNetCore.Mvc;

namespace Juniper.Cedrus.Controllers.V1;

public partial class CedrusController
{
    [HttpGet("classifications/caveats")]
    public Task<IActionResult> GetClassificationCaveatsAsync() =>
        WithUserAsync(user =>
            Json(from c in db.GetClassificationCaveats(user)
                 select new ClassificationCaveatModel(c)));

    [HttpPost("classifications/caveats")]
    public Task<IActionResult> CreateClassificationCaveatsAsync([FromBody] SetClassificationCaveatInput input) =>
        WithUserAsync(async user =>
        {
            var level = await db.GetClassificationLevelAsync(input.ClassificationLevel, user);
            var caveat = db.SetClassificationCaveat(level, input.Name, input.Description);
            await db.SaveChangesAsync();
            return Json(new ClassificationCaveatModel(caveat));
        });

    [HttpDelete($"classifications/caveats/{{{nameof(classCaveatId)}:int}}")]
    public Task<IActionResult> DeleteClassificationCaveatAsync([FromRoute] int classCaveatId) =>
        WithUserAsync(async user =>
        {
            var classCaveat = await db.GetClassificationCaveatAsync(classCaveatId, user);
            db.DeleteClassificationCaveat(classCaveat);
            await db.SaveChangesAsync();
            return Ok();
        });
}
