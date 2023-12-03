using Juniper.IO;
using Juniper.Progress;

namespace Juniper.Caching;

public class FileCacheLayer : ICacheDestinationLayer
{
    /// <summary>
    /// Creates a caching layer that uses the user's temp directory.
    /// </summary>
    /// <returns></returns>
    public static ICacheDestinationLayer GetTempCache()
    {
        return new FileCacheLayer(Path.GetTempPath());
    }

    private readonly DirectoryInfo cacheLocation;

    public FileCacheLayer(DirectoryInfo cacheLocation)
    {
        this.cacheLocation = cacheLocation;
    }

    public FileCacheLayer(string directoryName)
        : this(new DirectoryInfo(directoryName))
    { }

    public virtual bool CanCache(ContentReference fileRef)
    {
        if (fileRef is null)
        {
            throw new ArgumentNullException(nameof(fileRef));
        }

        return !fileRef.DoNotCache;
    }

    /// <summary>
    /// Converts a ContentReference to a FileInfo as if the file exists
    /// in this cache layer. Check FileInfo.Exists to make sure the file
    /// actually exists.
    /// </summary>
    /// <param name="fileRef"></param>
    /// <returns></returns>
    public FileInfo Resolve(ContentReference fileRef)
    {
        return cacheLocation + fileRef;
    }

    public bool IsCached(ContentReference fileRef)
    {
        var file = Resolve(fileRef);
        return file.Exists;
    }

    public Stream Create(ContentReference fileRef)
    {
        if (fileRef is null)
        {
            throw new ArgumentNullException(nameof(fileRef));
        }

        if (!CanCache(fileRef))
        {
            throw new ArgumentException("Cannot cache this file.", fileRef.ToString());
        }

        var file = Resolve(fileRef);
        file.Directory?.Create();
        return file.Create();
    }

    public Task<Stream?> GetStreamAsync(ContentReference fileRef, IProgress? prog = null)
    {
        if (fileRef is null)
        {
            throw new ArgumentNullException(nameof(fileRef));
        }

        if (!IsCached(fileRef))
        {
            throw new FileNotFoundException("File not found in disk cache!", fileRef.ToString());
        }

        var file = Resolve(fileRef);
        var stream = file.OpenRead();
        var progStream = new ProgressStream(stream, file.Length, prog, true);
        return Task.FromResult((Stream?)progStream);
    }

    public IEnumerable<ContentReference> GetContentReferences(MediaType ofType)
    {
        if (ofType is null)
        {
            throw new ArgumentNullException(nameof(ofType));
        }

        foreach (var ext in ofType.Extensions)
        {
            foreach (var file in cacheLocation.GetFiles($"*.{ext}"))
            {
                var shortName = file.Name;
                var cacheID = PathExt.RemoveShortExtension(shortName);
                yield return new ContentReference(cacheID, ofType);
            }
        }
    }

    public bool Delete(ContentReference fileRef)
    {
        var file = Resolve(fileRef);
        return file.TryDelete();
    }
}
