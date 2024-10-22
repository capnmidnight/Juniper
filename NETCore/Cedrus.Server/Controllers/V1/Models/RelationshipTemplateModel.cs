using Juniper.Cedrus.Entities;

namespace Juniper.Cedrus.Controllers.V1;

public class RelationshipTemplateModel : AbstractNamedModel
{
    public string TypeStamp => "relationshipTemplate";
    public EntityTypeModel EntityType { get; }
    public EntityTypeModel? PropertyEntityType { get; }
    public RelationshipTypeModel RelationshipType { get; }
    public EntityTypeModel[] AllowedEntityTypes { get; }

    public RelationshipTemplateModel(RelationshipTemplate template, Memoizer? memo = null)
        : base(template)
    {
        EntityType = template.EntityType.Memo(memo, () => new EntityTypeModel(template.EntityType, memo));
        PropertyEntityType = template.PropertyEntityType?.Memo(memo, () => new EntityTypeModel(template.PropertyEntityType, memo));
        RelationshipType = template.RelationshipType.Memo(memo, () => new RelationshipTypeModel(template.RelationshipType));
        var allowed = template.GetAllAllowedEntityTypes();
        AllowedEntityTypes = allowed.Memo(memo, () => allowed
            .Select(t => t.Memo(memo, () => new EntityTypeModel(t, memo)))
            .ToArray());
    }
}
