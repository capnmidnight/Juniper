using Microsoft.AspNetCore.Mvc;

namespace Juniper.Cedrus.Controllers.V1;

public partial class CedrusController
{
    [HttpGet("relationships/types")]
    [HttpHeader("Accept", "application/json")]
    public Task<IActionResult> GetRelationshipTypesAsync() =>
        WithErrorHandlingAsync(() =>
            Json(from rt in db.FindRelationshipTypes()
                 select new RelationshipTypeModel(rt)));

    [HttpDelete($"relationships/types/{{{nameof(relationshipTypeId)}:int}}")]
    public Task<IActionResult> DeleteRelationshipTypeAsync([FromRoute] int relationshipTypeId) =>
        WithErrorHandlingAsync(async () =>
        {
            var relType = await db.GetRelationshipTypeAsync(relationshipTypeId);
            db.DeleteRelationshipType(relType);
            await db.SaveChangesAsync();
            return Ok();
        });

    [HttpPost("relationships/types")]
    [HttpHeader("Accept", "application/json")]
    public Task<IActionResult> SetRelationshipTypeAsync([FromBody] string[] names) =>
        WithErrorHandlingAsync(async () =>
        {
            if (names.Length is < 1 or > 2)
            {
                throw new ArgumentException("Must provide only 1 or 2 names", nameof(names));
            }

            var parentRole = names[0];
            var childRole = names.LastOrDefault();
            var rt = await db.SetRelationshipTypeAsync(parentRole, childRole);
            await db.SaveChangesAsync();
            return Json(new RelationshipTypeModel(rt));
        });
}
