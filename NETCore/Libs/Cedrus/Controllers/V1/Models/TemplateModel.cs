using Juniper.Cedrus.Entities;

namespace Juniper.Cedrus.Controllers.V1;

public class TemplateModel
{
    public string TypeStamp => "template";
    public int? Id { get; }
    public string? Name { get; }
    public EntityTypeModel EntityType { get; }
    public PropertyTypeModel[] PropertyTypes { get; }

    public TemplateModel(Template? template)
    {
        Id = template?.Id;
        Name = template?.Name;
        EntityType = new EntityTypeModel(template?.EntityType);
        PropertyTypes = template
            ?.PropertyTypes
            ?.Select(t => new PropertyTypeModel(t))
            ?.ToArray()
            ?? Array.Empty<PropertyTypeModel>();
    }
}