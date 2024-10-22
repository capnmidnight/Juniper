using Juniper.Cedrus.Entities;

using Microsoft.EntityFrameworkCore;

namespace Juniper.Cedrus.Models;

public partial class CedrusContextSecure
{
    public async Task<Entity> TagEntityAsync(Entity entity, string tag, CedrusUser user)
    {
        var tagEntity = await SetEntityAsync(TagEntityType, tag, user);
        await SetRelationshipAsync(TagRelationshipType, tagEntity, entity, null, user);
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
