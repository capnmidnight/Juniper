using System.IO;
using System.Threading.Tasks;

using Juniper.Progress;

namespace Juniper.IO
{
    public static class ICacheDestinationLayerExt
    {
        public static async Task CopyToAsync(
            this ICacheSourceLayer fromLayer,
            ContentReference fromRef,
            ICacheDestinationLayer toLayer,
            ContentReference toRef,
            bool overwrite = false,
            IProgress prog = null)
        {
            if (fromLayer.IsCached(fromRef)
                && (overwrite || !toLayer.IsCached(toRef)))
            {
                using (var inStream = await fromLayer
                    .GetStreamAsync(fromRef, prog)
                    .ConfigureAwait(false))
                using (var outStream = toLayer.Create(toRef, overwrite))
                {
                    await inStream
                        .CopyToAsync(outStream)
                        .ConfigureAwait(false);
                }
            }
        }

        public static void Save<ResultType>(
            this ICacheDestinationLayer layer,
            ISerializer<ResultType> serializer,
            ContentReference fileRef,
            ResultType value,
            bool overwrite = false,
            IProgress prog = null)
        {
            using (var stream = layer.Create(fileRef, overwrite))
            {
                serializer.Serialize(stream, value, prog);
            }
        }
    }
}
