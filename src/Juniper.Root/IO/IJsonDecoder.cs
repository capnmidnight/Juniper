namespace Juniper.IO
{
    public interface IJsonDecoder<T> : IDeserializer<T>, IContentHandler<MediaType.Application>
    { }
}