using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using System;
using System.Data.Common;
using System.Net;
using System.Threading.Tasks;

namespace Juniper.HTTP.Server
{
    /// <summary>
    /// When using Entity Framework in ASP.NET Core, this ActionResult can be used to retrieve and stream
    /// a single BLOB to the Response body.
    /// </summary>
    public class DbFileResult : IActionResult
    {
        private readonly DbContext db;
        private readonly long size;
        private readonly string contentType;
        private readonly string fileName;
        private readonly Action<DbCommand> makeCommand;

        /// <summary>
        /// Creates a new ActionResult that can perform an ADO.NET query against a database and stream the results
        /// to the client through the request body.
        /// </summary>
        /// <param name="db">The Entity Framework database context through which the query will be made.</param>
        /// <param name="size">The size of the file that will be sent. This should be retrieved separately.</param>
        /// <param name="contentType">The content type of the file that will be sent. This should be retrieved separately.</param>
        /// <param name="fileName">The name of the file that will be sent. This should be retrieved separately.</param>
        /// <param name="makeCommand">A callback function to construct the Command that will perform the query to retrieve the file stream.</param>
        public DbFileResult(DbContext db, long size, string contentType, string fileName, Action<DbCommand> makeCommand)
        {
            this.db = db;
            this.size = size;
            this.contentType = contentType;
            this.fileName = fileName;
            this.makeCommand = makeCommand;
        }

        /// <summary>
        /// Performs the database query.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task ExecuteResultAsync(ActionContext context)
        {
            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            using var conn = db.Database.GetDbConnection();
            await conn.OpenAsync().ConfigureAwait(false);

            using var cmd = conn.CreateCommand();
            makeCommand(cmd);

            using var reader = await cmd.ExecuteReaderAsync(
                System.Data.CommandBehavior.SingleResult
                | System.Data.CommandBehavior.SequentialAccess)
                .ConfigureAwait(false);
            var read = await reader.ReadAsync()
                .ConfigureAwait(false);

            var response = context.HttpContext.Response;
            if (!read)
            {
                response.StatusCode = (int)HttpStatusCode.NotFound;
            }
            else
            {
                response.ContentType = contentType;
                response.ContentLength = size;
                response.Headers["Content-Disposition"] = $"attachment; filename=\"{fileName}\"";

                using var stream = reader.GetStream(0);
                await stream.CopyToAsync(context.HttpContext.Response.Body)
                    .ConfigureAwait(false);
            }
        }
    }
}
