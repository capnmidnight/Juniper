using Juniper.Data;

namespace Juniper.Cedrus.Controllers.V1;

public abstract class AbstractNamedModel : AbstractSequencedModel
{
    public string Name { get; }

    protected AbstractNamedModel(INamed obj)
        : base(obj)
    {
        Name = obj.Name;
    }

    protected AbstractNamedModel(int id, string name)
        : base(id)
    {
        Name = name;
    }
}
