using System.IO;

using Juniper.HTTP;

namespace Juniper.IO
{
    public interface ICacheLayer
    {
        bool CanCache { get; }

        bool IsCached<MediaTypeT>(IContentReference<MediaTypeT> source)
            where MediaTypeT : MediaType;

        Stream OpenWrite<MediaTypeT>(IContentReference<MediaTypeT> source)
            where MediaTypeT : MediaType;

        void Copy<MediaTypeT>(IContentReference<MediaTypeT> source, FileInfo file)
            where MediaTypeT : MediaType;

        Stream Cache<MediaTypeT>(IContentReference<MediaTypeT> source, Stream stream)
            where MediaTypeT : MediaType;

        IStreamSource<MediaTypeT> GetCachedSource<MediaTypeT>(IContentReference<MediaTypeT> source)
            where MediaTypeT : MediaType;
    }
}
