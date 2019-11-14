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

        IEnumerable<ContentReference> Get(MediaType ofType);
    }

    public static class ICacheSourceLayerExt
    {
        public static Task<Stream> Open(
            this ICacheSourceLayer layer,
            ContentReference fileRef)
        {
            return layer.Open(fileRef, null);
        }

        public static async Task<ResultType> Load<ResultType>(
            this ICacheSourceLayer layer,
            IDeserializer<ResultType> deserializer,
            ContentReference fileRef,
            IProgress prog = null)
        {
            if (fileRef == null)
            {
                throw new ArgumentNullException(nameof(fileRef));
            }

            if (deserializer == null)
            {
                throw new ArgumentNullException(nameof(deserializer));
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

        public static bool TryLoad<ResultType>(
            this ICacheSourceLayer layer,
            IDeserializer<ResultType> deserializer,
            ContentReference fileRef,
            out ResultType value,
            IProgress prog = null)
        {
            var task = layer.Load(deserializer, fileRef, prog);

            try
            {
                task.Wait();
            }
            catch
            { }
            finally
            {
                if (task.IsSuccessful())
                {
                    value = task.Result;
                }
                else
                {
                    value = default;
                }
            }

            return task.IsSuccessful();
        }

        public static async Task Proxy(
            this ICacheSourceLayer layer,
            HttpListenerResponse response,
            ContentReference fileRef)
        {
            var stream = await layer.Open(fileRef, null);
            await stream.Proxy(response);
        }

        public static Task Proxy(
            this ICacheSourceLayer layer,
            HttpListenerContext context,
            ContentReference fileRef)
        {
            return layer.Proxy(context.Response, fileRef);
        }
    }
}
