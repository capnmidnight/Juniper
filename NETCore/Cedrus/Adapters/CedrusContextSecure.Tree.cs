using Juniper.Cedrus.Entities;
using Juniper.Data;

using Microsoft.EntityFrameworkCore;

namespace Juniper.Cedrus.Models;

public partial class CedrusContextSecure
{
    public async Task<DataTreeModel> GetTreeAsync(CedrusUser user, NameOrId[]? entityTypes = null, NameOrId[]? propertyTypes = null)
    {
        var entitiesById = new Dictionary<int, FlatEntityModel>();
        var propertiesById = new Dictionary<int, FlatPropertyModel>();

        var entities = await FindEntities(user, entityTypes)
            .ToArrayAsync();

        var allProperties = GetProperties(user, entityTypes: entityTypes, propertyTypes: propertyTypes);

        var entityTypeIds = entities
            .Select(e => e.TypeId)
            .Distinct()
            .ToArray();

        var entityTypesById = insecure
            .EntityTypes
            .AsEnumerable()
            .Where(et => entityTypeIds.Contains(et.Id))
            .SelectMany(et => et.GetChain().Reverse())
            .Distinct()
            .ToDictionary(et => et.Id, et => new FlatEntityTypeModel(
                et.Id,
                et.Name,
                et.IsPrimary,
                et.ParentId
            ));

        var propertiesByEntityId = allProperties
            .GroupBy(p => p.EntityId)
            .ToDictionary(p => p.Key, p => p.ToArray());

        foreach (var e in entities)
        {
            var entity = new FlatEntityModel(
                e.Id,
                e.TypeId,
                e.DisplayName,
                [],
                e.Parents.Select(rel => new FlatRelationshipModel(rel.ParentId, rel.ParentName)).ToArray(),
                e.Children.Select(rel => new FlatRelationshipModel(rel.ChildId, rel.ChildName)).ToArray()
            );
            entitiesById.Add(e.Id, entity);

            if (propertiesByEntityId.TryGetValue(e.Id, out var properties))
            {
                foreach (var p in properties)
                {
                    entity.Properties.Add(p.Id);

                    if (!propertiesById.ContainsKey(p.Id))
                    {
                        propertiesById.Add(p.Id, new FlatPropertyModel(
                            p.Id,
                            p.TypeId,
                            p.Type.Name,
                            p.Type.Description,
                            p.Type.Type,
                            p.Type.Storage,
                            p.Type.UnitsCategory,
                            p.Units,
                            p.Value
                        ));
                    }
                }
            }
        }

        return new DataTreeModel(
            entityTypesById,
            entitiesById,
            propertiesById
        );
    }
}
