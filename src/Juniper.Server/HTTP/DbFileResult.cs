using System.Buffers;
using System.Data;
using System.Data.Common;

using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Juniper.HTTP
{
    /// <summary>
    /// When using Entity Framework in ASP.NET Core, this ActionResult can be used to retrieve and stream
    /// a single BLOB to the Response body.
    /// </summary>
    public class DbFileResult : FileInfoResult
    {
        private readonly DatabaseFacade db;
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
        /// <param name="range">A range request expression.</param>
        public DbFileResult(DatabaseFacade db, long size, string contentType, string fileName, int cacheTime, string range, Action<DbCommand> makeCommand)
            : base(size, contentType, fileName, cacheTime, range)
        {
            this.db = db;
            this.makeCommand = makeCommand;
        }

        /// <summary>
        /// Performs the stream operation.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        protected override async Task WriteBody(HttpResponse response)
        {
            using var conn = db.GetDbConnection();

            await conn.OpenAsync(response.HttpContext.RequestAborted)
                .ConfigureAwait(false);

            using var cmd = conn.CreateCommand();
            makeCommand(cmd);

            using var reader = await cmd.ExecuteReaderAsync(
                CommandBehavior.SingleResult
                | CommandBehavior.SequentialAccess
                | CommandBehavior.CloseConnection,
                response.HttpContext.RequestAborted)
                .ConfigureAwait(false);

            var read = await reader.ReadAsync(response.HttpContext.RequestAborted)
                .ConfigureAwait(false);

            if (!read)
            {
                throw new EndOfStreamException();
            }

            using var stream = reader.GetStream(0);
            if (hasRange)
            {
                const int FRAME_SIZE = 64 * 1024;
                if (rangeStart > 0)
                {
                    using var mem = new PooledMemory(FRAME_SIZE);
                    var toBurn = rangeStart;
                    while (toBurn > 0)
                    {
                        var shouldBurn = (int)Math.Min(toBurn, FRAME_SIZE);
                        var wasBurned = await stream.ReadAsync(mem.Mem[..shouldBurn], response.HttpContext.RequestAborted);
                        toBurn -= wasBurned;
                    }
                }

                await StreamCopyOperation.CopyToAsync(stream, response.Body, RangeLength, FRAME_SIZE, response.HttpContext.RequestAborted);
            }
            else
            {
                await stream.CopyToAsync(response.Body, response.HttpContext.RequestAborted)
                    .ConfigureAwait(false);
            }
        }

        private class PooledMemory : IDisposable
        {
            private static readonly ArrayPool<byte> arrayPool = ArrayPool<byte>.Create();
            private readonly byte[] buffer;
            public Memory<byte> Mem { get; private set; }

            public PooledMemory(int size)
            {
                buffer = arrayPool.Rent(size);
                Mem = new Memory<byte>(buffer);
            }

            public void Dispose()
            {
                arrayPool.Return(buffer);
            }
        }
    }
}
