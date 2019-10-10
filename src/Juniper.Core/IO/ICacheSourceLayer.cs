using System.IO;

namespace Juniper.IO
{
    public interface ICacheSourceLayer
    {
        bool IsCached<MediaTypeT>(IContentReference<MediaTypeT> fileRef)
            where MediaTypeT : MediaType;

        Stream Open<MediaTypeT>(IContentReference<MediaTypeT> fileRef)
            where MediaTypeT : MediaType;
    }
}
