using Juniper.Cedrus.Entities;

namespace Juniper.Cedrus.Controllers.V1;

public abstract class AbstractClassificationMarked : AbstractSequencedModel
{
    public ClassificationModel Classification { get; }

    protected AbstractClassificationMarked(IClassificationMarked? obj)
        : base(obj)
    {
        Classification = new ClassificationModel(obj?.Classification);
    }
}
