namespace Juniper.Data;

public interface INamed : ISequenced
{
    string Name { get; set; }
}
