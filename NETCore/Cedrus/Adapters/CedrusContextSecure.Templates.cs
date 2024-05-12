using Juniper.Cedrus.Controllers.V1;
using Juniper.Cedrus.Entities;

using Microsoft.EntityFrameworkCore;
namespace Juniper.Cedrus.Models;

public partial class CedrusContextSecure
{
    public IQueryable<Template> Templates => insecure.Templates;

    /// <summary>
    /// Create a new template, or use an existing one.
    /// </summary>
    /// <param name="name">The name to call this template.</param>
    /// <returns></returns>
    public Template SetTemplate(string name, EntityType type, params PropertyType[] props) =>
        insecure.Templates.Upsert(
            name,
            () => new Template
            {
                Name = name,
                EntityType = type,
                PropertyTypes = props
            },
            value =>
            {
                value.EntityType = type;

                foreach (var propType in value.PropertyTypes)
                {
                    propType.Templates.Remove(value);
                }

                value.PropertyTypes.Clear();

                foreach (var prop in props)
                {
                    value.PropertyTypes.Add(prop);
                    prop.Templates.Add(value);
                }
            }
        );

    public IQueryable<Template> GetTemplates(int? entityTypeId = null) =>
        from temp in Templates
        where entityTypeId == null || temp.EntityTypeId == entityTypeId
        select temp;

    public async Task<Template> GetTemplateAsync(int templateId) =>
        await Templates.SingleOrDefaultAsync(t => t.Id == templateId)
        ?? throw new FileNotFoundException();

    public void DeleteTemplate(Template template) =>
        insecure.Templates.Remove(template);
}
