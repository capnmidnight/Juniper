using Juniper.Cedrus.Entities;

namespace Juniper.Cedrus.Controllers.V1;

public class PropertyTemplateModel : AbstractNamedModel
{
    public string TypeStamp => "propertyTemplate";
    public EntityTypeModel EntityType { get; }
    public PropertyTypeModel[] PropertyTypes { get; }

    public PropertyTemplateModel(PropertyTemplate template, Memoizer? memo = null)
        : base(template)
    {
        EntityType = template.EntityType.Memo(memo, () => new EntityTypeModel(template.EntityType, memo));
        PropertyTypes = template.PropertyTypes.Memo(memo, () => template
            .PropertyTypes
            .Select(t => t.Memo(memo, () => new PropertyTypeModel(t)))
            .ToArray());
    }
}
