using System.IO;

namespace Juniper.IO
{
    public interface ICacheLayer
    {
        bool CanCache<MediaTypeT>(IContentReference<MediaTypeT> fileRef)
            where MediaTypeT : MediaType;

        bool IsCached<MediaTypeT>(IContentReference<MediaTypeT> fileRef)
            where MediaTypeT : MediaType;

        Stream OpenWrite<MediaTypeT>(IContentReference<MediaTypeT> fileRef)
            where MediaTypeT : MediaType;

        void Copy<MediaTypeT>(IContentReference<MediaTypeT> fileRef, FileInfo file)
            where MediaTypeT : MediaType;

        Stream Cache<MediaTypeT>(IContentReference<MediaTypeT> fileRef, Stream stream)
            where MediaTypeT : MediaType;

        IStreamSource<MediaTypeT> GetCachedSource<MediaTypeT>(IContentReference<MediaTypeT> fileRef)
            where MediaTypeT : MediaType;
    }
}
