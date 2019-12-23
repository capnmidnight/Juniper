using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;

using Juniper.Progress;

namespace Juniper.IO
{

    public static class ICacheSourceLayerExt
    {
        public static Task<Stream> Open(
            this ICacheSourceLayer layer,
            ContentReference fileRef)
        {
            return layer.GetStreamAsync(fileRef, null);
        }

        public static async Task<ResultT> Load<ResultT>(
            this ICacheSourceLayer layer,
            IDeserializer<ResultT> deserializer,
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
            var stream = await layer
                .GetStreamAsync(fileRef, progs[0])
                .ConfigureAwait(false);
            if (stream == null)
            {
                return default;
            }
            else
            {
                return deserializer.Deserialize(stream, progs[1]);
            }
        }

        public static bool TryLoad<ResultT>(
            this ICacheSourceLayer layer,
            IDeserializer<ResultT> deserializer,
            ContentReference fileRef,
            out ResultT value,
            IProgress prog = null)
            where ResultT : class
        {
            value = default;

            var task = layer.Load(deserializer, fileRef, prog);
            Task.WaitAny(task);

            if (task.IsSuccessful())
            {
                value = task.Result;
            }

            return value != default;
        }

        public static async Task Proxy(
            this ICacheSourceLayer layer,
            HttpListenerResponse response,
            ContentReference fileRef)
        {
            var stream = await layer
                .GetStreamAsync(fileRef, null)
                .ConfigureAwait(false);
            await stream
                .Proxy(response)
                .ConfigureAwait(false);
        }

        public static Task Proxy(
            this ICacheSourceLayer layer,
            HttpListenerContext context,
            ContentReference fileRef)
        {
            return layer.Proxy(context.Response, fileRef);
        }

        /// <summary>
        /// Retrieve all the content references that can be deserialized by the
        /// given deserializer.
        /// </summary>
        /// <typeparam name="ResultT"></typeparam>
        /// <typeparam name="MediaTypeT"></typeparam>
        /// <param name="deserializer"></param>
        /// <returns></returns>
        public static IEnumerable<(ContentReference contentRef, ResultT result)> Get<ResultT, MediaTypeT>(this ICacheSourceLayer source, IFactory<ResultT, MediaTypeT> deserializer)
            where ResultT : class
            where MediaTypeT : MediaType
        {
            foreach (var contentRef in source.GetContentReference(deserializer.ContentType))
            {
                if (source.TryLoad(deserializer, contentRef, out var result))
                {
                    yield return (contentRef, result);
                }
            }
        }
    }
}
