namespace Juniper.IO
{
    public interface IFactory : ISerializer, IDeserializer
    {
    }

    public interface ITextDecoder : ISerializer, IDeserializer
    {
        MediaType ContentType { get; }
    }

    public interface IFactory<T> : ISerializer<T>, IDeserializer<T>
    {
    }

    public interface ITextDecoder<T> : ISerializer<T>, IDeserializer<T>
    {
        MediaType ContentType { get; }
    }
}