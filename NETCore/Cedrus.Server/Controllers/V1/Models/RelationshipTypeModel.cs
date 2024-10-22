using Juniper.Cedrus.Entities;

namespace Juniper.Cedrus.Controllers.V1;

public class RelationshipTypeModel : AbstractNamedModel
{
    public string TypeStamp => "relationshipType";
    public string ParentRole { get; }
    public string ChildRole { get; }

    public RelationshipTypeModel(RelationshipType value)
        : base(value)
    {
        ParentRole = value?.ParentRole ?? "Unknown";
        ChildRole = value?.ChildRole ?? "Unknown"; ;
    }
}