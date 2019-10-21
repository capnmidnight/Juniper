using System.IO;
using System.Threading.Tasks;

using Juniper.Progress;

namespace Juniper.IO
{

    public interface ICacheDestinationLayer : ICacheSourceLayer
    {
        bool CanCache(ContentReference fileRef);

        Stream Create(ContentReference fileRef, bool overwrite);

        Stream Cache(ContentReference fileRef, Stream stream);

        bool Delete(ContentReference fileRef);
    }

    public static class ICacheDestinationLayerExt
    {
        public static async Task CopyTo(
            this ICacheSourceLayer fromLayer,
            ContentReference fromRef,
            ICacheDestinationLayer toLayer,
            ContentReference toRef,
            bool overwrite,
            IProgress prog)
        {
            if (fromLayer.IsCached(fromRef)
                && (overwrite || !toLayer.IsCached(toRef)))
            {
                using (var inStream = await fromLayer.Open(fromRef, prog))
                using (var outStream = toLayer.Create(toRef, overwrite))
                {
                    await inStream.CopyToAsync(outStream);
                }
            }
        }

        public static Task CopyTo(
            this ICacheSourceLayer fromLayer,
            ContentReference fromRef,
            ICacheDestinationLayer toLayer,
            ContentReference toRef,
            bool overwrite)
        {
            return fromLayer.CopyTo(fromRef, toLayer, toRef, overwrite, null);
        }

        public static Task CopyTo(
            this ICacheSourceLayer fromLayer,
            ContentReference fromRef,
            ICacheDestinationLayer toLayer,
            ContentReference toRef,
            IProgress prog)
        {
            return fromLayer.CopyTo(fromRef, toLayer, toRef, false, prog);
        }

        public static Task CopyTo(
            this ICacheSourceLayer fromLayer,
            ContentReference fromRef,
            ICacheDestinationLayer toLayer,
            ContentReference toRef)
        {
            return fromLayer.CopyTo(fromRef, toLayer, toRef, false, null);
        }

        public static void Save<ResultType>(
            this ICacheDestinationLayer layer,
            ISerializer<ResultType> serializer,
            ContentReference fileRef,
            ResultType value,
            IProgress prog)
        {
            using (var stream = layer.Create(fileRef, true))
            {
                serializer.Serialize(stream, value, prog);
            }
        }

        public static void Save<ResultType>(
            this ICacheDestinationLayer layer,
            ISerializer<ResultType> serializer,
            ContentReference fileRef,
            ResultType value)
        {
            layer.Save(serializer, fileRef, value, null);
        }
    }
}
