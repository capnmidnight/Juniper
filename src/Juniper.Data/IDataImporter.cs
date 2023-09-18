using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Juniper.Data;

public interface IDataImporter<DbContextT>
    where DbContextT : DbContext

{
    void Import(DbContextT db, FileInfo file, ILogger logger);
}