using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;

namespace Juniper.Data;

public static class WebApplicationExtensions
{
    public static WebApplication MigrateDatabase<T>(this WebApplication app) where T : DbContext
    {
        app.Services.MigrateDatabase<T>();
        return app;
    }
}
