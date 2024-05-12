using Microsoft.EntityFrameworkCore;

using Juniper.Cedrus.Entities;
using Juniper.Cedrus.Controllers.V1;

namespace Juniper.Cedrus.Models;

public partial class CedrusContextSecure
{
    public ClassificationLevel UnclassLevel => SetClassificationLevel("U", "Unclassified");
    public ClassificationLevel CUILevel => SetClassificationLevel("CUI", "Controlled Unclassified Information");
    public ClassificationLevel SecretLevel => SetClassificationLevel("S", "Secret");
    public ClassificationLevel TSLevel => SetClassificationLevel("TS", "Top Secret");
    public ClassificationLevel TS_SCILevel => SetClassificationLevel("TS/SCI", "Top Secret/Sensitive Compartmented Information");

    public IQueryable<ClassificationLevel> GetClassificationLevels(CedrusUser user)
    {
        var parts = GetClassificationParts(user);
        return from level in insecure.ClassificationLevels
               where parts.Levels.Contains(level.Id)
               select level;
    }

    public Task<ClassificationLevel?> FindClassificationLevelAsync(string name, CedrusUser user) =>
        GetClassificationLevels(user)
            .SingleOrDefaultAsync(l => l.Name == name);

    public Task<ClassificationLevel?> FindClassificationLevelAsync(int id, CedrusUser user) =>
        GetClassificationLevels(user)
            .SingleOrDefaultAsync(l => l.Id == id);

    public async Task<ClassificationLevel> GetClassificationLevelAsync(IDOrName input, CedrusUser user)
    {
        if(input.Id is not null)
        {
            return await GetClassificationLevelAsync(input.Id.Value, user);
        }
        else if(input.Name is not null)
        {
            return await GetClassificationLevelAsync(input.Name, user);
        }

        throw new ArgumentException("Input does not specify a searchable classification level", nameof(input));
    }

    public async Task<ClassificationLevel> GetClassificationLevelAsync(string name, CedrusUser user) =>
        await FindClassificationLevelAsync(name, user)
        ?? throw new FileNotFoundException();

    public async Task<ClassificationLevel> GetClassificationLevelAsync(int id, CedrusUser user) =>
        await FindClassificationLevelAsync(id, user)
        ?? throw new FileNotFoundException();

    public ClassificationLevel SetClassificationLevel(string name, string description) =>
        insecure.ClassificationLevels.Upsert(
            ValidateString(nameof(name), name),
            () => new ClassificationLevel
            {
                Name = name,
                Description = description
            },
            value =>
            {
                value.Description = description;
            }
        );

    public void DeleteClassificationLevel(ClassificationLevel classLevel)
    {
        insecure.ClassificationLevels.Remove(classLevel);
    }
}
