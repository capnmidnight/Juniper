using Juniper.Data;

namespace Juniper.Cedrus.Controllers.V1;

public abstract class AbstractSequencedModel
{
    public int Id { get; }

    protected AbstractSequencedModel(ISequenced obj)
    {
        Id = obj.Id;
    }

    protected AbstractSequencedModel(int id)
    {
        Id = id;
    }
}
