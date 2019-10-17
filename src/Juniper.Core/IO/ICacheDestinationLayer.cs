using System.IO;

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

    public static class ICacheLayerExt
    {
        public static void CopyTo(
            this ICacheSourceLayer fromLayer,
            ICacheDestinationLayer toLayer,
            ContentReference fromRef,
            ContentReference toRef,
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
            ContentReference fromRef,
            ContentReference toRef,
            bool overwrite)
        {
            fromLayer.CopyTo(toLayer, fromRef, toRef, overwrite, null);
        }
        public static void CopyTo(
            this ICacheSourceLayer fromLayer,
            ICacheDestinationLayer toLayer,
            ContentReference fromRef,
            ContentReference toRef,
            IProgress prog)
        {
            fromLayer.CopyTo(toLayer, fromRef, toRef, false, prog);
        }

        public static void CopyTo(
            this ICacheSourceLayer fromLayer,
            ICacheDestinationLayer toLayer,
            ContentReference fromRef,
            ContentReference toRef)
        {
            fromLayer.CopyTo(toLayer, fromRef, toRef, false, null);
        }
    }
}
