using Juniper.Cedrus.Entities;

namespace Juniper.Cedrus.Controllers.V1;

public class ClassificationCaveatModel
{
    public string TypeStamp => "classificationCaveat";
    public int? Id { get; }
    public string? Name { get; }
    public string? Description { get; }
    public ClassificationLevelModel Level { get; }

    public ClassificationCaveatModel(ClassificationCaveat? caveat)
    {
        Id = caveat?.Id;
        Name = caveat?.Name;
        Description = caveat?.Description;
        Level = new ClassificationLevelModel(caveat?.Level);
    }
}

