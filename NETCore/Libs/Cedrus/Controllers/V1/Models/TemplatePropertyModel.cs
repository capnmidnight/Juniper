using Juniper.Cedrus.Entities;

namespace Juniper.Cedrus.Controllers.V1;

public class TemplatePropertyModel
{
    public string TypeStamp => "templateProperty";
    public TemplateModel Template { get; }
    public PropertyTypeModel PropertyType { get; }

    public TemplatePropertyModel(Template? template, PropertyType? propType)
    {
        Template = new TemplateModel(template);
        PropertyType = new PropertyTypeModel(propType);
    }
}