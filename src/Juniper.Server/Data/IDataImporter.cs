using Microsoft.EntityFrameworkCore;

namespace Juniper.Data;

public interface IDataImporter<DbContextT>
    where DbContextT : DbContext

{
    void Prepare(DbContextT db);
    void Import(FileInfo file);
}