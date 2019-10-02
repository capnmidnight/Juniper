using Juniper.HTTP;

namespace Juniper.IO
{
    public class ContentReference<MediaTypeT> : IContentReference<MediaTypeT>
        where MediaTypeT : MediaType
    {
        public ContentReference(string fileName, MediaTypeT contentType)
        {
            CacheID = fileName;
            ContentType = contentType;
        }

        public string CacheID
        {
            get;
        }

        public MediaTypeT ContentType
        {
            get;
        }
    }
}
