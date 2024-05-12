using Juniper.Cedrus.Entities;

namespace Juniper.Cedrus.Controllers.V1;

public class RelationshipModel : AbstractClassificationMarked
{
    public string TypeStamp => "relationship";
    public RelationshipTypeModel Type { get; }

    public EntityModel Parent { get; }

    public EntityModel Child { get; }

    public UserModel User { get; }
    public DateTime? CreatedOn { get; }

    public RelationshipModel(Relationship? relationship)
        : base(relationship)
    {
        Type = new RelationshipTypeModel(relationship?.Type);
        Parent = new EntityModel(relationship?.Parent);
        Child = new EntityModel(relationship?.Child);
        User = new UserModel(relationship?.User);
        CreatedOn = relationship?.CreatedOn;
    }
}