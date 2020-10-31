using System;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Juniper.HTTP
{
    public class DbFileStreamHandler : IDisposable
    {
        private readonly Action<DbCommand> makeCommand;
        private DbConnection conn;
        private DbCommand cmd;
        private DbDataReader reader;
        private bool disposedValue;

        public DbFileStreamHandler(DatabaseFacade db, Action<DbCommand> makeCommand)
        {
            conn = db.GetDbConnection();
            this.makeCommand = makeCommand;
        }

        public async Task<Stream> GetStreamAsync()
        {
            await conn.OpenAsync().ConfigureAwait(false);

            cmd = conn.CreateCommand();
            makeCommand(cmd);

            reader = await cmd.ExecuteReaderAsync(
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

            return reader.GetStream(0);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (reader is object)
                    {
                        reader.Dispose();
                        reader = null;
                    }

                    if (cmd is object)
                    {
                        cmd.Dispose();
                        cmd = null;
                    }

                    if (conn is object)
                    {
                        conn.Dispose();
                        conn = null;
                    }
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
