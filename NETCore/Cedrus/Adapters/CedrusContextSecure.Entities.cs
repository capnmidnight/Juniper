using Microsoft.EntityFrameworkCore;

using Juniper.Cedrus.Entities;
using Juniper.Cedrus.Controllers.V1;

namespace Juniper.Cedrus.Models;

public partial class CedrusContextSecure
{
    public IQueryable<Entity> GetEntities(CedrusUser currentUser, DateTime context) =>
        GetEntities(currentUser, null, context);

    public IQueryable<Entity> GetEntities(CedrusUser currentUser, EntityType[]? entityTypes = null, DateTime? context = null)
    {
        context ??= DateTime.Now;

        var parts = GetClassificationParts(currentUser);

        var entityTypeIds = entityTypes?.Select(et => et.Id)?.ToArray();

        return insecure
            .Entities
            .Where(e => entityTypeIds == null || entityTypeIds.Contains(e.TypeId))
            .Secure(parts)
            .Include(e => e.Properties
                            .Where(p => p.Start <= context && context < p.End
                                && parts.Levels.Contains(p.Classification.LevelId)
                                && p.Classification.Caveats.All(cv => parts.Caveats.Contains(cv.Id))
                            )
            )
                .ThenInclude(p => p.ReferenceEntity)
                    .ThenInclude(re => re!.Properties
                            .Where(rep => rep.Start <= context && context < rep.End
                                && parts.Levels.Contains(rep.Classification.LevelId)
                                && rep.Classification.Caveats.All(cv => parts.Caveats.Contains(cv.Id)
                                && parts.Levels.Contains(rep.Classification.LevelId)
                                && rep.Classification.Caveats.All(cv => parts.Caveats.Contains(cv.Id)))
                            ))
            .Include(e => e.Parents
                            .Where(pr => pr.Start <= context && context < pr.End
                                && (entityTypeIds == null || entityTypeIds.Contains(pr.Parent.TypeId))
                                && parts.Levels.Contains(pr.Classification.LevelId)
                                && pr.Classification.Caveats.All(cv => parts.Caveats.Contains(cv.Id))
                            )
            )
                .ThenInclude(p => p.Parent)
                    .ThenInclude(pe => pe.Properties
                                        .Where(pv => pv.Start <= context && context < pv.End
                                                && parts.Levels.Contains(pv.Classification.LevelId)
                                                && pv.Classification.Caveats.All(cv => parts.Caveats.Contains(cv.Id))
                                        )
                    )
            .Include(e => e.Children
                            .Where(cr => cr.Start <= context && context < cr.End
                                && (entityTypeIds == null || entityTypeIds.Contains(cr.Parent.TypeId))
                                && parts.Levels.Contains(cr.Classification.LevelId)
                                && cr.Classification.Caveats.All(cv => parts.Caveats.Contains(cv.Id))
                            )
            )
                .ThenInclude(c => c.Child)
                    .ThenInclude(ce => ce.Properties
                                        .Where(cv => cv.Start <= context && context < cv.End
                                            && parts.Levels.Contains(cv.Classification.LevelId)
                                            && cv.Classification.Caveats.All(cv => parts.Caveats.Contains(cv.Id))
                                        )
                    );
    }

    public IQueryable<Entity> GetEntitiesOnly(CedrusUser currentUser, EntityType[]? entityTypes = null)
    {
        var parts = GetClassificationParts(currentUser);

        var entityTypeIds = entityTypes?.Select(et => et.Id)?.ToArray();

        return insecure
            .Entities
            .IgnoreAutoIncludes()
            .Include(e => e.User)
            .Include(e => e.Type)
            .Include(e => e.Classification)
            .Where(e => entityTypeIds == null || entityTypeIds.Contains(e.TypeId))
            .Secure(parts);
    }

    public async Task<Entity> GetEntityAsync(int entityId, CedrusUser user) =>
        await GetEntities(user)
            .SingleOrDefaultAsync(e => e.Id == entityId)
        ?? throw new FileNotFoundException();

    public async Task<Entity> GetEntityAsync(string name, CedrusUser user) =>
            await GetEntities(user)
                .SingleOrDefaultAsync(e => e.Name == name)
            ?? throw new FileNotFoundException();

    public async Task<Entity> GetEntityAsync(IDOrName input, CedrusUser user)
    {
        if(input.Id is not null)
        {
            return await GetEntityAsync(input.Id.Value, user);
        }
        else if(input.Name is not null)
        {
            return await GetEntityAsync(input.Name, user);
        }


        throw new ArgumentException("Input does not specify a searchable entity", nameof(input));
    }

    public Task<Entity?> FindEntityAsync(EntityType entityType, string name, CedrusUser user) =>
        GetEntities(user)
            .SingleOrDefaultAsync(e =>
                e.TypeId == entityType.Id
                    && e.Name == name);

    public async Task<Entity> GetEntityAsync(EntityType entityType, string name, CedrusUser user) =>
        await FindEntityAsync(entityType, name, user)
            ?? throw new FileNotFoundException();

    public IQueryable<Entity> GetEntitiesInGroup(string groupName) =>
        from r in insecure.Relationships
        where r.Parent.Type.Name == "Grouping"
            && r.Parent.Name == groupName
        select r.Child;

    public Entity SetEntity(EntityType entityType, string name, CedrusUser user, Classification? classification = null)
    {
        var entity = insecure.Entities.Upsert(
            ValidateString(nameof(name), name),
            () => new Entity
            {
                Type = entityType,
                Name = name,
                Classification = classification ?? U,
                User = user
            },
            value =>
            {
                value.Type = entityType;
                value.Classification = classification ?? value.Classification;
            }
        );

        var nameType = insecure.PropertyTypes.FindByName("Name")
            ?? NamePropertyType;

        SetProperty(nameType, entity, name, user, classification);

        return entity;
    }

    /// <summary>
    /// Set a reference associated with a property. 
    /// 
    /// If the property is not already in a PropertyGroup entity, it will be moved to one.
    /// </summary>
    /// <param name="reference"></param>
    /// <param name="classification"></param>
    /// <param name="user"></param>
    /// <param name="startDate"></param>
    /// <param name="endDate"></param>
    /// <returns></returns>
    public Entity SetReferenceEntity(string refName, string reference, CedrusUser user, Classification? classification = null, DateTime? startDate = null, DateTime? endDate = null) =>
        SetReferenceEntityInternal(refName, reference, ReferenceLinkPropertyType, user, classification, startDate, endDate);

    /// <summary>
    /// Set a reference associated with a property. 
    /// 
    /// If the property is not already in a PropertyGroup entity, it will be moved to one.
    /// </summary>
    /// <param name="property"></param>
    /// <param name="reference"></param>
    /// <param name="classification"></param>
    /// <param name="user"></param>
    /// <param name="startDate"></param>
    /// <param name="endDate"></param>
    /// <returns></returns>
    public Entity SetReferenceEntity(string refName, FileAsset reference, CedrusUser user, Classification? classification = null, DateTime? startDate = null, DateTime? endDate = null) =>
        SetReferenceEntityInternal(refName, reference.LinkPath, ReferenceFilePropertyType, user, classification, startDate, endDate);

    private Entity SetReferenceEntityInternal(string refName, string reference, PropertyType referencePT, CedrusUser user, Classification? classification, DateTime? startDate, DateTime? endDate)
    {
        var refEntity = SetEntity(ReferenceEntityType, refName, user, classification);
        SetProperty(referencePT, refEntity, reference, user, classification, startDate, endDate);
        return refEntity;
    }

    public void DeleteEntity(Entity entity)
    {
        insecure.Entities.Remove(entity);
    }
}
