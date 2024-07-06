using System.Linq.Expressions;

using Juniper.Cedrus.Entities;

using Microsoft.EntityFrameworkCore;

namespace Juniper.Cedrus.Controllers.V1;

public static partial class DbSetExtensions
{
    public static T? FindByName<T>(this DbSet<T> set, string name)
        where T : class, INamed =>
        set.FindByNameAsync(name).Result;

    public static async Task<T?> FindByNameAsync<T>(this DbSet<T> set, string name)
        where T : class, INamed =>
        await set.SingleOrDefaultAsync(v => v.Name == name)
            ?? set.Local
                .Where(v => v.Id == 0)
                .SingleOrDefault(v => v.Name == name);

    internal static T Upsert<T>(this DbSet<T> set, string name, Func<T> create, Action<T>? update = null)
        where T : class, INamed =>
        set.UpsertAsync(name, create, update).Result;

    internal static async Task<T> UpsertAsync<T>(this DbSet<T> set, string name, Func<T> create, Action<T>? update = null)
        where T : class, INamed
    {
        var value = await set.FindByNameAsync(name);
        if (value is null)
        {
            await set.AddAsync(value = create());
        }
        else if (update is not null)
        {
            update(value);
        }

        return value;
    }

    public static IQueryable<ValueT> Secure<ValueT>(this IQueryable<ValueT> data, ClassificationParts parts)
        where ValueT : class, IClassificationMarked =>
        from v in data
        where parts.Levels.Contains(v.Classification.LevelId)
           && v.Classification.Caveats.All(cv => parts.Caveats.Contains(cv.Id))
        select v;

    public static IEnumerable<ValueT> Secure<ValueT>(this ICollection<ValueT> data, ClassificationParts parts)
        where ValueT : class, IClassificationMarked =>
        from v in data
        where parts.Levels.Contains(v.Classification.LevelId)
           && v.Classification.Caveats.All(cv => parts.Caveats.Contains(cv.Id))
        select v;

    public static IQueryable<ValueT> Current<TypeT, ValueT>(this IQueryable<ValueT> data, ClassificationParts parts, DateTime? context = null)
        where TypeT : INamed
        where ValueT : class, ITimeSeries<TypeT>, IClassificationMarked
    {
        context ??= DateTime.Now;
        return from d in data.Secure(parts)
               where d.Start <= context && context < d.End
               select d;
    }

    public static IQueryable<ValueT> Historic<TypeT, ValueT>(this IQueryable<ValueT> data, ClassificationParts parts, DateTime? context = null)
        where TypeT : INamed
        where ValueT : class, ITimeSeries<TypeT>, IClassificationMarked
    {
        context ??= DateTime.Now;
        return from v in data.Secure(parts)
               where v.End <= context
               select v;
    }

    public static IEnumerable<ValueT> GetChain<ValueT>(this ValueT data)
        where ValueT : class, IParentChained<ValueT>
    {
        var here = data;
        while (here is not null)
        {
            yield return here;
            here = here.Parent;
        }
    }

    public static ValueT TimeSeriesSplit<TypeT, ValueT>(
        this DbSet<ValueT> data,
        TypeT type,
        Expression<Func<ValueT, bool>> matchKey,
        Func<ValueT, bool> matchLocal,
        Func<ValueT, ValueT, bool> compareValues,
        Func<TypeT, DateTime, DateTime, ValueT> createValue,
        Func<TypeT, ValueT, DateTime, DateTime, ValueT> splitValue,
        DateTime? startDate = null,
        DateTime? endDate = null)
        where TypeT : class, ISequenced
        where ValueT : class, ITimeSeries<TypeT>
    {
        var startX = startDate ?? DateTime.Now;
        var endX = endDate ?? DateTime.MaxValue;

        var start = DateTimeExt.Min(startX, endX);
        var end = DateTimeExt.Max(startX, endX);

        var toInsert = createValue(type, start, end);

        var overlapping = (from here in data.Where(matchKey)
                           where here.TypeId == type.Id
                               && here.Start < toInsert.End
                               && toInsert.Start < here.End
                           orderby here.Start
                           select here)
                           .AsEnumerable()
                           .Union(from here in data.Local.Where(matchLocal)
                                  where here.Id == 0 && here.Type == type
                                      && here.Start < toInsert.End
                                      && toInsert.Start < here.End
                                  orderby here.Start
                                  select here)
                           .ToList();

        for (var i = overlapping.Count - 1; i >= 0; --i)
        {
            var here = overlapping[i];
            if (toInsert.Start <= here.Start
                && here.Start < toInsert.End && toInsert.End <= here.End)
            {
                // Overlapping start
                if (compareValues(here, toInsert))
                {
                    toInsert.End = here.End;
                    here.Start = here.End = toInsert.Start;
                }
                else
                {
                    here.Start = toInsert.End;
                }
            }
            else if (here.Start <= toInsert.Start && toInsert.Start < here.End
                && toInsert.End >= here.End)
            {
                // Overlapping end
                if (compareValues(here, toInsert))
                {
                    toInsert.Start = DateTimeExt.Min(toInsert.Start, here.Start);
                    here.Start = here.End = toInsert.Start;
                }
                else
                {
                    here.End = toInsert.Start;
                }
            }
            else if (toInsert.Start <= here.Start && here.End <= toInsert.End)
            {
                // Completely surrounding
                here.Start = here.End = toInsert.Start;
            }
            else if (here.Start <= toInsert.Start && toInsert.End <= here.End)
            {
                // Completely internal
                if (compareValues(here, toInsert))
                {
                    toInsert.Start = DateTimeExt.Min(toInsert.Start, here.Start);
                    toInsert.End = DateTimeExt.Max(toInsert.End, here.End);
                    here.Start = here.End = toInsert.Start;
                }
                else
                {
                    // Start
                    here.End = toInsert.Start;

                    // toInsert is the middle

                    // End
                    var split = splitValue(type, here, toInsert.End, here.End);
                    if (split.Alive.Ticks > 0)
                    {
                        data.Add(split);
                    }
                }
            }
            else if (toInsert.End <= here.Start || toInsert.Start >= here.End)
            {
                // Not at all related
            }
            else
            {
                throw new Exception("This shouldn't happen!");
            }

            if (here.Alive.Ticks == 0)
            {
                data.Remove(here);
            }
        }

        data.Add(toInsert);

        return toInsert;
    }
}