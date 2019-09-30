using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Juniper.HTTP;
using Juniper.HTTP.REST;
using Juniper.Progress;

namespace Juniper.Serialization
{
    public class CachingStrategy : ICacheLayer
    {
        private readonly List<ICacheLayer> layers = new List<ICacheLayer>();

        public CachingStrategy AddLayer(ICacheLayer layer)
        {
            layers.Add(layer);
            return this;
        }

        public Stream WrapStream(string fileDescriptor, MediaType contentType, Stream stream)
        {
            foreach (var layer in layers)
            {
                stream = layer.WrapStream(fileDescriptor, contentType, stream);
            }

            return stream;
        }

        public bool IsCached(string fileDescriptor, MediaType contentType)
        {
            foreach(var layer in layers)
            {
                if (layer.IsCached(fileDescriptor, contentType))
                {
                    return true;
                }
            }

            return false;
        }

        private async Task<Stream> GetStream(
            IEnumerable<ICacheLayer> forwardLayers,
            string fileDescriptor,
            MediaType contentType,
            IProgress prog)
        {
            var backwardLayers = forwardLayers.Reverse();
            Stream stream = null;
            foreach (var layer in forwardLayers)
            {
                stream = await layer.GetStream(fileDescriptor, contentType, prog);
                if (stream != null)
                {
                    break;
                }
            }

            foreach (var layer in backwardLayers)
            {
                if (!layer.IsCached(fileDescriptor, contentType))
                {
                    stream = layer.WrapStream(fileDescriptor, contentType, stream);
                }
            }

            return stream;
        }

        public Task<Stream> GetStream(string fileDescriptor, MediaType contentType, IProgress prog)
        {
            return GetStream(layers, fileDescriptor, contentType, prog);
        }

        public Task<Stream> GetStream(AbstractRequest request, IProgress prog)
        {
            return GetStream(layers.Append(request), request.CacheID, request.ContentType, prog);
        }

        public async Task<T> GetDecoded<T>(AbstractRequest request, IDeserializer<T> decoder, IProgress prog)
        {
            var split = prog.Split("Get", "Decode");
            using (var stream = await GetStream(request, split[0]))
            {
                return decoder.Deserialize(stream, split[1]);
            }
        }

        public bool IsCached(AbstractRequest request)
        {
            return IsCached(request.CacheID, request.ContentType);
        }

        public Task<Stream> GetStream(AbstractRequest request)
        {
            return GetStream(request, null);
        }

        public Task<T> GetDecoded<T>(AbstractRequest request, IDeserializer<T> decoder)
        {
            return GetDecoded(request, decoder, null);
        }
    }
}
