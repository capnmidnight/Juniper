using Microsoft.AspNetCore.Mvc;

namespace Juniper.Cedrus.Controllers.V1;

public partial class CedrusController
{
    public record GetRelationshipsInput(bool? ExpandGraph, NameOrId[]? Parent, NameOrId[]? Child, NameOrId[]? Both);
    [HttpPost("relationships/search")]
    [HttpHeader("Accept", "application/json")]
    public Task<IActionResult> FindRelationshipsAsync([FromBody] GetRelationshipsInput? input)
    {
        var memo = new Memoizer();
        return WithUserAsync(user =>
        {
            var relationshipTypes = input?.Both.OfTypeStamp("relationshipType");
            var relationships = input?.Both.OfTypeStamp("relationship");
            var bothEntityTypes = input?.Both.OfTypeStamp("entityType");
            var bothEntities = input?.Both.OfTypeStamp("entity");
            var parentEntityTypes = input?.Parent.OfTypeStamp("entityType");
            var parentEntities = input?.Parent.OfTypeStamp("entity");
            var childEntityTypes = input?.Child.OfTypeStamp("entityType");
            var childEntities = input?.Child.OfTypeStamp("entity");

            return Json(from r in db.GetRelationships(user, relationshipTypes, relationships, bothEntityTypes, bothEntities, parentEntityTypes, parentEntities, childEntityTypes, childEntities, input?.ExpandGraph)
                        select r.Memo(memo, () => new RelationshipModel(r, memo)));
        });
    }

    [HttpDelete($"relationships/{{{nameof(relationshipId)}:int}}")]
    public Task<IActionResult> DeleteRelationshipAsync([FromRoute] int relationshipId) =>
        WithUserAsync(async user =>
        {
            var relationship = db.GetRelationships(user, relationships: [new NameOrId("relationship", Id: relationshipId)]).SingleOrDefault()
                ?? throw new FileNotFoundException($"Relationship:{relationshipId}");
            db.DeleteRelationship(relationship);
            await db.SaveChangesAsync();
            return Ok();
        });

    public record SetRelationshipInput(
        NameOrId? Type,
        NameOrId ChildEntity,
        NameOrId? PropertyEntity
    );
    [HttpPost($"entities/{{{nameof(parentEntityId)}:int}}/relationships")]
    [HttpHeader("Accept", "application/json")]
    public Task<IActionResult> SetRelationshipAsync([FromRoute] int parentEntityId, [FromBody] SetRelationshipInput input)
    {
        var memo = new Memoizer();
        return WithUserAsync(async user =>
        {
            var parent = await db.GetEntityAsync(parentEntityId, user);
            var child = await db.GetEntityAsync(input.ChildEntity, user);
            var propertyEntity = await db.FindEntityAsync(user, input.PropertyEntity);
            var type = await db.GetRelationshipTypeAsync(input.Type);
            var rel = await db.SetRelationshipAsync(type, parent, child, propertyEntity, user);
            await db.SaveChangesAsync();
            return Json(rel.Memo(memo, () => new RelationshipModel(rel, memo)));
        });
    }
}
