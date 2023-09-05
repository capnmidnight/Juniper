using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;

using System.Net;
using System.Text.RegularExpressions;

namespace Juniper.HTTP;

public class FileInfoResult : IActionResult
{
    private readonly string contentType;
    private readonly string fileName;
    private readonly int cacheTime;
    private readonly ILogger? logger;
    protected readonly long size;

    protected readonly bool hasRange;
    protected readonly long rangeStart;
    protected readonly long rangeEnd;
    protected long RangeLength => Math.Min(rangeEnd, size) - rangeStart;

    private static readonly Regex rangePattern = new(@"^bytes=(\d*)=(\d*)$", RegexOptions.Compiled);

    /// <summary>
    /// Creates a new ActionResult that sends along the file metadata.
    /// </summary>
    /// <param name="db">The Entity Framework database context through which the query will be made.</param>
    /// <param name="size">The size of the file that will be sent. This should be retrieved separately.</param>
    /// <param name="contentType">The content type of the file that will be sent. This should be retrieved separately.</param>
    /// <param name="fileName">The name of the file that will be sent. This should be retrieved separately.</param>
    /// <param name="cacheTime">The number of seconds to tell the client to cache the result.</param>
    /// <param name="range">A range request expression.</param>
    public FileInfoResult(long size, string contentType, string? fileName, int cacheTime, string range, ILogger? logger = null)
    {
        var type = MediaType.Parse(contentType);
        this.contentType = contentType;
        this.fileName = (fileName ?? "download").AddExtension(type);
        this.cacheTime = cacheTime;
        this.logger = logger;
        this.size = size;

        hasRange = !string.IsNullOrEmpty(range);
        if (hasRange)
        {
            var rangeMatch = rangePattern.Match(range);
            var rangeStart = rangeMatch.Groups[1].Value;
            var rangeEnd = rangeMatch.Groups[2].Value;
            this.rangeStart = rangeStart.Length > 0 ? long.Parse(rangeStart) : 0;
            this.rangeEnd = rangeEnd.Length > 0 ? long.Parse(rangeEnd) : size;
        }
    }

    /// <summary>
    /// Performs the stream operation.
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public async Task ExecuteResultAsync(ActionContext context)
    {
        if (context is null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        var response = context.HttpContext.Response;
        response.StatusCode = StatusCodes.Status200OK;
        response.ContentType = contentType;
        response.ContentLength = hasRange ? RangeLength : size;
        if (hasRange)
        {
            response.Headers[HeaderNames.AcceptRanges] = "bytes";
            response.Headers[HeaderNames.ContentRange] = $"bytes {rangeStart}-{rangeEnd}/{size}";
        }

        if (!string.IsNullOrEmpty(fileName))
        {
            response.Headers[HeaderNames.ContentDisposition] = $"attachment; filename=\"{WebUtility.UrlEncode(fileName)}\"";
        }

        if (cacheTime > 0)
        {
            response.Headers[HeaderNames.CacheControl] = $"public,max-age={cacheTime}";
        }

        try
        {
            await WriteBody(response);
        }
        catch (OperationCanceledException)
        {

        }
        catch (Exception exp)
        {
            logger?.LogError(exp, "Download cancelled: {path}", context.HttpContext.Request.Path);
        }
    }

    protected virtual Task WriteBody(HttpResponse response)
    {
        return Task.CompletedTask;
    }
}