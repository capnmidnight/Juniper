using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Juniper.HTTP;

/// <summary>
/// When a raw file can be read off disk and sent directly to the user, this ActionResult can be used
/// to stream it directly, rather than loading it all into memory first.
/// </summary>
public class StreamFileResult : FileInfoResult
{
    private readonly FileInfo file;

    public StreamFileResult(FileInfo file, string? fileName, int cacheTime, string? range, ILogger logger)
        : this(
            file,
            null,
            fileName,
            cacheTime,
            range,
            logger)
    {
    }

    public StreamFileResult(FileInfo file, string? etag, string? fileName, int cacheTime, string? range, ILogger logger)
        : base(
            file.Length,
            MediaType.GuessByFileName(file.Name).FirstOrDefault()
              ?? MediaType.Application_Octet_Stream,
            etag,
            fileName,
            cacheTime,
            range,
            logger)
    {
        this.file = file;
    }

    protected override async Task WriteBody(HttpResponse response)
    {
        var cancellationToken = response.HttpContext.RequestAborted;
        var body = response.Body;
        using var stream = file.OpenRead();
        if (hasRange)
        {
            await stream.WriteToAsync(body, rangeStart, rangeEnd, cancellationToken);
        }
        else
        {
            await stream.WriteToAsync(body, cancellationToken);
        }
    }
}
