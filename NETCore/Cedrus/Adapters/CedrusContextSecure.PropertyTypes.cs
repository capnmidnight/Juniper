using Juniper.Cedrus.Entities;
using Juniper.Data;
using Juniper.Units;

using Microsoft.EntityFrameworkCore;

namespace Juniper.Cedrus.Models;

public partial class CedrusContextSecure
{

    [SeedValue]
    public PropertyType NamePropertyType => Lazy(() => SetPropertyTypeAsync(DataType.String, "Name", "The display name of an entity."));

    [SeedValue]
    public PropertyType DescriptionPropertyType => Lazy(() => SetPropertyTypeAsync(DataType.LongText, "Description", "A long-form description of an entity."));

    [SeedValue]
    public PropertyType AuthorsPropertyType => Lazy(() => SetPropertyTypeAsync(DataType.String, "Authors", "The names of the authors for a given document."));

    [SeedValue]
    public PropertyType ImagePropertyType => Lazy(() => SetPropertyTypeAsync(DataType.File, "Image", "An image file. [content-type: image/*]"));

    [SeedValue]
    public PropertyType ReferenceLinkPropertyType => Lazy(() => SetPropertyTypeAsync(DataType.Link, "ReferenceLink", "A link to a reference file that specifies the given data."));

    [SeedValue]
    public PropertyType ReferenceFilePropertyType => Lazy(() => SetPropertyTypeAsync(DataType.File, "ReferenceFile", "A file that specifies the given data."));

    [SeedValue]
    public PropertyType PublicationDateType => Lazy(() => SetPropertyTypeAsync(DataType.Date, "Publication Date", "The date a publication was released for distribution"));

    public async Task<PropertyType?> FindPropertyTypeAsync(string name) =>
        insecure.PropertyTypes.Local.FirstOrDefault(t => t.Name == name)
        ?? await insecure.PropertyTypes.FirstOrDefaultAsync(t => t.Name == name);

    public async Task<PropertyType> GetPropertyTypeAsync(int propertyTypeId) =>
        await insecure.PropertyTypes.FindAsync(propertyTypeId)
        ?? throw new FileNotFoundException($"PropertyType:{propertyTypeId}");

    public async Task<PropertyType> GetPropertyTypeAsync(string name) =>
        await FindPropertyTypeAsync(name)
        ?? throw new FileNotFoundException($"PropertyType:{name}");

    public async Task<PropertyType?> GetPropertyTypeAsync(NameOrId? input)
    {
        input.CheckTypeStamp("propertyType");

        if (input is null)
        {
            return null;
        }
        else if (input.Id is not null)
        {
            return await GetPropertyTypeAsync(input.Id.Value);
        }
        else if (input.Name is not null)
        {
            return await GetPropertyTypeAsync(input.Name);
        }
        else
        {
            throw new ArgumentException("Input does not specify a searchable property type", nameof(input));
        }
    }

    public IQueryable<PropertyType> FindPropertyTypes(string[] names) =>
        from t in insecure.PropertyTypes
        where names.Contains(t.Name)
        select t;


    public async Task<PropertyType[]?> FindPropertyTypesAsync(string? propertyTypesCSV)
    {
        if (propertyTypesCSV is null)
        {
            return null;
        }

        var parts = propertyTypesCSV.Split(',').ToArray();

        var typeMap = await FindPropertyTypes(parts)
            .ToDictionaryAsync(rt => rt.Name);

        var types = (
            from part in parts
            where typeMap.ContainsKey(part)
            select typeMap[part]
        ).ToArray();

        return types;
    }

    public IQueryable<PropertyType> FindPropertyTypes(NameOrId[]? input = null)
    {
        input.CheckTypeStamp("propertyType");

        var ids = input.IDs();
        var names = input.Names();

        return from pt in insecure.PropertyTypes
               where (ids.Length == 0 || ids.Contains(pt.Id))
                && (names.Length == 0 || names.Contains(pt.Name))
               select pt;
    }

    public Task<PropertyType> SetPropertyTypeAsync(DataType type, StorageType storage, string name, string description, UnitsCategory unitsCategory = UnitsCategory.None) =>
        insecure.PropertyTypes.UpsertAsync(
            ValidateString(nameof(name), name),
            () => new PropertyType
            {
                Name = name,
                Type = type,
                Storage = storage,
                UnitsCategory = unitsCategory,
                Description = description
            },
            value =>
            {
                value.Type = type;
                value.Storage = storage;
                value.UnitsCategory = unitsCategory;
                value.Description = description;
            }
        );

    public Task<PropertyType> SetPropertyTypeAsync(DataType type, string name, string description, UnitsCategory unitsCategory = UnitsCategory.None) =>
        SetPropertyTypeAsync(type, StorageType.Single, name, description, unitsCategory);

    public async Task<PropertyType> SetEnumerationPropertyTypeAsync<EnumT>(StorageType storage, string description)
        where EnumT : struct, Enum
    {
        var enumT = typeof(EnumT);
        var pt = await SetPropertyTypeAsync(
            DataType.Enumeration,
            storage,
            enumT.Name,
            description
        );
        await SetPropertyTypeValidValuesFromEnumAsync<EnumT>(pt);
        return pt;
    }

    public Task<PropertyType> SetEnumerationPropertyTypeAsync<EnumT>(string description)
        where EnumT : struct, Enum =>
        SetEnumerationPropertyTypeAsync<EnumT>(StorageType.Single, description);

    public void DeletePropertyType(PropertyType pt) =>
        insecure.PropertyTypes.Remove(pt);
}
