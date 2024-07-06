using Juniper.Cedrus.Controllers.V1;
using Juniper.Cedrus.Entities;

using Microsoft.EntityFrameworkCore;

namespace Juniper.Cedrus.Models;

public partial class CedrusContextSecure
{
    public EntityType GroupingEntityType => SetEntityType("Grouping");
    public EntityType ReferenceEntityType => SetEntityType("Reference");
    public EntityType CommentEntityType => SetEntityType("Comment");
    public EntityType TagEntityType => SetEntityType("Tag");

    public IQueryable<EntityType> EntityTypes =>
        insecure.EntityTypes;

    public async Task<EntityType?> FindEntityTypeAsync(string name) =>
        insecure.EntityTypes.Local.FirstOrDefault(t => t.Name == name)
        ?? await insecure.EntityTypes.FindByNameAsync(name);

    public async Task<EntityType> GetEntityTypeAsync(int id) =>
        await insecure.EntityTypes.FindAsync(id)
        ?? throw new FileNotFoundException();

    public async Task<EntityType> GetEntityTypeAsync(string name) =>
        await FindEntityTypeAsync(name)
        ?? throw new FileNotFoundException();

    public async Task<EntityType> GetEntityTypeAsync(IDOrName input)
    {
        if (input.Id is not null)
        {
            return await GetEntityTypeAsync(input.Id.Value);
        }
        else if (input.Name is not null)
        {
            return await GetEntityTypeAsync(input.Name);
        }

        throw new ArgumentException("Input does not specify a searchable entity type", nameof(input));
    }

    public EntityType SetEntityType(string name, EntityType? parentEntityType = null) =>
        SetEntityTypeAsync(name, parentEntityType).Result;

    public Task<EntityType> SetEntityTypeAsync(string name, EntityType? parentEntityType = null) =>
        insecure.EntityTypes.UpsertAsync(
            ValidateString(nameof(name), name),
            () => new EntityType
            {
                Name = name,
                Parent = parentEntityType
            }
        );

    public void DeleteEntityType(EntityType type) =>
        insecure.EntityTypes.Remove(type);

    public IQueryable<EntityType> FindEntityTypesAsync(string[] names) =>
        from t in insecure.EntityTypes
        where names.Contains(t.Name)
        select t;

    public async Task<EntityType[]?> FindEntityTypesAsync(string? entityTypesCSV)
    {
        if(entityTypesCSV is null)
        {
            return null;
        }

        var parts = entityTypesCSV.Split(',').ToArray();

        var typeMap = await FindEntityTypesAsync(parts)
            .ToDictionaryAsync(rt => rt.Name);

        var types = (
            from part in parts
            where typeMap.ContainsKey(part)
            select typeMap[part]
        ).ToArray();

        return types;
    }
}