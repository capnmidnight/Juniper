using Juniper.Cedrus.Entities;

namespace Juniper.Cedrus.Controllers.V1;

public class RelationshipModel : AbstractSequencedModel
{
    public string TypeStamp => "relationship";
    public RelationshipTypeModel Type { get; }
    public EntityModel Parent { get; }
    public EntityModel Child { get; }
    public EntityModel? PropertyEntity { get; }
    public UserModel CreatedBy { get; }
    public DateTime CreatedOn { get; }

    public RelationshipModel(Relationship relationship, Memoizer? memo = null)
        : base(relationship)
    {
        Type = relationship.Type.Memo(memo, () => new RelationshipTypeModel(relationship.Type));
        Parent = relationship.Parent.Memo(memo, () => new EntityModel(relationship.Parent, memo));
        Child = relationship.Child.Memo(memo, () => new EntityModel(relationship.Child, memo));
        PropertyEntity = relationship.PropertyEntity?.Memo(memo, () => new EntityModel(relationship.PropertyEntity, memo));
        CreatedBy = relationship.User.Memo(memo, () => new UserModel(relationship.User, memo));
        CreatedOn = relationship.CreatedOn;
    }
}