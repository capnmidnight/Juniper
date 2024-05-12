using Juniper.Cedrus.Entities;

namespace Juniper.Cedrus.Controllers.V1;

public class EntityTypeModel
{
    public string TypeStamp => "entityType";
    public int? Id { get; }
    public string? Name { get; }

    public EntityTypeModel(EntityType? value)
    {
        Id = value?.Id;
        Name = value?.Name;
    }
}