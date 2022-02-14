using Juniper.Progress;

using System;
using System.Threading.Tasks;

namespace Juniper.IO
{
    public static class ICacheDestinationLayerExt
    {
        public static async Task CopyToAsync(
            this ICacheSourceLayer fromLayer,
            ContentReference fromRef,
            ICacheDestinationLayer toLayer,
            ContentReference toRef,
            IProgress prog = null)
        {
            if (fromLayer is null)
            {
                throw new ArgumentNullException(nameof(fromLayer));
            }

            if (fromRef is null)
            {
                throw new ArgumentNullException(nameof(fromRef));
            }

            if (toLayer is null)
            {
                throw new ArgumentNullException(nameof(toLayer));
            }

            if (toRef is null)
            {
                throw new ArgumentNullException(nameof(toRef));
            }

            if (fromLayer.IsCached(fromRef))
            {
                using var inStream = await fromLayer
                    .GetStreamAsync(fromRef, prog)
                    .ConfigureAwait(false);
                using var outStream = toLayer.Create(toRef);
                await inStream
                    .CopyToAsync(outStream)
                    .ConfigureAwait(false);
            }
        }

        public static void Save<ResultType, M>(
            this ICacheDestinationLayer layer,
            ISerializer<ResultType, M> serializer,
            ContentReference fileRef,
            ResultType value)
            where M : MediaType
        {
            if (layer is null)
            {
                throw new ArgumentNullException(nameof(layer));
            }

            if (serializer is null)
            {
                throw new ArgumentNullException(nameof(serializer));
            }

            if (fileRef is null)
            {
                throw new ArgumentNullException(nameof(fileRef));
            }

            using var stream = layer.Create(fileRef);
            _ = serializer.Serialize(stream, value);
        }

        public static void SaveJson<ResultType>(
            this ICacheDestinationLayer layer,
            string fileName,
            ResultType value)
        {
            if (layer is null)
            {
                throw new ArgumentNullException(nameof(layer));
            }

            if (fileName is null)
            {
                throw new ArgumentNullException(nameof(fileName));
            }

            if (fileName.Length == 0)
            {
                throw new ArgumentException("File name must be more than 0 characters long");
            }

            var serializer = new JsonFactory<ResultType>();
            var fileRef = fileName + MediaType.Application_Json;
            using var stream = layer.Create(fileRef);
            _ = serializer.Serialize(stream, value);
        }
    }
}
