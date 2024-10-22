using Juniper.Progress;

namespace Juniper.Caching;

public abstract class AbstractStreamSource : ContentReference
{
    protected AbstractStreamSource(MediaType contentType)
        : base(contentType)
    { }

    protected AbstractStreamSource(string cacheID, MediaType contentType)
        : base(cacheID, contentType)
    { }

    public sealed override string? CacheID =>
        InternalCacheID;

    public sealed override bool DoNotCache =>
        CacheID is null;

    protected abstract string? InternalCacheID { get; }

    public abstract Task<Stream> GetStreamAsync(IProgress? prog = null);
}
