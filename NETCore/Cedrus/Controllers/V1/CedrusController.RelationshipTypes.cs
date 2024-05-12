using Microsoft.AspNetCore.Mvc;

namespace Juniper.Cedrus.Controllers.V1;

public partial class CedrusController
{
    [HttpGet("relationships/types")]
    public Task<IActionResult> GetRelationshipTypesAsync() =>
        WithErrorHandlingAsync(() =>
            Json(from rt in db.RelationshipTypes
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
    public Task<IActionResult> SetRelationshipTypeAsync([FromBody] string[] names) =>
        WithErrorHandlingAsync(async () =>
        {
            if (1 > names.Length || names.Length > 2)
            {
                throw new ArgumentException("Must provide only 1 or 2 names", nameof(names));
            }

            var parentRole = names[0];
            var childRole = names.LastOrDefault();
            var rt = db.SetRelationshipType(parentRole, childRole);
            await db.SaveChangesAsync();
            return Json(new RelationshipTypeModel(rt));
        });
}
