namespace System.Collections.Generic;

/// <summary>
/// Extension methods for <c>System.Collections.Generic.IEnumerable{T}</c>
/// </summary>
public static class IEnumerableExt
{
    /// <summary>
    /// Creates set-like behavior for generic lists. If the list already contains
    /// the given value, it will not be added to the list again.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="collect"></param>
    /// <param name="value"></param>
    /// <returns>True if the item was actually added.</returns>
    public static bool MaybeAdd<T>(this List<T> collect, T value)
    {
        if (collect is null)
        {
            throw new ArgumentNullException(nameof(collect));
        }

        if (!collect.Contains(value))
        {
            collect.Add(value);
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// A random number generator to use with the following methods.
    /// </summary>
    private static readonly Random r = new();

    /// <summary>
    /// Get a random item out of the collection.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="collection"></param>
    /// <returns>A random item out of the collection.</returns>
    /// <example><code><![CDATA[
    /// // This is just one potential outcome.
    /// var arr = new int[]{ 1, 2, 3, 4, 5, 6, 7, 8, 9 };
    /// arr.Random(); // --> 3
    /// arr.Random(); // --> 1
    /// arr.Random(); // --> 4
    /// arr.Random(); // --> 1
    /// arr.Random(); // --> 5
    /// arr.Random(); // --> 9
    /// arr.Random(); // --> 2
    /// arr.Random(); // --> 6
    /// ]]></code></example>
    public static T? RandomItem<T>(this IEnumerable<T> collection)
    {
        var count = collection.Count();
        var skip = r.Next(0, count);
        return collection
            .Skip(skip)
            .FirstOrDefault();
    }

    /// <summary>
    /// Checks two collections to see if they contain all the same items.
    /// </summary>
    /// <typeparam name="T">Any type</typeparam>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns>
    /// True if the collections are the same length and contain the same items, in the same order.
    /// </returns>
    /// <example><code><![CDATA[
    /// var a = new int[]{ 1, 2, 3, 4, 5 };
    /// var b = new int[]{ 1, 2, 3, 4, 5 };
    /// var c = new int[]{ 1, 2, 3, 4, 6 };
    /// var d = new int[]{ 1, 2, 3, 4, 5, 6 };
    /// var e = new int[]{ 1, 2, 3, 4 };
    /// a.Matches(b); // --> true
    /// a.Matches(c); // --> false
    /// a.Matches(d); // --> false
    /// a.Matches(e); // --> false
    /// ]]></code></example>
    public static bool Matches<T>(this IEnumerable<T> a, IEnumerable<T> b)
    {
        if (a is null)
        {
            return b is null;
        }
        else if (b is null)
        {
            return false;
        }

        if (a.Count() != b.Count())
        {
            return false;
        }

        using var c = a.GetEnumerator();
        using var d = b.GetEnumerator();
        while (c.MoveNext() && d.MoveNext())
        {
            if ((c.Current is null) != (d.Current is null))
            {
                return false;
            }
            else if (c.Current is not null && !c.Current.Equals(d.Current)
                || d.Current is not null && !d.Current.Equals(c.Current))
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Checks two collections to see if they contain all the same items.
    /// </summary>
    /// <typeparam name="T">Any type</typeparam>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns>
    /// True if the collections are the same length and contain the same items, in the same order.
    /// </returns>
    /// <example><code><![CDATA[
    /// var a = new int[]{ 1, 2, 3, 4, 5 };
    /// var b = new int[]{ 1, 2, 3, 4, 5 };
    /// var c = new int[]{ 1, 2, 3, 4, 6 };
    /// var d = new int[]{ 1, 2, 3, 4, 5, 6 };
    /// var e = new int[]{ 1, 2, 3, 4 };
    /// a.Matches(b); // --> true
    /// a.Matches(c); // --> false
    /// a.Matches(d); // --> false
    /// a.Matches(e); // --> false
    /// ]]></code></example>
    public static bool Matches<T>(this IEnumerable<T> a, IEnumerable b)
    {
        if (a is null || b is null)
        {
            return (a is null) == (b is null);
        }

        return a.Matches(b.Cast<T>());
    }

    /// <summary>
    /// Checks two collections to see if they contain all the same items.
    /// </summary>
    /// <typeparam name="T">Any type</typeparam>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns>
    /// True if the collections are the same length and contain the same items, in the same order.
    /// </returns>
    /// <example><code><![CDATA[
    /// var a = new int[]{ 1, 2, 3, 4, 5 };
    /// var b = new int[]{ 1, 2, 3, 4, 5 };
    /// var c = new
    /// int[]{ 1, 2, 3, 4, 6 };
    /// var d = new int[]{ 1, 2, 3, 4, 5, 6 };
    /// var e = new int[]{ 1, 2, 3, 4 };
    /// a.Matches(b); // --> true
    /// a.Matches(c); // --> false
    /// a.Matches(d); // --> false
    /// a.Matches(e); // --> false
    /// ]]></code></example>
    public static bool Matches<T>(this IEnumerable<T> a, IEnumerator b)
    {
        if (a is null || b is null)
        {
            return (a is null) == (b is null);
        }

        return a.Matches(b.AsEnumerable().Cast<T>());
    }

    /// <summary>
    /// Check two collections of types to see if they contain all the same items.
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static bool Matches(this IEnumerable<Type> a, IEnumerable<Type> b)
    {
        if (a is null)
        {
            return b is null;
        }
        else if (b is null)
        {
            return false;
        }

        if (a.Count() != b.Count())
        {
            return false;
        }

        using var c = a.GetEnumerator();
        using var d = b.GetEnumerator();
        while (c.MoveNext() && d.MoveNext())
        {
#if NETFX_CORE
                var e = c.Current.GetTypeInfo();
                var f = d.Current.GetTypeInfo();
#else
            var e = c.Current;
            var f = d.Current;
#endif
            if (!e.IsAssignableFrom(f))
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Takes N enumerators and moves through them all, even if some of them are null or finished.
    /// </summary>
    /// <param name="enums"></param>
    /// <returns><c>True</c> when all enumerators can no longer move to another item.</returns>
    public static bool MoveNext(this IEnumerable<IEnumerator> enums)
    {
        if (enums is null)
        {
            throw new ArgumentNullException(nameof(enums));
        }

        var anyNext = false;
        foreach (var iter in enums)
        {
            anyNext |= iter?.MoveNext() == true;
        }

        return anyNext;
    }

    /// <summary>
    /// For N enumerators, returns all M <c>Current</c> values of each non-null entry in the
    /// <paramref name="enums"/> collection. M is less than or equal to N.
    /// </summary>
    /// <param name="enums"></param>
    /// <returns></returns>
    public static IEnumerable Current(this IEnumerable<IEnumerator> enums)
    {
        if (enums is null)
        {
            throw new ArgumentNullException(nameof(enums));
        }

        var anyExist = false;
        var anySucceed = false;
        foreach (var e in enums)
        {
            if (e is not null)
            {
                anyExist = true;
                object? value;
                try
                {
                    value = e.Current;
                    anySucceed = true;
                }
                catch (InvalidOperationException)
                {
                    value = null;
                }

                if (value is not null)
                {
                    yield return value;
                }
            }
        }

        if (anyExist && !anySucceed)
        {
            throw new InvalidOperationException();
        }
    }

    /// <summary>
    /// Filters a collection and checks to see if it would return 0 results.
    /// </summary>
    /// <returns>The empty.</returns>
    /// <param name="enumer">Enumerator.</param>
    /// <param name="predicate">Predicate.</param>
    /// <typeparam name="T">The 1st type parameter.</typeparam>
    public static bool Empty<T>(this IEnumerable<T> enumer, Func<T, bool> predicate)
    {
        return !enumer.Any(predicate);
    }

    /// <summary>
    /// Evaluates a collection and checks to see if it has 0 items.
    /// </summary>
    /// <returns>The empty.</returns>
    /// <param name="enumer">Enumerator.</param>
    /// <typeparam name="T">The 1st type parameter.</typeparam>
    public static bool Empty<T>(this IEnumerable<T> enumer)
    {
        return !enumer.Any();
    }

    /// <summary>
    /// Evaluates a collection and checks to see if it has 0 items.
    /// </summary>
    /// <returns>The empty.</returns>
    /// <param name="enumer">Enumerator.</param>
    /// <typeparam name="T">The 1st type parameter.</typeparam>
    public static bool Empty(this IEnumerable enumer)
    {
        return enumer.Cast<object>().Empty();
    }

    /// <summary>
    /// Convert an Enumerator collection to an Enumerable collection.
    /// </summary>
    /// <returns>The enumerable.</returns>
    /// <param name="iter">Iterator.</param>
    public static IEnumerable AsEnumerable(this IEnumerator iter)
    {
        while (iter?.MoveNext() == true)
        {
            yield return iter.Current;
        }
    }

    /// <summary>
    /// Add an entire collection to a stack.
    /// </summary>
    /// <typeparam name="T">The type of elements in the stack.</typeparam>
    /// <param name="q">The stack to which to add items.</param>
    /// <param name="e">The items to add to the stack, in order.</param>
    public static void AddRange<T>(this Stack<T> q, IEnumerable<T> e)
    {
        if (q is null)
        {
            throw new ArgumentNullException(nameof(q));
        }

        if (e is null)
        {
            throw new ArgumentNullException(nameof(e));
        }

        foreach (var o in e)
        {
            q.Push(o);
        }
    }

    /// <summary>
    /// Add an entire collection to a queue.
    /// </summary>
    /// <typeparam name="T">The type of elements in the queue.</typeparam>
    /// <param name="q">The queue to which to add items.</param>
    /// <param name="e">The items to add to the queue, in order.</param>
    public static void AddRange<T>(this Queue<T> q, IEnumerable<T> e)
    {
        if (q is null)
        {
            throw new ArgumentNullException(nameof(q));
        }

        if (e is null)
        {
            throw new ArgumentNullException(nameof(e));
        }

        foreach (var o in e)
        {
            q.Enqueue(o);
        }
    }

    /// <summary>
    /// Make Queues usable with collection initializer syntax.
    /// </summary>
    /// <typeparam name="T">The type of the elements of the queue</typeparam>
    /// <param name="q">The queue to which to add the item.</param>
    /// <param name="value">The item to add to the queue.</param>
    public static void Add<T>(this Queue<T> q, T value)
    {
        if (q is null)
        {
            throw new ArgumentNullException(nameof(q));
        }

        q.Enqueue(value);
    }

    /// <summary>
    /// Make Stacks usable with collection initializer syntax.
    /// </summary>
    /// <typeparam name="T">The type of the elements of the stack</typeparam>
    /// <param name="q">The stack to which to add the item.</param>
    /// <param name="value">The item to add to the stack.</param>
    public static void Add<T>(this Stack<T> q, T value)
    {
        if (q is null)
        {
            throw new ArgumentNullException(nameof(q));
        }

        q.Push(value);
    }

    public static string ToString(this IEnumerable<string?> enumer, string sep)
    {
        return string.Join(sep, enumer);
    }

    public static string ToString<T>(this IEnumerable<T> enumer, string sep)
    {
        return enumer.Select(ValueToString<T>).ToString(sep);
    }

    private static string? ValueToString<T>(T value)
    {
        return value?.ToString();
    }

    public static TimeSpan Sum(this IEnumerable<TimeSpan> values)
    {
        return values.Aggregate(TimeSpan.Zero, (a, b) => a + b);
    }

    public static TimeSpan Sum<T>(this IEnumerable<T> values, Func<T, TimeSpan> selector)
    {
        return values.Select(selector).Sum();
    }

    public static TResult? MaybeMax<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector)
    {
        if (source == null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        if (selector == null)
        {
            throw new ArgumentNullException(nameof(selector));
        }

        TResult? value = default;
        using (var e = source.GetEnumerator())
        {
            if (value == null)
            {
                do
                {
                    if (!e.MoveNext())
                    {
                        return value;
                    }

                    value = selector(e.Current);
                }
                while (value == null);

                var comparer = Comparer<TResult>.Default;
                while (e.MoveNext())
                {
                    var x = selector(e.Current);
                    if (x != null && comparer.Compare(x, value) > 0)
                    {
                        value = x;
                    }
                }
            }
            else
            {
                if (!e.MoveNext())
                {
                    return default;
                }

                value = selector(e.Current);
                while (e.MoveNext())
                {
                    var x = selector(e.Current);
                    if (Comparer<TResult?>.Default.Compare(x, value) > 0)
                    {
                        value = x;
                    }
                }
            }
        }

        return value;
    }

    public static TResult? MaybeMin<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector)
    {
        if (source == null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        if (selector == null)
        {
            throw new ArgumentNullException(nameof(selector));
        }

        TResult? value = default;
        using (var e = source.GetEnumerator())
        {
            if (value == null)
            {
                do
                {
                    if (!e.MoveNext())
                    {
                        return value;
                    }

                    value = selector(e.Current);
                }
                while (value == null);

                var comparer = Comparer<TResult?>.Default;
                while (e.MoveNext())
                {
                    var x = selector(e.Current);
                    if (x != null && comparer.Compare(x, value) < 0)
                    {
                        value = x;
                    }
                }
            }
            else
            {
                if (!e.MoveNext())
                {
                    return default;
                }

                value = selector(e.Current);
                while (e.MoveNext())
                {
                    var x = selector(e.Current);
                    if (Comparer<TResult?>.Default.Compare(x, value) < 0)
                    {
                        value = x;
                    }
                }
            }
        }

        return value;
    }

    public static async Task<List<T>> ToListAsync<T>(this IEnumerable<Task<T>> tasks)
    {
        var list = new List<T>();
        foreach (var task in tasks)
        {
            list.Add(await task);
        }
        return list;
    }

    public static IEnumerable<T> NotNull<T>(this IEnumerable<T?> iter)
        where T : class =>
        iter.Where(item => item is not null)
            .Select(item => item!);


    public static IEnumerable<T> NotNull<T>(this IEnumerable<T?> iter)
        where T : struct =>
        iter.Where(item => item is not null)
            .Select(item => item!.Value);
}