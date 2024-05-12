using Juniper.Units;

using Microsoft.EntityFrameworkCore;

using Juniper.Cedrus.Entities;
using Juniper.Cedrus.Controllers.V1;

namespace Juniper.Cedrus.Models;

public partial class CedrusContextSecure
{
    public PropertyType NamePropertyType => SetPropertyType(DataType.String, "Name", "The display name of an entity.");
    public PropertyType DescriptionPropertyType => SetPropertyType(DataType.String, "Description", "A long-form description of an entity.");
    public PropertyType ReferenceLinkPropertyType => SetPropertyType(DataType.Link, "ReferenceLink", "A link to a reference file that specifies the given data.");
    public PropertyType ReferenceFilePropertyType => SetPropertyType(DataType.File, "ReferenceFile", "A file that specifies the given data.");

    public IQueryable<PropertyType> GetPropertyTypes() => insecure.PropertyTypes;

    public async Task<PropertyType?> FindPropertyTypeAsync(string name) =>
        insecure.PropertyTypes.Local.FirstOrDefault(t => t.Name == name)
        ?? await insecure.PropertyTypes.FirstOrDefaultAsync(t => t.Name == name);

    public async Task<PropertyType> GetPropertyTypeAsync(int propertyTypeId) =>
        await insecure.PropertyTypes.FindAsync(propertyTypeId)
        ?? throw new FileNotFoundException();

    public async Task<PropertyType> GetPropertyTypeAsync(string name) =>
        await FindPropertyTypeAsync(name)
        ?? throw new FileNotFoundException();

    public async Task<PropertyType> GetPropertyTypeAsync(IDOrName input)
    {
        if(input.Id is not null)
        {
            return await GetPropertyTypeAsync(input.Id.Value);
        }
        else if(input.Name is not null)
        {
            return await GetPropertyTypeAsync(input.Name);
        }

        throw new ArgumentException("Input does not specify a searchable property type", nameof(input));
    }

    public PropertyType SetPropertyType(DataType type, string name, string description, UnitsCategory unitsCategory = UnitsCategory.None) =>
        SetPropertyTypeAsync(type, name, description, unitsCategory).Result;

    public Task<PropertyType> SetPropertyTypeAsync(DataType type, string name, string description, UnitsCategory unitsCategory = UnitsCategory.None) =>
        insecure.PropertyTypes.UpsertAsync(
            ValidateString(nameof(name), name),
            () => new PropertyType
            {
                Name = name,
                Type = type,
                UnitsCategory = unitsCategory,
                Description = description
            },
            value =>
            {
                value.Type = type;
                value.UnitsCategory = unitsCategory;
                value.Description = description;
            }
        );

    public void DeletePropertyType(PropertyType pt) =>
        insecure.PropertyTypes.Remove(pt);
}
