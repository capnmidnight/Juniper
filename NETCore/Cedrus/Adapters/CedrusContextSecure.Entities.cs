using Juniper.Cedrus.Entities;
using Juniper.Data;

using Microsoft.EntityFrameworkCore;

namespace Juniper.Cedrus.Models;

public partial class CedrusContextSecure
{
    public IQueryable<Entity> GetEntities(CedrusUser currentUser) =>
        insecure
            .Entities
            .Include(e => e.Type)
            .Include(e => e.User);

    public IQueryable<Entity> GetEntities(CedrusUser currentUser, EntityType[]? entityTypes = null, int[]? entityIds = null)
    {
        entityIds ??= Array.Empty<int>();
        var entityTypeIds = entityTypes?.Select(et => et.Id)?.ToArray() ?? Array.Empty<int>();

        return insecure
            .Entities
            .Where(e => (entityTypeIds.Length == 0 || entityTypeIds.Contains(e.TypeId))
                    && (entityIds.Length == 0 || entityIds.Contains(e.Id)))
            .Include(e => e.Type)
                .ThenInclude(et => et.Parent)
            .Include(e => e.Parents
                            .Where(r => (entityTypeIds.Length == 0 || entityTypeIds.Contains(r.Parent.TypeId)))
            )
                .ThenInclude(p => p.Parent)
            .Include(e => e.Children
                            .Where(r => (entityTypeIds.Length == 0 || entityTypeIds.Contains(r.Parent.TypeId)))
            )
                .ThenInclude(r => r.Child);
    }

    public async Task<Entity> GetEntityAsync(int entityId, CedrusUser user) =>
        await GetEntities(user)
            .SingleOrDefaultAsync(e => e.Id == entityId)
        ?? throw new FileNotFoundException($"Entity:{entityId}");

    public async Task<Entity> GetEntityAsync(string name, CedrusUser user) =>
            await GetEntities(user)
                .SingleOrDefaultAsync(e => e.Name == name)
            ?? throw new FileNotFoundException($"Entity:{name}");

    public async Task<Entity?> FindEntityAsync(CedrusUser user, NameOrId? input)
    {
        input.CheckTypeStamp("entity");

        if (input?.Id is not null)
        {
            return await GetEntityAsync(input.Id.Value, user);
        }
        else if (input?.Name is not null)
        {
            return await GetEntityAsync(input.Name, user);
        }
        else
        {
            return null;
        }
    }

    public IQueryable<Entity> FindEntities(CedrusUser user, NameOrId[]? entityTypes = null, NameOrId[]? entities = null)
    {
        entityTypes.CheckTypeStamp("entityType");
        entities.CheckTypeStamp("entity");

        var entityTypeIds = entityTypes.IDs();
        var entityTypeNames = entityTypes.Names();
        var entityIds = entities.IDs();
        var entityNames = entities.Names();

        return from entity in insecure.Entities
               where (entityTypeIds.Length == 0 || entityTypeIds.Contains(entity.TypeId))
                && (entityTypeNames.Length == 0 || entityTypeNames.Contains(entity.Type.Name))
                && (entityIds.Length == 0 || entityIds.Contains(entity.Id))
                && (entityNames.Length == 0 || entityNames.Contains(entity.Name))
               select entity;
    }

    public async Task<Entity> GetEntityAsync(NameOrId input, CedrusUser user) =>
        await FindEntityAsync(user, input)
            ?? throw new ArgumentException("Input does not specify a searchable entity", nameof(input));

    public async Task<Entity> SetEntityAsync(EntityType entityType, string name, CedrusUser user)
    {
        var entity = await insecure.Entities.UpsertAsync(
            ValidateString(nameof(name), name),
            () => new Entity
            {
                Type = entityType,
                Name = name,
                User = user
            },
            value =>
            {
                value.Type = entityType;
            }
        );

        await SetPropertyAsync(NamePropertyType, entity, name, user);

        return entity;
    }

    public void DeleteEntity(Entity entity)
    {
        insecure.Entities.Remove(entity);
    }

    public void MarkEntityReviewed(Entity entity, CedrusUser user)
    {
        entity.ReviewedByUser = user;
        entity.ReviewedOn = DateTime.Now;
    }
}
