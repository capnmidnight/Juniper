using Juniper.Cedrus.Entities;

namespace Juniper.Cedrus.Controllers.V1;

public class RelationshipTypeModel
{
    public string TypeStamp => "relationshipType";
    public int Id { get; }
    public string Name { get; }
    public string ParentRole { get; }
    public string ChildRole { get; }

    public RelationshipTypeModel(RelationshipType? value)
    {
        Id = value?.Id ?? 0;
        Name = value?.Name ?? "Unknown";
        ParentRole = value?.ParentRole ?? "Unknown";
        ChildRole = value?.ChildRole ?? "Unknown"; ;
    }
}