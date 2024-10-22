using Juniper.Cedrus.Entities;

using Microsoft.EntityFrameworkCore;

namespace Juniper.Cedrus.Models;

public partial class CedrusContextSecure
{
    [SeedValue]
    public RelationshipType DefaultRelationshipType => Lazy(() => SetRelationshipTypeAsync("Default"));

    [SeedValue]
    public RelationshipType CommentRelationshipType => Lazy(() => SetRelationshipTypeAsync("Comment"));

    [SeedValue]
    public RelationshipType TagRelationshipType => Lazy(() => SetRelationshipTypeAsync("Tag"));


    private IQueryable<RelationshipType> RelationshipTypes =>
        insecure.RelationshipTypes;

    public void DeleteRelationshipType(RelationshipType relationshipType) =>
        insecure.RelationshipTypes.Remove(relationshipType);

    public async Task<RelationshipType> GetRelationshipTypeAsync(int relationshipTypeId) =>
        await insecure.RelationshipTypes.FindAsync(relationshipTypeId)
            ?? throw new FileNotFoundException($"RelationshipType:{relationshipTypeId}");

    public IQueryable<RelationshipType> FindRelationshipTypesAsync(string[] names) =>
        from t in insecure.RelationshipTypes
        where names.Contains(t.Name)
        select t;

    public async Task<RelationshipType[]?> FindRelationshipTypesAsync(string? relTypesCSV)
    {
        if (relTypesCSV is null)
        {
            return null;
        }

        var parts = relTypesCSV.Split(',').ToArray();

        var typeMap = await FindRelationshipTypesAsync(parts)
            .ToDictionaryAsync(rt => rt.Name);

        var types = (
            from part in parts
            where typeMap.ContainsKey(part)
            select typeMap[part]
        ).ToArray();

        return types;
    }

    public IQueryable<RelationshipType> FindRelationshipTypes(NameOrId[]? inputs = null)
    {
        inputs.CheckTypeStamp("relationshipType");

        var ids = inputs.IDs();
        var names = inputs.Names();

        return from rt in insecure.RelationshipTypes
               where (ids.Length == 0 || ids.Contains(rt.Id))
                && (names.Length == 0 || names.Contains(rt.Name))
               select rt;
    }

    public async Task<RelationshipType?> FindRelationshipTypeAsync(string name) =>
        insecure.RelationshipTypes.Local.FirstOrDefault(t => t.Name == name)
        ?? await insecure.RelationshipTypes.FirstOrDefaultAsync(t => t.Name == name);

    public async Task<RelationshipType> GetRelationshipTypeAsync(string name) =>
        await FindRelationshipTypeAsync(name)
            ?? throw new FileNotFoundException($"RelationshipType:{name}");

    public async Task<RelationshipType> GetRelationshipTypeAsync(NameOrId? input)
    {
        input.CheckTypeStamp("relationshipType");

        if (input is null)
        {
            return DefaultRelationshipType;
        }
        else if (input.Id is not null)
        {
            return await GetRelationshipTypeAsync(input.Id.Value);
        }
        else if (input.Name is not null)
        {
            return await GetRelationshipTypeAsync(input.Name);
        }
        else
        {
            throw new ArgumentException("Input does not specify a searchable relationship type", nameof(input));
        }
    }

    /// <summary>
    /// Create a type of relationship, when it's important to differentiate between kind of relationships.
    /// </summary>
    /// <param name="parentRole">The name to call this kind of relationship when looking from child to parent.</param>
    /// <param name="childRole">(Optional) The name to call this kind of relationship when looking from parent to child. Defaults to the child-to-parent name.</param>
    /// <returns></returns>
    public async Task<RelationshipType> SetRelationshipTypeAsync(string parentRole, string? childRole = null)
    {
        parentRole = ValidateString(nameof(parentRole), parentRole);

        if (string.IsNullOrWhiteSpace(childRole) && childRole is not null)
        {
            childRole = null;
        }

        childRole ??= parentRole;

        var type = await FindRelationshipTypeAsync(parentRole);

        if (type is null)
        {
            await insecure.RelationshipTypes.AddAsync(type = new RelationshipType
            {
                ParentRole = parentRole,
                ChildRole = childRole
            });
        }
        else
        {
            type.ChildRole = childRole;
        }

        return type;
    }
}