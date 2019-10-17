using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;

using Juniper.Progress;

namespace Juniper.IO
{
    public interface ICacheSourceLayer
    {
        bool IsCached(ContentReference fileRef);

        Task<Stream> Open(ContentReference fileRef, IProgress prog);

        IEnumerable<ContentReference> Get<MediaTypeT>(MediaTypeT ofType)
            where MediaTypeT : MediaType;
    }

    public static class ICacheSourceLayerExt
    {
        public static Task<Stream> Open(this ICacheSourceLayer layer, ContentReference fileRef)
        {
            return layer.Open(fileRef, null);
        }


        public static async Task<ResultType> Load<MediaTypeT, ResultType>(this ICacheSourceLayer layer, ContentReference fileRef, IDeserializer<ResultType, MediaTypeT> deserializer, IProgress prog)
            where MediaTypeT : MediaType
        {
            if (fileRef == null)
            {
                throw new ArgumentNullException(nameof(fileRef));
            }

            if (deserializer == null)
            {
                throw new ArgumentNullException(nameof(deserializer));
            }

            if (fileRef.ContentType != deserializer.ContentType)
            {
                throw new ArgumentException($"{nameof(fileRef)} parameter's content type ({fileRef.ContentType.Value}) does not match {nameof(deserializer)} parameter's content type ({deserializer.ContentType.Value})");
            }

            var progs = prog.Split("Read", "Decode");
            var stream = await layer.Open(fileRef, progs[0]);
            if (stream == null)
            {
                return default;
            }
            else
            {
                return deserializer.Deserialize(stream, progs[1]);
            }
        }

        public static Task<ResultType> Load<MediaTypeT, ResultType>(this ICacheSourceLayer layer, ContentReference fileRef, IDeserializer<ResultType, MediaTypeT> deserializer)
            where MediaTypeT : MediaType
        {
            return layer.Load(fileRef, deserializer, null);
        }

        public static Task<ResultType> Load<MediaTypeT, ResultType>(this ICacheSourceLayer layer, string cacheID, IDeserializer<ResultType, MediaTypeT> deserializer, IProgress prog)
            where MediaTypeT : MediaType
        {
            return layer.Load(cacheID + deserializer.ContentType, deserializer, prog);
        }

        public static Task<ResultType> Load<MediaTypeT, ResultType>(this ICacheSourceLayer layer, string cacheID, IDeserializer<ResultType, MediaTypeT> deserializer)
            where MediaTypeT : MediaType
        {
            return layer.Load(cacheID, deserializer, null);
        }

        public static async Task Proxy(this ICacheSourceLayer layer, ContentReference fileRef, HttpListenerResponse response)
        {
            var stream = await layer.Open(fileRef, null);
            await stream.Proxy(response);
        }

        public static Task Proxy<MediaTypeT>(this ICacheSourceLayer layer, ContentReference fileRef, HttpListenerContext context)
            where MediaTypeT : MediaType
        {
            return layer.Proxy(fileRef, context.Response);
        }
    }
}
