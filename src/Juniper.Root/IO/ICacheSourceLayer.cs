using Juniper.Progress;

using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Juniper.IO
{
    public interface ICacheSourceLayer
    {
        bool IsCached(ContentReference fileRef);

        Task<Stream> GetStreamAsync(ContentReference fileRef, IProgress prog);

        IEnumerable<ContentReference> GetContentReferences(MediaType ofType);
    }
}
