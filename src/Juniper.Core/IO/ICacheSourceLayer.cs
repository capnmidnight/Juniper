using System.Collections.Generic;
using System.IO;

using Juniper.Progress;

namespace Juniper.IO
{
    public interface ICacheSourceLayer
    {
        bool IsCached(IContentReference fileRef);

        Stream Open(IContentReference fileRef, IProgress prog);

        IEnumerable<IContentReference> Get<MediaTypeT>(MediaTypeT ofType)
            where MediaTypeT : MediaType;
    }

    public static class ICacheSourceLayerExt
    {
        public static Stream Open(this ICacheSourceLayer layer, IContentReference fileRef)
        {
            return layer.Open(fileRef, null);
        }
    }
}
