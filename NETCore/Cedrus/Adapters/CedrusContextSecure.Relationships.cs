using Juniper.Cedrus.Entities;
using Juniper.Data;

namespace Juniper.Cedrus.Models;

public partial class CedrusContextSecure
{
    public IQueryable<Relationship> GetAllRelationships(CedrusUser user) =>
        insecure.Relationships;

    public IEnumerable<Relationship> GetRelationships(CedrusUser user,
        NameOrId[]? relationshipTypes = null,
        NameOrId[]? relationships = null,
        NameOrId[]? bothEntityTypes = null,
        NameOrId[]? bothEntities = null,
        NameOrId[]? parentEntityTypes = null,
        NameOrId[]? parentEntities = null,
        NameOrId[]? childEntityTypes = null,
        NameOrId[]? childEntities = null,
        bool? expandGraph = null)
    {
        relationshipTypes.CheckTypeStamp("relationshipType");
        relationships.CheckTypeStamp("relationship");
        bothEntityTypes.CheckTypeStamp("entityType");
        bothEntities.CheckTypeStamp("entity");
        parentEntityTypes.CheckTypeStamp("entityType");
        parentEntities.CheckTypeStamp("entity");
        childEntityTypes.CheckTypeStamp("entityType");
        childEntities.CheckTypeStamp("entity");

        var relationshipTypeIds = relationshipTypes.IDs();
        var relationshipTypeNames = relationshipTypes.Names();
        var relationshipIds = relationships.IDs();
        var bothEntityTypeIds = bothEntityTypes.IDs();
        var bothEntityTypeNames = bothEntityTypes.Names();
        var bothEntityIds = bothEntities.IDs();
        var bothEntityNames = bothEntities.Names();
        var parentEntityTypeIds = parentEntityTypes.IDs();
        var parentEntityTypeNames = parentEntityTypes.Names();
        var parentEntityIds = parentEntities.IDs();
        var parentEntityNames = parentEntities.Names();
        var childEntityTypeIds = childEntityTypes.IDs();
        var childEntityTypeNames = childEntityTypes.Names();
        var childEntityIds = childEntities.IDs();
        var childEntityNames = childEntities.Names();

        var allRelationships = GetAllRelationships(user)
            .Where(r =>
                   (relationshipTypeIds.Length == 0 || relationshipTypeIds.Contains(r.TypeId))
                && (relationshipTypeNames.Length == 0 || (relationshipTypeNames.Contains(r.Type.ParentRole) || relationshipTypeNames.Contains(r.Type.ChildRole)))
                && (relationshipIds.Length == 0 || relationshipIds.Contains(r.Id))

                && (bothEntityTypeIds.Length == 0 || bothEntityTypeIds.Contains(r.Parent.TypeId) || bothEntityTypeIds.Contains(r.Child.TypeId))
                && (bothEntityTypeNames.Length == 0 || bothEntityTypeNames.Contains(r.Parent.Type.Name) || bothEntityTypeNames.Contains(r.Child.Type.Name))

                && (parentEntityTypeIds.Length == 0 || parentEntityTypeIds.Contains(r.Parent.TypeId))
                && (parentEntityTypeNames.Length == 0 || parentEntityTypeNames.Contains(r.Parent.Type.Name))

                && (childEntityTypeIds.Length == 0 || childEntityTypeIds.Contains(r.Child.TypeId))
                && (childEntityTypeNames.Length == 0 || childEntityTypeNames.Contains(r.Child.Type.Name))
            )
            .AsEnumerable();

        var specificEntityCount = bothEntityIds.Length
            + bothEntityNames.Length
            + parentEntityIds.Length
            + parentEntityNames.Length
            + childEntityIds.Length
            + childEntityNames.Length;

        if(specificEntityCount == 0)
        {
            return allRelationships;
        }

        var selectedEntityRelationships = allRelationships
            .Where(r =>
                   (bothEntityIds.Length == 0 || bothEntityIds.Contains(r.ParentId) || bothEntityIds.Contains(r.ChildId))
                && (bothEntityNames.Length == 0 || bothEntityNames.Contains(r.Parent.Name) || bothEntityNames.Contains(r.Child.Name))

                && (parentEntityIds.Length == 0 || parentEntityIds.Contains(r.ParentId))
                && (parentEntityNames.Length == 0 || parentEntityNames.Contains(r.Parent.Name))

                && (childEntityIds.Length == 0 || childEntityIds.Contains(r.ChildId))
                && (childEntityNames.Length == 0 || childEntityNames.Contains(r.Child.Name))
            );

        if(expandGraph != true)
        {
            return selectedEntityRelationships;
        }

        var entities = new Queue<Entity>(selectedEntityRelationships.Select(r => r.Parent)
            .Union(selectedEntityRelationships.Select(r => r.Child))
            .Distinct());

        var visited = new HashSet<Entity>();
        var output = new List<Relationship>();

        while(entities.Count > 0)
        {
            var here = entities.Dequeue();
            if(!visited.Contains(here))
            {
                visited.Add(here);

                output.AddRange(here.Parents);
                output.AddRange(here.Children);

                var related = here.Parents.Select(p => p.Parent)
                    .Union(here.Children.Select(c => c.Child))
                    .Where(e => !visited.Contains(e));

                entities.AddRange(related);
            }
        }

        return output;
    }

    public Task<Relationship> SetRelationshipAsync(RelationshipType type, Entity parent, Entity child, CedrusUser user) =>
        SetRelationshipAsync(type, parent, child, null, user);

    public Task<Relationship> SetRelationshipAsync(Entity parent, Entity child, CedrusUser user) =>
        SetRelationshipAsync(DefaultRelationshipType, parent, child, null, user);

    public Task<Relationship> SetRelationshipAsync(Entity parent, Entity child, Entity properties, CedrusUser user) =>
        SetRelationshipAsync(DefaultRelationshipType, parent, child, properties, user);

    public Task<Relationship> SetRelationshipAsync(RelationshipType type, Entity parent, Entity child, Entity? properties, CedrusUser user) =>
        insecure.Relationships.UpsertAsync(
            r => r.Parent.Name == parent.Name && r.Child.Name == child.Name && r.Type.Name == type.Name,
            () => new Relationship
            {
                Parent = parent,
                Child = child,
                PropertyEntity = properties,
                Type = type,
                User = user
            },
            (here) =>
            {
                here.User = user;
            }
        );

    public void DeleteRelationship(Relationship rel) =>
        insecure.Relationships.Remove(rel);
}
