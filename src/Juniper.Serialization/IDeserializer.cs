namespace Juniper.Serialization
{
    public interface IDeserializer
    {
        bool TryDeserialize<T>(string text, out T value);
    }
}
