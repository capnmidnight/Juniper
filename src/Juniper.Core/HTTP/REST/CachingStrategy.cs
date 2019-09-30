using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Juniper.Progress;
using Juniper.Serialization;

namespace Juniper.HTTP.REST
{
    public class CachingStrategy : ICacheLayer
    {
        private readonly List<ICacheLayer> layers = new List<ICacheLayer>();

        public CachingStrategy AddLayer(ICacheLayer layer)
        {
            layers.Add(layer);
            return this;
        }

        public Stream WrapStream(AbstractRequest request, Stream stream)
        {
            foreach (var layer in layers)
            {
                stream = layer.WrapStream(request, stream);
            }

            return stream;
        }

        public bool IsCached(AbstractRequest request)
        {
            foreach(var layer in layers)
            {
                if (layer.IsCached(request))
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<Stream> GetStream(AbstractRequest request, IProgress prog)
        {
            var forwardLayers = layers.Append(request);
            var backwardLayers = forwardLayers.Reverse();
            Stream stream = null;
            foreach(var layer in forwardLayers)
            {
                stream = await layer.GetStream(request, prog);
                if(stream != null)
                {
                    break;
                }
            }

            foreach(var layer in backwardLayers)
            {
                if (!layer.IsCached(request))
                {
                    stream = layer.WrapStream(request, stream);
                }
            }

            return stream;
        }
    }
}
