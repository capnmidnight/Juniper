using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;

using Juniper.Progress;

namespace Juniper.IO
{
    public interface ICacheSourceLayer
    {
        bool IsCached(ContentReference fileRef);

        Task<Stream> GetStreamAsync(ContentReference fileRef, IProgress prog);

        IEnumerable<ContentReference> GetContentReference(MediaType ofType);
    }
}
