using Juniper.HTTP;

namespace Juniper.Serialization
{
    public interface IFactory : ISerializer, IDeserializer
    {
    }

    public interface IFactory<T> : ISerializer<T>, IDeserializer<T>
    {
    }
}