using Juniper.Cedrus.Entities;

namespace Juniper.Cedrus.Models;

public partial class CedrusContextSecure
{
    public async Task<Relationship> AddCommentToEntityAsync(int entityId, string commentText, CedrusUser user)
    {
        var entity = await GetEntityAsync(entityId, user);
        var commentEntity = SetEntity(CommentEntityType, $"Comment {new Guid()}", user);
        SetProperty(DescriptionPropertyType, commentEntity, commentText, user);
        var comment = SetRelationship(CommentRelationshipType, entity, commentEntity, user);
        return comment;
    }

    public IEnumerable<Property> GetCommentsOnEntity(Entity entity, CedrusUser user) =>
        from e in GetEntities(user)
        where e.Id == entity.Id
        from r in e.Children
        where r.TypeId == CommentRelationshipType.Id
            && r.Child.TypeId == CommentEntityType.Id
        from p in r.Child.Properties
        where p.TypeId == DescriptionPropertyType.Id
        orderby p.Start
        select p;
}
