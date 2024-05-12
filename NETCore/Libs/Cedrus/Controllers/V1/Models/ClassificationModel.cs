using Juniper.Cedrus.Entities;

namespace Juniper.Cedrus.Controllers.V1;

public class ClassificationModel
{
    public string TypeStamp => "classification";
    public int? Id { get; }
    public string? Name { get; }
    public string? Description { get; }
    public string? CaveatString { get; }

    public ClassificationLevelModel Level { get; }

    public ClassificationCaveatModel[] Caveats { get; }

    public ClassificationModel(Classification? classification)
    {
        Id = classification?.Id;
        Name = classification?.Name;
        Description = classification?.Description;
        CaveatString = classification?.CaveatsString;
        Level = new ClassificationLevelModel(classification?.Level);
        Caveats = classification
            ?.Caveats
            ?.Select(c => new ClassificationCaveatModel(c))
            ?.ToArray()
            ?? Array.Empty<ClassificationCaveatModel>();
    }
}
