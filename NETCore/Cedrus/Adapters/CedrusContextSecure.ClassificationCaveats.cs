using Juniper.Cedrus.Controllers.V1;
using Juniper.Cedrus.Entities;

using Microsoft.EntityFrameworkCore;

namespace Juniper.Cedrus.Models;

public partial class CedrusContextSecure
{
    public IQueryable<ClassificationCaveat> GetClassificationCaveats(CedrusUser user)
    {
        var parts = GetClassificationParts(user);
        return from c in insecure.ClassificationCaveats
                        .Include(cv => cv.Level)
               where parts.Levels.Contains(c.LevelId)
               select c;
    }

    public Task<ClassificationCaveat?> FindClassificationCaveatAsync(string name, CedrusUser user) =>
        GetClassificationCaveats(user)
            .SingleOrDefaultAsync(l => l.Name == name);

    public async Task<ClassificationCaveat[]> GetClassificationCaveatsAsync(IDOrName[] input, CedrusUser user)
    {
        var ids = (from c in input
                   where c.Id is not null
                   select c.Id!.Value).ToArray();

        var names = (from c in input
                     where c.Name is not null
                     select c.Name).ToArray();

        var caveats = await (from c in GetClassificationCaveats(user)
                      where ids.Contains(c.Id)
                        || names.Contains(c.Name)
                      select c).ToArrayAsync();

        if(caveats.Length != input.Length)
        {
            throw new ArgumentException("Input does not specify a searchable caveat", nameof(input));
        }

        return caveats;
    }

    public async Task<ClassificationCaveat> GetClassificationCaveatAsync(int classCaveatId, CedrusUser user) =>
        await GetClassificationCaveats(user)
            .SingleOrDefaultAsync(l => l.Id == classCaveatId)
        ?? throw new FileNotFoundException();

    public async Task<ClassificationCaveat> GetClassificationCaveatAsync(string name, CedrusUser user) =>
        await FindClassificationCaveatAsync(name, user)
        ?? throw new FileNotFoundException();

    public ClassificationCaveat SetClassificationCaveat(ClassificationLevel level, string name, string description) =>
        insecure.ClassificationCaveats.Upsert(
            ValidateString(nameof(name), name),
            () => new ClassificationCaveat
            {
                Name = name,
                Description = description,
                Level = level
            },
            value =>
            {
                value.Description = description;
                value.Level = level;
            }
        );

    public void DeleteClassificationCaveat(ClassificationCaveat classCaveat)
    {
        insecure.ClassificationCaveats.Remove(classCaveat);
    }
}
