using Juniper.Cedrus.Controllers.V1;
using Juniper.Cedrus.Entities;

namespace Juniper.Cedrus.Models;

public partial class CedrusContextSecure
{

    public DataTreeModel GetTree(EntityType[]? entityTypes, RelationshipType[]? relationshipPriorities, CedrusUser user)
    {
        var entities = new Dictionary<int, FlatEntityModel>();
        var properties = new Dictionary<int, FlatPropertyModel>();
        var priorityMap = relationshipPriorities
            ?.Select((r, index) => (r.Id, index))
            ?.ToDictionary(r => r.Id, r => (int?)r.index);

        foreach (var e in GetEntities(user, entityTypes))
        {
            var defaultParent = e
                .Parents
                .OrderBy(rel => priorityMap?.Get(rel.TypeId) ?? int.MaxValue)
                .FirstOrDefault();

            entities.Add(e.Id, new FlatEntityModel(
                e.Id,
                defaultParent?.ParentId,
                e.TypeId,
                e.DisplayName,
                e.Type.Name,
                e.Parents
                    .Select(p => new FlatRelationshipModel(p.ParentId, p.Type.ParentRole))
                    .ToArray(),
                e.Children
                    .Select(c => new FlatRelationshipModel(c.ChildId, c.Type.ChildRole))
                    .ToArray(),
                e.Properties
                    .Select(p => p.Id)
                    .ToArray()
            ));

            foreach (var p in e.Properties)
            {
                var reference = p.ReferenceEntity?.Properties?.FirstOrDefault(rp => rp.TypeName.StartsWith("Reference"));

                if (!properties.ContainsKey(p.Id))
                {
                    properties.Add(p.Id, new FlatPropertyModel(
                        p.Id,
                        p.TypeId,
                        p.Type.Name,
                        p.Type.Description,
                        p.Type.Type,
                        p.Type.UnitsCategory,
                        p.Units,
                        p.Value,
                        reference?.Id
                    ));
                }

                if (reference is not null && !properties.ContainsKey(reference.Id))
                {
                    properties.Add(reference.Id, new FlatPropertyModel(
                        reference.Id,
                        reference.TypeId,
                        reference.Entity.DisplayName,
                        reference.Type.Description,
                        reference.Type.Type,
                        reference.Type.UnitsCategory,
                        reference.Units,
                        reference.Value,
                        null
                    ));
                }
            }
        }

        return new DataTreeModel(
            entities,
            properties,
            entities
                .Values
                .Where(e => e.Parents.Length == 0
                    && e.TypeId != ReferenceEntityType.Id)
                .Select(e => e.Id)
                .ToArray()
        );
    }
}
