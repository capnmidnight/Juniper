using Juniper.Cedrus.Entities;

namespace Juniper.Cedrus.Controllers.V1;

public class EntityModel : AbstractClassificationMarked
{
    public string TypeStamp => "entity";
    public string? Name { get; }
    public EntityTypeModel Type { get; }
    public UserModel User { get; }
    public DateTime? CreatedOn { get; }

    public string? Link { get; }

    public Dictionary<string, PropertyModel> Properties { get; }

    public EntityModel(Entity? entity)
        : base(entity)
    {
        Name = entity?.DisplayName;
        Link = Id is null ? null : $"/entities/{Id}";
        Type = new EntityTypeModel(entity?.Type);
        User = new UserModel(entity?.User);
        CreatedOn = entity?.CreatedOn;
        Properties = entity
            ?.Properties
            ?.ToDictionary(
                v => v.Type.Name,
                v => new PropertyModel(v))
            ?? new Dictionary<string, PropertyModel>();
    }
}
