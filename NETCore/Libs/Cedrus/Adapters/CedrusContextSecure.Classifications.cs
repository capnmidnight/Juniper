// Ignore Spelling: Cedrus CUI

using Juniper.Cedrus.Controllers.V1;
using Juniper.Cedrus.Entities;

using Microsoft.EntityFrameworkCore;

namespace Juniper.Cedrus.Models;

public partial class CedrusContextSecure
{
    public Classification U => SetClassification(UnclassLevel);
    public Classification CUI => SetClassification(U, CUILevel);
    public Classification S => SetClassification(CUI, SecretLevel);
    public Classification TS => SetClassification(S, TSLevel);
    public Classification TS_SCI => SetClassification(TS, TS_SCILevel);

    private ClassificationParts GetClassificationParts(CedrusUser user)
    {
        var classification = insecure
            .Classifications
            .Include(c => c.Parent)
            .AsEnumerable()
            .SingleOrDefault(c => c.Id == user.ClassificationId);

        var classes = classification?.GetChain() ?? [];

        var levels = (from c in classes
                      select c.LevelId)
                    .Distinct()
                    .ToArray();

        var caveats = (from c in classes
                       from cv in c.Caveats
                       select cv.Id)
                       .Distinct()
                       .ToArray();

        return new ClassificationParts(levels, caveats);
    }

    public IQueryable<Classification> GetClassifications(CedrusUser user, ClassificationLevel? level = null)
    {
        var parts = GetClassificationParts(user);
        return from c in insecure.Classifications
               where parts.Levels.Contains(c.LevelId)
                && (level == null || c.Level.Name == level.Name)
               select c;
    }

    public Task<Classification?> FindClassificationAsync(string levelName, CedrusUser user) =>
        GetClassifications(user)
            .SingleOrDefaultAsync(c => c.Level.Name == levelName
                && c.Caveats.Count == 0);

    public async Task<Classification> GetClassificationAsync(int classId, CedrusUser user) =>
        await GetClassifications(user).SingleOrDefaultAsync(c => c.Id == classId)
        ?? throw new FileNotFoundException();

    public Classification GetClassification(string fullName, CedrusUser user) =>
        GetClassifications(user)
            .AsEnumerable()
            .FirstOrDefault(c => c.Name == fullName)
            ?? throw new FileNotFoundException();

    public async Task<Classification?> GetClassificationAsync(IDOrName? input, CedrusUser user)
    {
        if(input is null)
        {
            return null;
        }
        else if (input.Id is not null)
        {
            return await GetClassificationAsync(input.Id.Value, user);
        }
        else if (input.Name is not null)
        {
            return GetClassification(input.Name, user);
        }
        else
        {
            throw new ArgumentException("Input does not specify a searchable classification", nameof(input));
        }
    }

    public Classification SetClassification(ClassificationLevel level, params ClassificationCaveat[] caveats) =>
        SetClassificationAsync(null, level, caveats).Result;

    public Task<Classification> SetClassificationAsync(ClassificationLevel level, params ClassificationCaveat[] caveats) =>
        SetClassificationAsync(null, level, caveats);

    public Classification SetClassification(Classification parent, ClassificationLevel level) =>
        SetClassificationAsync(parent, level).Result;

    private Classification SetClassification(ClassificationLevel level) =>
        SetClassificationAsync(null, level).Result;

    private async Task<Classification> SetClassificationAsync(Classification? parent, ClassificationLevel level, params ClassificationCaveat[] caveats)
    {
        if (parent is not null && caveats.Length > 0)
        {
            throw new ClassificationException(level, "Classification with caveats must be created from level and caveats, not parent, level and caveats");
        }

        if (caveats.Length > 0)
        {
            parent = await (from c in insecure.Classifications.Include(c => c.Parent)
                            where c.LevelId == level.Id
                              && c.Caveats.Count == 0
                            select c).SingleOrDefaultAsync()
                        ?? (from c in insecure.Classifications.Local
                            where c.Level.Name == level.Name
                                && c.Caveats.Count == 0
                            select c).SingleOrDefault();

            if (parent is null)
            {
                throw new ClassificationException(level, "Could not find a parent classification for the given level");
            }

            var levels = parent
                .GetChain()
                .Select(p => p.Level.Name)
                .ToHashSet();

            if (caveats.Any(c => !levels.Contains(c.Level.Name)))
            {
                throw new ClassificationException(level, "Some caveats are not permitted in the current classification level");
            }
        }

        if (parent is not null && parent.Caveats.Count > 0)
        {
            throw new ClassificationException(level, "Classification parent must have no caveats");
        }

        var caveatNames = caveats.Select(c => c.Name).ToArray();
        var classification = await (from c in insecure.Classifications
                                    where c.LevelId == level.Id
                                      && c.Caveats.Count == caveats.Length
                                      && c.Caveats.All(c => caveatNames.Contains(c.Name))
                                    select c).SingleOrDefaultAsync()
                                ?? (from c in insecure.Classifications.Local
                                    where c.Level.Name == level.Name
                                        && c.Caveats.Count == caveats.Length
                                        && c.Caveats.All(c => caveatNames.Contains(c.Name))
                                    select c).SingleOrDefault();

        if (classification is null)
        {
            await insecure.Classifications.AddAsync(classification = new Classification()
            {
                Parent = parent,
                Level = level,
                Caveats = caveats
            });
        }

        return classification;
    }

    public void DeleteClassification(Classification classification)
    {
        insecure.Classifications.Remove(classification);
    }
}
