using Juniper.Cedrus.Entities;
using Juniper.Units;

namespace Juniper.Cedrus.Controllers.V1;

public class PropertyModel : AbstractSequencedModel
{
    public string TypeStamp => "property";
    public EntityModel Entity { get; }
    public PropertyTypeModel Type { get; }
    public object Value { get; }
    public UnitOfMeasure Units { get; }
    public UserModel CreatedBy { get; }
    public DateTime CreatedOn { get; }
    public UserModel? UpdatedBy { get; }
    public DateTime? UpdatedOn { get; }
    public EntityModel? Reference { get; }

    public PropertyModel(Property property, Memoizer? memo = null)
        : base(property)
    {
        Type = property.Type.Memo(memo, () => new PropertyTypeModel(property.Type));
        Value = property.Value;
        Units = property.Units;
        CreatedBy = property.User.Memo(memo, () => new UserModel(property.User, memo));
        CreatedOn = property.CreatedOn;
        UpdatedBy = property.UpdatedByUser?.Memo(memo, () => new UserModel(property.UpdatedByUser, memo));
        UpdatedOn = property.UpdatedOn;
        Reference = property.Reference?.Memo(memo, () => new EntityModel(property.Reference, memo));
        Entity = property.Entity.Memo(memo, () => new EntityModel(property.Entity, memo));
    }
}
