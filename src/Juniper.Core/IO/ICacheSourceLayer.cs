using System.Collections.Generic;
using System.IO;

using Juniper.Progress;

namespace Juniper.IO
{
    public interface ICacheSourceLayer
    {
        bool IsCached(ContentReference fileRef);

        Stream Open(ContentReference fileRef, IProgress prog);

        IEnumerable<ContentReference> Get<MediaTypeT>(MediaTypeT ofType)
            where MediaTypeT : MediaType;
    }

    public static class ICacheSourceLayerExt
    {
        public static Stream Open(this ICacheSourceLayer layer, ContentReference fileRef)
        {
            return layer.Open(fileRef, null);
        }
    }
}
