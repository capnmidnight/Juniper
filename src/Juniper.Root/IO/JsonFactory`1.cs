using System.IO;

using Juniper.Progress;

using Newtonsoft.Json;

namespace Juniper.IO
{
    public class JsonFactory<T> : JsonFactory<T, MediaType.Application>, IJsonDecoder<T>
    {
        public JsonFactory() : base(MediaType.Application.Json)
        { }
    }
}