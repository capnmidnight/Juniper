using Juniper.Cedrus.Entities;
using Juniper.Units;

namespace Juniper.Cedrus.Controllers.V1;

public class PropertyTypeModel : AbstractNamedModel
{
    public string TypeStamp => "propertyType";
    public DataType Type { get; }
    public StorageType Storage { get; }
    public UnitsCategory UnitsCategory { get; }
    public string Description { get; }

    public string DataType => $"{Type}{Storage}";

    public PropertyTypeModel(PropertyType value)
        : base(value)
    {
        Type = value.Type;
        Storage = value.Storage;
        UnitsCategory = value.UnitsCategory;
        Description = value.Description;
    }
}
