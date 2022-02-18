using Juniper.IO;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

using System.Data;
using System.Data.Common;

namespace Juniper.HTTP
{
    /// <summary>
    /// When using Entity Framework in ASP.NET Core, this ActionResult can be used to retrieve and stream
    /// a single BLOB to the Response body.
    /// </summary>
    public class DbFileStream : AbstractWrappedStream
    {
        private DbConnection conn;
        private DbCommand cmd;
        private DbDataReader reader;
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
        public DbFileStream(DatabaseFacade db, Action<DbCommand> makeCommand)
            : base()
        {
            this.db = db;
            this.makeCommand = makeCommand;
        }

        protected override Stream GetStream()
        {
            conn = db.GetDbConnection();
            conn.Open();

            cmd = conn.CreateCommand();
            makeCommand(cmd);

            reader = cmd.ExecuteReader(
                CommandBehavior.SingleResult
                | CommandBehavior.SequentialAccess
                | CommandBehavior.CloseConnection);

            var read = reader.Read();
            if (!read)
            {
                throw new EndOfStreamException();
            }
            return reader.GetStream(0);
        }

       
        public override void Close()
        {
            base.Close();
            conn.Close();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                reader.Dispose();
                cmd.Dispose();
                conn.Dispose();
            }
        }

        public override async ValueTask DisposeAsync()
        {
            await base.DisposeAsync();
            await reader.DisposeAsync();
            await cmd.DisposeAsync();
            await conn.DisposeAsync();
        }
    }
}
