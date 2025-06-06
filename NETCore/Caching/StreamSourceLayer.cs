using Juniper.IO;
using Juniper.Progress;

namespace Juniper.Caching;

public class StreamSourceLayer : ICacheSourceLayer
{
    private readonly CachingStrategy parent;

    public StreamSourceLayer(CachingStrategy parent)
    {
        this.parent = parent;
    }

    public IEnumerable<ContentReference> GetContentReferences(MediaType ofType)
    {
        return Array.Empty<ContentReference>();
    }

    public async Task<Stream?> GetStreamAsync(ContentReference fileRef, IProgress? prog)
    {
        if (fileRef is null)
        {
            throw new ArgumentNullException(nameof(fileRef));
        }

        if (fileRef is not AbstractStreamSource streamSource)
        {
            throw new InvalidOperationException("This layer can only retrieve data from stream sources.");
        }

        var stream = await streamSource
            .GetStreamAsync(prog)
            .ConfigureAwait(false);

        if (parent.CanCache(fileRef))
        {
            return new CachingStream(stream, parent.Create(fileRef));
        }

        return stream;
    }

    public bool IsCached(ContentReference fileRef)
    {
        return fileRef is AbstractStreamSource;
    }
}
