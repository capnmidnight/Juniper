using Juniper.Cedrus.Entities;

using Microsoft.EntityFrameworkCore;

namespace Juniper.Cedrus.Models;

public partial class CedrusContextSecure
{
    public async Task<Relationship> AddCommentToEntityAsync(Entity entity, string commentText, CedrusUser user)
    {
        var commentEntity = await SetEntityAsync(CommentEntityType, $"Comment {Guid.NewGuid()}", user);
        await SetPropertyAsync(DescriptionPropertyType, commentEntity, commentText, user);
        var comment = await SetRelationshipAsync(CommentRelationshipType, entity, commentEntity, null, user);
        return comment;
    }

    public IQueryable<Property> GetCommentsOnEntity(Entity entity, CedrusUser user) =>
        GetAllRelationships(user)
            .Include(r => r.Child)
                .ThenInclude(r => r.Properties)
            .Where(r => r.Type == CommentRelationshipType
                && r.Parent == entity
                && r.Child.Type == CommentEntityType)
            .Select(r => r.Child)
            .SelectMany(e => e.Properties)
            .Where(p => p.Type == DescriptionPropertyType);
}
