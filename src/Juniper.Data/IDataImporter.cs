using System.ComponentModel;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Juniper.Data;

public interface IDataImporter<DbContextT>
    where DbContextT : DbContext

{
    void Import(DbContextT db, FileInfo file, ILogger logger);
}

public static class DbContextTExtensions
{
    private static Dictionary<DbContext, Dictionary<IListSource, List<object>>> contextCaches = new();
    public static EntityT Upsert<DbContextT, EntityT>(this DbSet<EntityT> set, DbContextT db, Func<EntityT, bool> filter, Func<EntityT> create)
        where DbContextT : DbContext
        where EntityT : class
    {
        if(!contextCaches.ContainsKey(db))
        {
            contextCaches.Add(db, new Dictionary<IListSource, List<object>>());
        }

        var contextCache = contextCaches[db];
        if (!contextCache.ContainsKey(set))
        {
            contextCache.Add(set, new List<object>());
        }

        var setCache = contextCache[set];

        var value = set.SingleOrDefault(filter);

        if (value is null)
        {
            value = setCache.SingleOrDefault(obj => obj is EntityT ent && filter(ent)) as EntityT;
            if (value is null)
            {
                value = set.Add(create()).Entity;
                setCache.Add(value);
            }
            else
            {

            }
        }

        return value;
    }


    public static EntityT? FindCached<DbContextT, EntityT>(this DbSet<EntityT> set, DbContextT db, Func<EntityT, bool> filter)
        where DbContextT : DbContext
        where EntityT : class
    {
        if (!contextCaches.ContainsKey(db))
        {
            return null;
        }

        var contextCache = contextCaches[db];
        if (!contextCache.ContainsKey(set))
        {
            return null;
        }

        var setCache = contextCache[set];

        var value = set.FirstOrDefault(filter);

        if (value is null)
        {
            value = setCache.FirstOrDefault(obj => obj is EntityT ent && filter(ent)) as EntityT;
        }

        return value;
    }
}