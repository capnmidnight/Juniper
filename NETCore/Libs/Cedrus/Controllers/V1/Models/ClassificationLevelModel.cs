using Juniper.Cedrus.Entities;

namespace Juniper.Cedrus.Controllers.V1;

public class ClassificationLevelModel
{
    public string TypeStamp => "classificationLevel";
    public int? Id { get; }
    public string? Name { get; }
    public string? Description { get; }

    public ClassificationLevelModel(ClassificationLevel? level)
    {
        Id = level?.Id;
        Name = level?.Name;
        Description = level?.Description;
    }
}

