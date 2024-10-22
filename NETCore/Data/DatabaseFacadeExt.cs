using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Logging;

namespace Juniper.Data;

public static class DatabaseFacadeExt
{
    public static void AutoMigrate(this DatabaseFacade database, ILogger? logger = null)
    {
        var pending = database.GetPendingMigrations();
        if (pending.Any())
        {
            logger?.LogInformation("Pending migrations: {migrations}", string.Join(", ", pending));
        }
        else
        {
            logger?.LogInformation("No pending migrations");
        }

        try
        {
            database.Migrate();
        }
        catch (Exception ex)
        {
            logger?.LogError(ex, "Error during migration");
        }
    }
}