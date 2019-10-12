using System.IO;

namespace Juniper.IO
{

    public interface ICacheDestinationLayer : ICacheSourceLayer
    {
        bool CanCache<MediaTypeT>(IContentReference<MediaTypeT> fileRef)
            where MediaTypeT : MediaType;

        Stream Create<MediaTypeT>(IContentReference<MediaTypeT> fileRef, bool overwrite)
            where MediaTypeT : MediaType;

        Stream Cache<MediaTypeT>(IContentReference<MediaTypeT> fileRef, Stream stream)
            where MediaTypeT : MediaType;

        bool Delete<MediaTypeT>(IContentReference<MediaTypeT> fileRef)
            where MediaTypeT : MediaType;
    }

    public static class ICacheLayerExt
    {
        public static void CopyTo<MediaTypeT>(
            this ICacheSourceLayer fromLayer,
            ICacheDestinationLayer toLayer,
            IContentReference<MediaTypeT> fromRef,
            IContentReference<MediaTypeT> toRef,
            bool overwrite)
            where MediaTypeT : MediaType
        {
            if (fromLayer.IsCached(fromRef)
                && (overwrite || !toLayer.IsCached(toRef)))
            {
                using (var inStream = fromLayer.Open(fromRef))
                using (var outStream = toLayer.Create(toRef, overwrite))
                {
                    inStream.CopyTo(outStream);
                }
            }
        }

        public static void CopyTo<MediaTypeT>(
            this ICacheSourceLayer fromLayer,
            ICacheDestinationLayer toLayer,
            IContentReference<MediaTypeT> fromRef,
            IContentReference<MediaTypeT> toRef)
            where MediaTypeT : MediaType
        {
            fromLayer.CopyTo(toLayer, fromRef, toRef, false);
        }
    }
}
