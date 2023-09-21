using Juniper.Data;

using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Logging;

using System.Data.Common;

namespace Juniper.HTTP;

/// <summary>
/// When using Entity Framework in ASP.NET Core, this ActionResult can be used to retrieve and stream
/// a single BLOB to the Response body.
/// </summary>
public class DbFileResult : FileInfoResult
{
    private readonly DbRawQuery query;

    /// <summary>
    /// Creates a new ActionResult that can perform an ADO.NET query against a database and stream the results
    /// to the client through the request body.
    /// </summary>
    /// <param name="db">The Entity Framework database context through which the query will be made.</param>
    /// <param name="size">The size of the file that will be sent. This should be retrieved separately.</param>
    /// <param name="contentType">The content type of the file that will be sent. This should be retrieved separately.</param>
    /// <param name="fileName">The name of the file that will be sent. This should be retrieved separately.</param>
    /// <param name="cacheTime">The number of seconds to tell the client to cache the result.</param>
    /// <param name="makeCommand">A callback function to construct the Command that will perform the query to retrieve the file stream.</param>
    /// <param name="range">A range request expression.</param>
    public DbFileResult(DatabaseFacade db, long size, string contentType, string? fileName, int cacheTime, string? range, ILogger logger, Action<DbCommand> makeCommand)
        : base(size, contentType, fileName, cacheTime, range, logger)
    {
        query = new DbRawQuery(db, makeCommand);
    }

    /// <summary>
    /// Performs the stream operation.
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    protected override async Task WriteBody(HttpResponse response)
    { 
        var cancellationToken = response.HttpContext.RequestAborted;
        var body = response.Body;
        if (hasRange)
        {
            await query.WriteToAsync(body, rangeStart, rangeEnd, cancellationToken);
        }
        else
        {
            await query.WriteToAsync(body, cancellationToken);
        }
    }
}
