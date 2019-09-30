using System.IO;
using System.Net;
using System.Threading.Tasks;

using Juniper.HTTP;
using Juniper.HTTP.REST;
using Juniper.Progress;

namespace Juniper.Serialization
{
    public interface ICacheLayer
    {
        bool IsCached(string fileDescriptor, MediaType contentType);

        Task<Stream> GetStream(string fileDescriptor, MediaType contentType, IProgress prog);

        Stream WrapStream(string fileDescriptor, MediaType contentType, Stream stream);
    }

    public static class ICacheLayerExt
    {
        public static Task<Stream> GetStream(this ICacheLayer layer, string fileDescriptor, MediaType contentType)
        {
            return layer.GetStream(fileDescriptor, contentType, null);
        }
    }
}
