using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;

namespace Juniper.Data;

public static class WebApplicationExtensions
{
    public static WebApplication MigrateDatabase<DbContextT>(this WebApplication app) 
        where DbContextT : DbContext
    {
        app.Services.MigrateDatabase<DbContextT>();
        return app;
    }

    public static WebApplication SeedData<DbContextT>(this WebApplication app, bool seedDB)
        where DbContextT : DbContext
    {
        if (seedDB)
        {
            app.Services.SeedData<DbContextT>(app.Configuration);
        }
        return app;
    }
}
