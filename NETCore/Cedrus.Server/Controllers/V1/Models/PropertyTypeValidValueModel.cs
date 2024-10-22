using Juniper.Cedrus.Entities;

namespace Juniper.Cedrus.Controllers.V1;

public class PropertyTypeValidValueModel : AbstractSequencedModel
{
    public string TypeStamp => "propertyTypeValidValue";
    public PropertyTypeModel PropertyType { get; }
    public string Value { get; }

    public PropertyTypeValidValueModel(PropertyTypeValidValue value, Memoizer? memo = null)
        : base(value)
    {
        PropertyType = value.PropertyType.Memo(memo, () => new PropertyTypeModel(value.PropertyType));
        Value = value.Value;
    }
}
