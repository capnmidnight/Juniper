using Microsoft.AspNetCore.Mvc;

namespace Juniper.Cedrus.Controllers.V1;

public partial class CedrusController
{
    [HttpGet("relationships")]
    public Task<IActionResult> GetRelationshipsAsync() =>
        WithUserAsync(user =>
            Json(from r in db.GetRelationships(user)
                 select new RelationshipModel(r)));


    [HttpDelete($"relationships/{{{nameof(relationshipId)}:int}}")]
    public Task<IActionResult> EndRelationshipAsync([FromRoute] int relationshipId) =>
        WithUserAsync(async user =>
        {
            var relationship = await db.GetRelationshipAsync(relationshipId, user);
            db.EndRelationship(relationship);
            await db.SaveChangesAsync();
            return Ok();
        });

    [HttpPost($"entities/{{{nameof(parentEntityId)}:int}}/relationships")]
    public Task<IActionResult> SetRelationshipAsync([FromRoute] int parentEntityId, [FromBody] SetRelationshipInput input) =>
        WithUserAsync(async user =>
        {
            var parent = await db.GetEntityAsync(parentEntityId, user);
            var child = await db.GetEntityAsync(input.ChildEntity, user);
            var type = await db.GetRelationshipTypeAsync(input.Type);
            var classification = await db.GetClassificationAsync(input.Classification, user);
            var rel = db.SetRelationship(type, parent, child, user, classification, input.Start, input.End);
            await db.SaveChangesAsync();
            return Json(new RelationshipModel(rel));
        });
}
