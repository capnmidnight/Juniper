using Juniper.Progress;

namespace Juniper.Caching;

public interface ICacheSourceLayer
{
    bool IsCached(ContentReference fileRef);

    Task<Stream?> GetStreamAsync(ContentReference fileRef, IProgress? prog);

    IEnumerable<ContentReference> GetContentReferences(MediaType ofType);
}
