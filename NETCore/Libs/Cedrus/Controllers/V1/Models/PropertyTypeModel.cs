using Juniper.Cedrus.Entities;
using Juniper.Units;

namespace Juniper.Cedrus.Controllers.V1;

public class PropertyTypeModel
{
    public string TypeStamp => "propertyType";
    public int? Id { get; }
    public string? Name { get; }
    public DataType? DataType { get; }

    public UnitsCategory? UnitsCategory { get; }

    public string? Description { get; }

    public PropertyTypeModel(PropertyType? value)
    {
        Id = value?.Id;
        Name = value?.Name;
        DataType = value?.Type;
        UnitsCategory = value?.UnitsCategory;
        Description = value?.Description;
    }
}
