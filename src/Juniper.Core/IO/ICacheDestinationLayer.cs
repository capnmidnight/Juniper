using System.IO;

namespace Juniper.IO
{

    public interface ICacheDestinationLayer : ICacheSourceLayer
    {
        bool CanCache<MediaTypeT>(IContentReference<MediaTypeT> fileRef)
            where MediaTypeT : MediaType;

        Stream Create<MediaTypeT>(IContentReference<MediaTypeT> fileRef)
            where MediaTypeT : MediaType;

        Stream Cache<MediaTypeT>(IContentReference<MediaTypeT> fileRef, Stream stream)
            where MediaTypeT : MediaType;
    }

    public static class ICacheLayerExt
    {
        public static void Copy<MediaTypeT>(this ICacheDestinationLayer layer, IContentReference<MediaTypeT> fileRef, FileInfo file)
            where MediaTypeT : MediaType
        {
            if (file.Exists && layer.CanCache(fileRef))
            {
                var mem = layer.Create(fileRef);
                using (var stream = file.Open(FileMode.Open, FileAccess.Read))
                {
                    stream.CopyTo(mem);
                }
            }
        }

    }
}
