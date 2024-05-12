using Juniper.Cedrus.Entities;

using Microsoft.EntityFrameworkCore;

namespace Juniper.Cedrus.Models;

public partial class CedrusContextSecure
{
    public Entity TagEntity(Entity entity, string tag, CedrusUser user) =>
        TagEntity(entity, tag, null, user);

    public Entity TagEntity(Entity entity, string tag, string? description, CedrusUser user)
    {
        var tagEntity = SetEntity(TagEntityType, tag, user);
        if (description is not null)
        {
            SetProperty(DescriptionPropertyType, tagEntity, description, user);
        }
        SetRelationship(TagRelationshipType, tagEntity, entity, user);
        return tagEntity;
    }

    public IQueryable<string> GetTagsAsync() =>
        from entity in insecure.Entities
        where entity.Type.Name == TagEntityType.Name
        select entity.DisplayName;

    public IQueryable<string> GetTagsOnEntityAsync(Entity entity) =>
        from e in insecure.Entities.Include(e => e.Parents)
        where e.Id == entity.Id
        from p in e.Parents
        where p.Type.Id == TagRelationshipType.Id
        select p.Parent.DisplayName;
}
