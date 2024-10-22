using System.Linq.Expressions;

using Microsoft.EntityFrameworkCore;

namespace Juniper.Data;

public static partial class DbSetExtensions
{
    public static Task<ValueT> UpsertAsync<ValueT>(this DbSet<ValueT> set, string name, Func<ValueT> create, Action<ValueT>? update = null)
        where ValueT : class, INamed =>
        set.UpsertAsync(
            v => v.Name == name,
            create,
            update
        );

    public static async Task<ValueT> UpsertAsync<ValueT>(
        this DbSet<ValueT> set,
        Expression<Func<ValueT, bool>> matchKey,
        Func<ValueT> create,
        Action<ValueT>? update)

        where ValueT : class
    {
        var matchLocal = matchKey.Compile();
        var value = set.Local.SingleOrDefault(matchLocal)
            ?? await set.SingleOrDefaultAsync(matchKey);

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
}