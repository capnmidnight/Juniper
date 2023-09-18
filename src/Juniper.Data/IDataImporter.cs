using System.ComponentModel;
using System.Linq.Expressions;

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
    private static Dictionary<DbContext, Dictionary<IQueryable, ISet<object>>> contextCaches = new();

    private static ISet<object> CoallesceCache<DbContextT, EntityT>(IQueryable<EntityT> set, DbContextT db, params string[] includeProps)
        where DbContextT : DbContext
        where EntityT : class
    {
        if (!contextCaches.ContainsKey(db))
        {
            contextCaches.Add(db, new Dictionary<IQueryable, ISet<object>>());
        }

        var contextCache = contextCaches[db];
        if (!contextCache.ContainsKey(set))
        {
            foreach(var includeProp in includeProps)
            {
                set = set.Include(includeProp);
            }
            contextCache.Add(set, set.Select(item => item as object).ToHashSet());
        }

        var setCache = contextCache[set];
        return setCache;
    }

    public static IEnumerable<EntityT> GetAllCached<DbContextT, EntityT>(this DbSet<EntityT> set, DbContextT db, params string[] includeProps)
        where DbContextT : DbContext
        where EntityT : class
    {
        var setCache = CoallesceCache(set, db, includeProps);
        foreach (var obj in setCache)
        {
            if (obj is EntityT ent)
            {
                yield return ent;
            }
        }
    }

    public static void AddCached<DbContextT, EntityT>(this DbSet<EntityT> set, DbContextT db, EntityT obj)
        where DbContextT : DbContext
        where EntityT : class
    {
        var setCache = CoallesceCache(set, db);
        set.Add(obj);
        setCache.Add(obj);
    }
}