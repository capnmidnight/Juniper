namespace Juniper.Cedrus.Entities;

public interface INamed : ISequenced
{
    string Name { get; set; }
}
