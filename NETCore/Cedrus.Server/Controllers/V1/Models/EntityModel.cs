using Juniper.Cedrus.Entities;

namespace Juniper.Cedrus.Controllers.V1;

public class EntityModel : AbstractSequencedModel
{
    public string TypeStamp => "entity";
    public string Name { get; }
    public EntityTypeModel Type { get; }
    public UserModel CreatedBy { get; }
    public DateTime CreatedOn { get; }
    public UserModel? ReviewedBy { get; }
    public DateTime? ReviewedOn { get; }
    public string? Link { get; }

    public EntityModel(Entity entity, Memoizer? memo = null)
        : base(entity)
    {
        Name = entity.DisplayName;
        Link = $"/entities/{Id}";
        Type = entity.Type.Memo(memo, () => new EntityTypeModel(entity.Type, memo));
        CreatedBy = entity.User.Memo(memo, () => new UserModel(entity.User, memo));
        CreatedOn = entity.CreatedOn;
        ReviewedBy = entity.ReviewedByUser?.Memo(memo, () => new UserModel(entity.ReviewedByUser, memo));
        ReviewedOn = entity.ReviewedOn;
    }
}
