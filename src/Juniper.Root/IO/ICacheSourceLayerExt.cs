using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

using Juniper.Progress;

namespace Juniper.IO
{

    public static class ICacheSourceLayerExt
    {
        public static Task<Stream> OpenAsync(
            this ICacheSourceLayer layer,
            ContentReference fileRef)
        {
            if (layer is null)
            {
                throw new ArgumentNullException(nameof(layer));
            }

            if (fileRef is null)
            {
                throw new ArgumentNullException(nameof(fileRef));
            }

            return layer.GetStreamAsync(fileRef, null);
        }

        public static async Task<ResultT> LoadAsync<ResultT>(
            this ICacheSourceLayer layer,
            IDeserializer<ResultT> deserializer,
            ContentReference fileRef,
            IProgress prog = null)
        {
            if (fileRef is null)
            {
                throw new ArgumentNullException(nameof(fileRef));
            }

            if (layer is null)
            {
                throw new ArgumentNullException(nameof(layer));
            }

            if (deserializer is null)
            {
                throw new ArgumentNullException(nameof(deserializer));
            }

            var progs = prog.Split("Read", "Decode");
            var stream = await layer
                .GetStreamAsync(fileRef, progs[0])
                .ConfigureAwait(false);
            if (stream is null)
            {
                return default;
            }
            else
            {
                return deserializer.Deserialize(stream, progs[1]);
            }
        }
    }
}
