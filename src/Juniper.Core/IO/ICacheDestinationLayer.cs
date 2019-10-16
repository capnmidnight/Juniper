using System.IO;

using Juniper.Progress;

namespace Juniper.IO
{

    public interface ICacheDestinationLayer : ICacheSourceLayer
    {
        bool CanCache(IContentReference fileRef);

        Stream Create(IContentReference fileRef, bool overwrite);

        Stream Cache(IContentReference fileRef, Stream stream);

        bool Delete(IContentReference fileRef);
    }

    public static class ICacheLayerExt
    {
        public static void CopyTo(
            this ICacheSourceLayer fromLayer,
            ICacheDestinationLayer toLayer,
            IContentReference fromRef,
            IContentReference toRef,
            bool overwrite,
            IProgress prog)
        {
            if (fromLayer.IsCached(fromRef)
                && (overwrite || !toLayer.IsCached(toRef)))
            {
                using (var inStream = fromLayer.Open(fromRef, prog))
                using (var outStream = toLayer.Create(toRef, overwrite))
                {
                    inStream.CopyTo(outStream);
                }
            }
        }
        public static void CopyTo(
            this ICacheSourceLayer fromLayer,
            ICacheDestinationLayer toLayer,
            IContentReference fromRef,
            IContentReference toRef,
            bool overwrite)
        {
            fromLayer.CopyTo(toLayer, fromRef, toRef, overwrite, null);
        }
        public static void CopyTo(
            this ICacheSourceLayer fromLayer,
            ICacheDestinationLayer toLayer,
            IContentReference fromRef,
            IContentReference toRef,
            IProgress prog)
        {
            fromLayer.CopyTo(toLayer, fromRef, toRef, false, prog);
        }

        public static void CopyTo(
            this ICacheSourceLayer fromLayer,
            ICacheDestinationLayer toLayer,
            IContentReference fromRef,
            IContentReference toRef)
        {
            fromLayer.CopyTo(toLayer, fromRef, toRef, false, null);
        }

        public static IContentReference ToRef(this string cacheID, MediaType contentType)
        {
            return new ContentReference(cacheID, contentType);
        }
    }
}
