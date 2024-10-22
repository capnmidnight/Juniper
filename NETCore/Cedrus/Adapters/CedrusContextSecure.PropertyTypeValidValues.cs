using Juniper.Cedrus.Entities;

using Microsoft.EntityFrameworkCore;

namespace Juniper.Cedrus.Models;

public partial class CedrusContextSecure
{
    public async Task<PropertyTypeValidValue> GetPropertyTypeValidValueAsync(int validValueId) =>
        await insecure.PropertyTypesValidValues.SingleOrDefaultAsync(ptv => ptv.Id == validValueId)
        ?? throw new FileNotFoundException($"PropertyTypeValidValue:{validValueId}");

    public IQueryable<PropertyTypeValidValue> GetPropertyTypeValidValues(PropertyType? propertyType = null) =>
        from ptv in insecure.PropertyTypesValidValues
        where propertyType == null || ptv.PropertyType.Name == propertyType.Name
        select ptv;

    public async Task<IEnumerable<PropertyTypeValidValue>> SetPropertyTypeValidValueAsync(PropertyType propertyType, params string[] values)
    {
        insecure.PropertyTypesValidValues.RemoveRange(GetPropertyTypeValidValues(propertyType));
        await insecure.PropertyTypesValidValues.AddRangeAsync(
            from value in values.Distinct()
            select new PropertyTypeValidValue
            {
                PropertyType = propertyType,
                Value = value
            });

        return GetPropertyTypeValidValues(propertyType);
    }

    public Task<IEnumerable<PropertyTypeValidValue>> SetPropertyTypeValidValuesFromEnumAsync<EnumT>(PropertyType propertyType)
        where EnumT : struct, Enum =>
        SetPropertyTypeValidValueAsync(propertyType, Enum.GetNames<EnumT>());

    public void DeletePropertyTypeValidValue(PropertyTypeValidValue validValue) =>
        insecure.PropertyTypesValidValues.Remove(validValue);
}
