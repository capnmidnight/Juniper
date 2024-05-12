using Microsoft.EntityFrameworkCore;

using Juniper.Cedrus.Entities;
using Juniper.Cedrus.Controllers.V1;

namespace Juniper.Cedrus.Models;

public partial class CedrusContextSecure
{
    private IQueryable<Relationship> FullRelationships(DateTime? context = null)
    {
        context ??= DateTime.Now;
        return insecure.Relationships
            .Include(r => r.Parent)
                .ThenInclude(p => p.Properties.Where(pp => pp.Start <= context && context < pp.End))
                    .ThenInclude(pv => pv.Type)
            .Include(r => r.Child)
                .ThenInclude(c => c.Properties.Where(cp => cp.Start <= context && context < cp.End));
    }

    public IQueryable<Relationship> CurrentRelationships(ClassificationParts parts, DateTime? context = null) =>
        FullRelationships().Current<RelationshipType, Relationship>(parts, context);

    public IQueryable<Relationship> HistoricRelationships(ClassificationParts parts, DateTime? context = null) =>
        FullRelationships().Historic<RelationshipType, Relationship>(parts, context);

    public IQueryable<Relationship> GetRelationships(CedrusUser user)
    {
        var parts = GetClassificationParts(user);
        return insecure.Relationships.Current<RelationshipType, Relationship>(parts);
    }

    public async Task<Relationship> GetRelationshipAsync(int relationshipId, CedrusUser user) =>
        await GetRelationships(user).SingleOrDefaultAsync(rel => rel.Id == relationshipId)
        ?? throw new FileNotFoundException();

    public Relationship SetRelationship(Entity parent, Entity child, CedrusUser user, Classification? classification = null, DateTime? startDate = null, DateTime? endDate = null) =>
        SetRelationship(DefaultRelationshipType, parent, child, user, classification, startDate, endDate);

    public Relationship SetRelationship(RelationshipType type, Entity parent, Entity child, CedrusUser user, Classification? classification = null, DateTime? startDate = null, DateTime? endDate = null) =>
        insecure.Relationships.TimeSeriesSplit(
            type,
            r => r.ParentId == parent.Id && r.ChildId == child.Id && r.TypeId == type.Id,
            r => r.Parent == parent && r.Child == child && r.Type == type,
            (_, __) => true,
            (type, start, end) => new Relationship
            {
                Parent = parent,
                Child = child,
                Type = type,
                Classification = classification ?? U,
                User = user,
                Start = start,
                End = end
            },
            (type, here, start, end) => new Relationship
            {
                Parent = parent,
                Child = child,
                Type = type,
                Classification = classification ?? here.Classification,
                User = user,
                Start = start,
                End = end
            },
            startDate,
            endDate
        );

    public void EndRelationship(RelationshipType type, Entity parent, Entity child, ClassificationParts parts)
    {
        var relationships = from r in insecure.Relationships.Current<RelationshipType, Relationship>(parts)
                            where r.ParentId == parent.Id
                                && r.ChildId == child.Id
                                && r.TypeId == type.Id
                            select r;

        foreach (var rel in relationships)
        {
            EndRelationship(rel);
        }
    }

    public void EndRelationship(Relationship rel) =>
        rel.End = DateTime.Now;
}
