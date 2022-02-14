namespace Juniper.IO
{
    public class JsonFactory<T> :
        JsonFactory<T, MediaType.Application>,
        IJsonFactory<T>,
        IJsonDecoder<T>
    {
        public JsonFactory() : base(MediaType.Application_Json)
        { }
    }
}