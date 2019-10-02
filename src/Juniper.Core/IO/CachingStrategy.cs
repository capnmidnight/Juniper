using System.Collections.Generic;
using System.IO;
using Juniper.HTTP;

namespace Juniper.IO
{
    public class CachingStrategy : ICacheLayer
    {
        private readonly List<ICacheLayer> layers = new List<ICacheLayer>();

        public CachingStrategy AddLayer(ICacheLayer layer)
        {
            layers.Add(layer);
            return this;
        }

        public Stream Cache<MediaTypeT>(IContentReference<MediaTypeT> source, Stream stream)
            where MediaTypeT : MediaType
        {
            if (stream != null)
            {
                foreach (var layer in layers)
                {
                    if (layer.CanCache && !layer.IsCached(source))
                    {
                        stream = layer.Cache(source, stream);
                    }
                }
            }

            return stream;
        }

        public bool CanCache
        {
            get
            {
                foreach (var layer in layers)
                {
                    if (layer.CanCache)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        public Stream OpenWrite<MediaTypeT>(IContentReference<MediaTypeT> source)
            where MediaTypeT : MediaType
        {
            var stream = new ForkedStream();
            foreach (var layer in layers)
            {
                if (layer.CanCache && !layer.IsCached(source))
                {
                    stream.AddStream(layer.OpenWrite(source));
                }
            }

            return stream;
        }

        public void Copy<MediaTypeT>(IContentReference<MediaTypeT> source, FileInfo file)
            where MediaTypeT : MediaType
        {
            foreach (var layer in layers)
            {
                if (layer.CanCache)
                {
                    layer.Copy(source, file);
                }
            }
        }

        public bool IsCached<MediaTypeT>(IContentReference<MediaTypeT> source)
            where MediaTypeT : MediaType
        {
            foreach (var layer in layers)
            {
                if (layer.IsCached(source))
                {
                    return true;
                }
            }

            return false;
        }

        public IStreamSource<MediaTypeT> GetStreamSource<MediaTypeT>(IContentReference<MediaTypeT> source)
            where MediaTypeT : MediaType
        {
            IStreamSource<MediaTypeT> cached = null;
            foreach (var layer in layers)
            {
                if (layer.IsCached(source))
                {
                    cached = layer.GetStreamSource(source);
                    break;
                }
            }

            return cached;
        }

        public IStreamSource<MediaTypeT> GetStreamSource<MediaTypeT>(IStreamSource<MediaTypeT> source)
            where MediaTypeT : MediaType
        {
            IStreamSource<MediaTypeT> cached = null;
            foreach (var layer in layers)
            {
                if (layer.IsCached(source))
                {
                    cached = layer.GetStreamSource(source);
                    break;
                }
            }

            return cached;
        }
    }
}
