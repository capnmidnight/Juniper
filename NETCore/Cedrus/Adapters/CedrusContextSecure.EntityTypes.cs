using Juniper.Cedrus.Entities;
using Juniper.Data;

using Microsoft.EntityFrameworkCore;

namespace Juniper.Cedrus.Models;

public partial class CedrusContextSecure
{

    [SeedValue]
    public EntityType ReferenceEntityType => Lazy(() => SetEntityTypeAsync("Reference", false));

    [SeedValue]
    public EntityType CommentEntityType => Lazy(() => SetEntityTypeAsync("Comment", false));

    [SeedValue]
    public EntityType TagEntityType => Lazy(() => SetEntityTypeAsync("Tag", false));

    private IQueryable<EntityType> EntityTypes =>
        insecure.EntityTypes
            .Include(et => et.Parent)
            .Include(et => et.Children);

    public IQueryable<EntityType> GetEntityTypesWithEntities(CedrusUser user) =>
        EntityTypes
            .Include(et => et.Entities);

    public async Task<EntityType> GetEntityTypeAsync(NameOrId input)
    {
        input.CheckTypeStamp("entityType");
        return await FindEntityTypes(input).SingleOrDefaultAsync()
            ?? throw new ArgumentException("Input does not specify a searchable entity type", nameof(input));
    }

    public Task<EntityType> SetEntityTypeAsync(string name, bool isPrimary, EntityType? parentEntityType = null) =>
        insecure.EntityTypes.UpsertAsync(
            ValidateString(nameof(name), name),
            () => new EntityType
            {
                Name = name,
                IsPrimary = isPrimary,
                Parent = parentEntityType
            },
            (value) =>
            {
                if (parentEntityType is not null)
                {
                    // make sure we don't end up creating a graph cycle
                    // out of what should be strictly a tree
                    var queue = new Queue<EntityType> { parentEntityType };
                    while (!queue.Empty())
                    {
                        var here = queue.Dequeue();
                        if (here.Name == value.Name)
                        {
                            throw new InvalidOperationException("The requested operation would create a graph cycle in the inheritance hierarchy of entity types, which is not allowed.");
                        }

                        var parent = here.Parent;
                        if (parent is null && here.ParentId is not null)
                        {
                            parent = EntityTypes.Include(et => et.Parent).FirstOrDefault(et => et.Id == here.ParentId);
                        }

                        if (parent is not null)
                        {
                            queue.Enqueue(parent);
                        }
                    }
                }

                value.IsPrimary = isPrimary;
                value.Parent = parentEntityType;
            }
        );

    public IQueryable<EntityType> FindEntityTypes(params NameOrId[]? inputs)
    {
        inputs.CheckTypeStamp("entityType");

        var ids = inputs.IDs();
        var names = inputs.Names();

        return from entityType in EntityTypes
               where (ids.Length == 0 || ids.Contains(entityType.Id))
                && (names.Length == 0 || names.Contains(entityType.Name))
               select entityType;
    }

    private async Task<int[]> GetEntityTypeIdChain(int entityTypeId) =>
        (await FindEntityTypes()
            .ToArrayAsync())
            .Where(et => et.Id == entityTypeId)
            .SelectMany(et => et.GetChain().Reverse())
            .Select(et => et.Id)
            .ToArray();

    public void DeleteEntityType(EntityType type) =>
        insecure.EntityTypes.Remove(type);
}