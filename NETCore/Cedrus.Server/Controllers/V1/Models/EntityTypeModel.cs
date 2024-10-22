using Juniper.Cedrus.Entities;

namespace Juniper.Cedrus.Controllers.V1;

public class EntityTypeModel : AbstractNamedModel
{
    public string TypeStamp => "entityType";
    public bool IsPrimary { get; }
    public EntityTypeModel? Parent { get; }

    public EntityTypeModel(EntityType value, Memoizer? memo = null)
        : base(value)
    {
        IsPrimary = value.IsPrimary;
        Parent = value.Parent?.Memo(memo, () => new EntityTypeModel(value.Parent, memo));
    }
}