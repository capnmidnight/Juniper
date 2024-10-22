using Microsoft.AspNetCore.Mvc;

namespace Juniper.Cedrus.Controllers.V1;

public partial class CedrusController
{
    [HttpPost("entities/types/search")]
    [HttpHeader("Accept", "application/json")]
    public IActionResult GetEntityTypes([FromBody] NameOrId[]? searchParams)
    {
        var memo = new Memoizer();
        return WithErrorHandling(() =>
            Json(from e in db.FindEntityTypes(searchParams).AsEnumerable()
                 select e.Memo(memo, () => new EntityTypeModel(e, memo))));
    }

    public record SetEntityTypeInput(string Name, bool IsPrimary, NameOrId? ParentEntityType);
    [HttpPost("entities/types")]
    [HttpHeader("Accept", "application/json")]
    public Task<IActionResult> SetEntityTypeAsync([FromBody] SetEntityTypeInput input)
    {
        var memo = new Memoizer();
        return WithErrorHandlingAsync(async () =>
        {
            var parentET = input.ParentEntityType is null
            ? null
            : await db.GetEntityTypeAsync(input.ParentEntityType);
            var newET = await db.SetEntityTypeAsync(input.Name, input.IsPrimary, parentET);
            await db.SaveChangesAsync();
            return Json(newET.Memo(memo, () => new EntityTypeModel(newET, memo)));
        });
    }

    [HttpDelete($"entities/types/{{{nameof(entityTypeId)}:int}}")]
    public Task<IActionResult> DeleteEntityTypeAsync([FromRoute] int entityTypeId) =>
        WithErrorHandlingAsync(async () =>
        {
            var entityType = await db.GetEntityTypeAsync(new NameOrId("entityType", Id: entityTypeId));
            db.DeleteEntityType(entityType);
            await db.SaveChangesAsync();
            return Ok();
        });
}
