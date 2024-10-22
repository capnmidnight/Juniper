using Juniper.Cedrus.Entities;
using Juniper.Data;

using Microsoft.AspNetCore.Builder;

namespace Juniper.Cedrus;

public static class WebApplicationExt 
{ 
    public static WebApplication MigrateCedrus(this WebApplication app, bool seedDB) =>
        app
            .MigrateDatabase<CedrusContextInsecure>()
            .SeedData<CedrusContextInsecure>(seedDB);
}
