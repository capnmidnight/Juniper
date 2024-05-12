using Juniper.Cedrus.Entities;

namespace Juniper.Cedrus.Controllers.V1;

public class PropertyTypeValidValueModel
{
    public string TypeStamp => "propertyTypeValidValue";
    public int? Id { get; }

    public PropertyTypeModel PropertyType { get; }

    public string? Value { get; }

    public PropertyTypeValidValueModel(PropertyTypeValidValue? value)
    {
        Id = value?.Id;
        PropertyType = new PropertyTypeModel(value?.PropertyType);
        Value = value?.Value;
    }
}
