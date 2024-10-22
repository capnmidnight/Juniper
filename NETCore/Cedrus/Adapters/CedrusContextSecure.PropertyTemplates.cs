using Juniper.Cedrus.Entities;
using Juniper.Data;

using Microsoft.EntityFrameworkCore;

namespace Juniper.Cedrus.Models;

public partial class CedrusContextSecure
{
    public IQueryable<PropertyTemplate> PropertyTemplates => insecure.PropertyTemplates;

    [SeedValue]
    public PropertyTemplate ReferenceEntityTemplate => Lazy(() => SetPropertyTemplateAsync(
        ReferenceEntityType, 
        "Reference", 
        NamePropertyType,
        AuthorsPropertyType,
        PublicationDateType,
        ReferenceFilePropertyType,
        ReferenceLinkPropertyType
    ));

    /// <summary>
    /// Create a new template, or use an existing one.
    /// </summary>
    /// <param name="name">The name to call this template.</param>
    /// <returns></returns>
    public Task<PropertyTemplate> SetPropertyTemplateAsync(EntityType type, string name, params PropertyType[] props) =>
        insecure.PropertyTemplates.UpsertAsync(
            template => template.EntityType.Name == type.Name && template.Name == name,
            () => new PropertyTemplate
            {
                Name = name,
                EntityType = type,
                PropertyTypes = props.ToList()
            },
            value =>
            {
                value.PropertyTypes.Sync(props, pt => pt.Name);
            }
        );

    public async Task<IQueryable<PropertyTemplate>> GetPropertyTemplatesAsync(int? entityTypeId = null)
    {
        if (entityTypeId is null)
        {
            return PropertyTemplates;
        }
        else
        {
            var entityTypeIds = await GetEntityTypeIdChain(entityTypeId.Value);

            return from template in insecure.PropertyTemplates
                   where entityTypeIds.Contains(template.EntityTypeId)
                   select template;
        }
    }

    public async Task<PropertyTemplate?> FindPropertyTemplateAsync(EntityType entityType, string name) =>
        insecure.PropertyTemplates
            .Local
            .SingleOrDefault(pt =>
                pt.Id == 0
                && pt.EntityType.Name == entityType.Name
                && pt.Name == name)
        ?? await insecure.PropertyTemplates
            .SingleOrDefaultAsync(pt =>
                pt.EntityTypeId == entityType.Id
                && pt.Name == name);

    public async Task<PropertyTemplate> GetPropertyTemplateAsync(int templateId) =>
        await PropertyTemplates.SingleOrDefaultAsync(t => t.Id == templateId)
        ?? throw new FileNotFoundException($"PropertyTemplate:{templateId}");

    public void DeletePropertyTemplate(PropertyTemplate template) =>
        insecure.PropertyTemplates.Remove(template);
}
