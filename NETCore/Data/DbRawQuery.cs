using System.Data;
using System.Data.Common;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Juniper.Data;

public class DbRawQuery
{
    private readonly DatabaseFacade db;
    private readonly Action<DbCommand> makeCommand;

    /// <summary>
    /// Creates a new ActionResult that can perform an ADO.NET query against a database and stream the results
    /// to the client through the request body.
    /// </summary>
    /// <param name="db">The Entity Framework database context through which the query will be made.</param>
    /// <param name="makeCommand">A callback function to construct the Command that will perform the query to retrieve the file stream.</param>
    public DbRawQuery(DatabaseFacade db, Action<DbCommand> makeCommand)
    {
        this.db = db;
        this.makeCommand = makeCommand;
    }
    
    public Task WriteToAsync(Stream body) =>
        WriteToAsync(null, null, body, CancellationToken.None);

    public Task WriteToAsync(Stream body, CancellationToken cancellationToken) =>
        WriteToAsync(null, null, body, cancellationToken);

    public Task WriteToAsync(Stream body, long rangeStart, long rangeEnd) =>
        WriteToAsync(rangeStart, rangeEnd, body, CancellationToken.None);

    public Task WriteToAsync(Stream body, long rangeStart, long rangeEnd, CancellationToken cancellationToken) =>
        WriteToAsync(rangeStart, rangeEnd, body, cancellationToken);

    private async Task WriteToAsync(long? rangeStart, long? rangeEnd, Stream body, CancellationToken cancellationToken)
    {
        using var conn = db.GetDbConnection();

        await conn.OpenAsync(cancellationToken)
            .ConfigureAwait(false);

        using var cmd = conn.CreateCommand();
        makeCommand(cmd);

        using var reader = await cmd.ExecuteReaderAsync(
            CommandBehavior.SingleResult
            | CommandBehavior.SequentialAccess
            | CommandBehavior.CloseConnection,
            cancellationToken)
            .ConfigureAwait(false);

        var read = await reader.ReadAsync(cancellationToken)
            .ConfigureAwait(false);

        if (!read)
        {
            throw new EndOfStreamException();
        }

        using var stream = reader.GetStream(0);
        await stream.WriteToAsync(rangeStart, rangeEnd, body, cancellationToken);
    }
}
