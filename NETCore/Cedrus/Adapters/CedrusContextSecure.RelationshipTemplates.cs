using Juniper.Cedrus.Entities;
using Juniper.Data;

using Microsoft.EntityFrameworkCore;
namespace Juniper.Cedrus.Models;

public partial class CedrusContextSecure
{
    public IQueryable<RelationshipTemplate> RelationshipTemplates => insecure.RelationshipTemplates;

    /// <summary>
    /// Create a new template, or use an existing one.
    /// </summary>
    /// <param name="name">The name to call this template.</param>
    /// <returns></returns>
    public Task<RelationshipTemplate> SetRelationshipTemplateAsync(EntityType parentEntityType, string name, EntityType? propertyEntityType, RelationshipType relType, params EntityType[] allowedEntityTypes) =>
        insecure.RelationshipTemplates.UpsertAsync(
            template => template.EntityType.Name == parentEntityType.Name && template.Name == name,
            () => new RelationshipTemplate
            {
                Name = name,
                EntityType = parentEntityType,
                RelationshipType = relType,
                PropertyEntityType = propertyEntityType,
                AllowedEntityTypes = allowedEntityTypes.ToList()
            },
            value =>
            {
                value.RelationshipType = relType;
                value.PropertyEntityType = propertyEntityType;
                value.AllowedEntityTypes.Sync(allowedEntityTypes, et => et.Name);
            }
        );

    public async Task<IEnumerable<RelationshipTemplate>> GetRelationshipTemplatesAsync(int? entityTypeId = null)
    {
        if (entityTypeId is null)
        {
            return RelationshipTemplates;
        }
        else
        {
            var entityTypeIds = await GetEntityTypeIdChain(entityTypeId.Value);

            return from template in insecure.RelationshipTemplates
                    .Include(rt => rt.AllowedEntityTypes)
                        .ThenInclude(et => et.Children)
                   where entityTypeIds.Contains(template.EntityTypeId)
                   select template;
        }
    }

    public async Task<RelationshipTemplate> GetRelationshipTemplateAsync(int templateId) =>
        await RelationshipTemplates.SingleOrDefaultAsync(t => t.Id == templateId)
        ?? throw new FileNotFoundException($"RelationshipTemplate:{templateId}");

    public void DeleteRelationshipTemplate(RelationshipTemplate template) =>
        insecure.RelationshipTemplates.Remove(template);
}
