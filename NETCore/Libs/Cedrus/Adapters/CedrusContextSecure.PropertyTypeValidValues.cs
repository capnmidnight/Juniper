using Microsoft.EntityFrameworkCore;

using Juniper.Cedrus.Entities;

namespace Juniper.Cedrus.Models;

public partial class CedrusContextSecure
{
    public IQueryable<PropertyTypeValidValue> GetPropertyTypeValidValues(int? propertyTypeId = null) =>
        from ptv in insecure.PropertyTypesValidValues
        where propertyTypeId == null || ptv.PropertyTypeId == propertyTypeId
        select ptv;


    public PropertyTypeValidValue SetPropertyTypeValidValue(PropertyType type, string value) =>
        SetPropertyTypeValidValueAsync(type, value).Result;

    public async Task<PropertyTypeValidValue> SetPropertyTypeValidValueAsync(PropertyType propertyType, string value)
    {
        var validValue = insecure.PropertyTypesValidValues.Local.SingleOrDefault(x => x.PropertyType.Name == propertyType.Name && x.Value == value)
            ?? await insecure.PropertyTypesValidValues.SingleOrDefaultAsync(x => x.PropertyType.Name == propertyType.Name && x.Value == value);
        
        if(validValue is null)
        {
            insecure.PropertyTypesValidValues.Add(validValue = new PropertyTypeValidValue { 
                PropertyType = propertyType, 
                Value = value 
            });
        }

        return validValue;
    }
}
