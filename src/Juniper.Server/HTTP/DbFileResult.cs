using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

using System.Data;
using System.Data.Common;
using System.Net;

namespace Juniper.HTTP
{
    /// <summary>
    /// When using Entity Framework in ASP.NET Core, this ActionResult can be used to retrieve and stream
    /// a single BLOB to the Response body.
    /// </summary>
    public class DbFileResult : IActionResult
    {
        private readonly string contentType;
        private readonly string fileName;
        private readonly int cacheTime;
        private readonly DatabaseFacade db;
        private readonly long size;
        private readonly Action<DbCommand> makeCommand;

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
        public DbFileResult(DatabaseFacade db, long size, string contentType, string fileName, int cacheTime, Action<DbCommand> makeCommand)
        {
            var type = MediaType.Parse(contentType);
            this.contentType = contentType;
            this.fileName = fileName?.AddExtension(type);
            this.cacheTime = cacheTime;
            this.db = db;
            this.size = size;
            this.makeCommand = makeCommand;
        }

        public DbFileResult(DatabaseFacade db, long size, string contentType, string fileName, Action<DbCommand> makeCommand)
            : this(db, size, contentType, fileName, 0, makeCommand)
        { }

        /// <summary>
        /// Performs the stream operation.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public virtual async Task ExecuteResultAsync(ActionContext context)
        {
            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var response = context.HttpContext.Response;
            try
            {
                using var conn = db.GetDbConnection();
                await conn.OpenAsync()
                    .ConfigureAwait(false);

                using var cmd = conn.CreateCommand();
                makeCommand(cmd);

                using var reader = await cmd.ExecuteReaderAsync(
                    CommandBehavior.SingleResult
                    | CommandBehavior.SequentialAccess
                    | CommandBehavior.CloseConnection)
                    .ConfigureAwait(false);

                var read = await reader.ReadAsync()
                    .ConfigureAwait(false);

                if (!read)
                {
                    throw new EndOfStreamException();
                }

                using var stream = reader.GetStream(0);

                response.StatusCode = (int)HttpStatusCode.OK;
                response.ContentType = contentType;
                response.ContentLength = size;
                if (!string.IsNullOrEmpty(fileName))
                {
                    response.Headers["Content-Disposition"] = $"attachment; filename=\"{WebUtility.UrlEncode(fileName)}\"";
                }

                if (cacheTime > 0)
                {
                    response.Headers["Cache-Control"] = $"public,max-age={cacheTime}";
                }

                await stream.CopyToAsync(response.Body)
                    .ConfigureAwait(false);
            }
            catch (Exception)
            {
                response.StatusCode = (int)HttpStatusCode.NotFound;
            }
        }
    }
}
