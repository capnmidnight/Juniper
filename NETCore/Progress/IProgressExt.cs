using static System.Math;

namespace Juniper.Progress;


/// <summary>
/// Progress reporting interface mixin methods.
/// </summary>
public static class IProgressExt
{
    /// <summary>
    /// The minimum amount of difference to allow between two floats to consider them the same number.
    /// </summary>
    private const float ALPHA = 1e-3f;

    /// <summary>
    /// Check to see <paramref name="prog"/>'s progress is within <see cref="ALPHA"/> of 1.
    /// </summary>
    /// <param name="prog"></param>
    /// <returns>True when progress is reasonably close to 1</returns>
    public static bool IsComplete(this IProgress prog)
    {
        var progress = prog?.Progress ?? 1;
        return Abs(progress - 1) < ALPHA;
    }

    /// <summary>
    /// Make a subdivision of a progress meter. Progress from [0, 1] on the subdivision
    /// will map to progress from [<paramref name="start"/>, <paramref name="start"/> + <paramref name="length"/>]
    /// on the parent progress tracker.
    /// </summary>
    /// <param name="parent">The tracker to subdivide</param>
    /// <param name="start">The beginning of the output range.</param>
    /// <param name="length">The length of the output range.</param>
    /// <param name="prefix">A text prefix to include as part of the status update.</param>
    /// <returns></returns>
    public static IProgress Subdivide(this IProgress parent, float start, float length, string? prefix = null)
    {
        return new ProgressSubdivision(parent, start, length, prefix);
    }

    /// <summary>
    /// Make a subdivision of a progress meter. Progress from [0, 1] on the subdivision
    /// will map to progress from [<paramref name="index"/>/<paramref name="count"/>, (<paramref name="index"/> + 1) / <paramref name="count"/>]
    /// on the parent progress tracker. This is useful for keeping track of iterations over elements in an array.
    /// </summary>
    /// <param name="parent">The tracker to subdivide</param>
    /// <param name="index">Which part of the subdivision to create.</param>
    /// <param name="count">The total number of subdivisions that this subdivision will be a part of.</param>
    /// <param name="prefix">A text prefix to include as part of the status update.</param>
    /// <returns></returns>
    public static IProgress Subdivide(this IProgress parent, int index, int count, string? prefix = null)
    {
        return new ProgressSubdivision(parent, (float)index / count, 1f / count, prefix);
    }

    /// <summary>
    /// Split a progress tracker into <paramref name="numParts"/> sub-trackers.
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="numParts"></param>
    /// <param name="prefix"></param>
    /// <returns></returns>
    public static IReadOnlyList<IProgress> Split(this IProgress? parent, long numParts)
    {
        return new ProgressAggregator(parent, numParts);
    }

    /// <summary>
    /// Split a progress tracker into sub-trackers with the provided prefixes, one per prefix.
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="prefixes"></param>
    /// <returns></returns>
    public static IReadOnlyList<IProgress> Split(this IProgress? parent, params string[] prefixes)
    {
        return new ProgressAggregator(parent, prefixes);
    }

    public static IEnumerable<(IProgress prog, T act)> Zip<T>(this IProgress? parent, params T[] items)
    {
        return parent
            .Split(items.Length)
            .Select((p, i) => (p, items[i]));
    }

    private static IEnumerable<(IProgress prog, T act)> Split2<T>(this IProgress? parent, params T[] actors)
        where T : Delegate
    {
        return parent
            .Split(actors.Length)
            .Select((p, i) => (p, actors[i]));
    }

    public static void Run(this IProgress parent, params Action<IProgress>[] actors)
    {
        foreach ((var prog, var act) in parent.Split2(actors))
        {
            prog.Report(0);
            act.Invoke(prog);
            prog.Report(1);
        }
    }

    public static void Run(this IProgress parent, params Action[] actors)
    {
        foreach ((var prog, var act) in parent.Split2(actors))
        {
            prog.Report(0);
            act.Invoke();
            prog.Report(1);
        }
    }

    public static async Task RunAsync(this IProgress parent, params Func<IProgress, Task>[] actors)
    {
        foreach ((var prog, var act) in parent.Split2(actors))
        {
            prog.Report(0);
            await act.Invoke(prog)
                .ConfigureAwait(false);
            prog.Report(1);
        }
    }

    public static async Task RunAsync(this IProgress parent, params Func<Task>[] actors)
    {
        foreach ((var prog, var act) in parent.Split2(actors))
        {
            prog.Report(0);
            await act.Invoke()
                .ConfigureAwait(false);
            prog.Report(1);
        }
    }

    /// <summary>
    /// Convert a ratio progress value to a proportion progress value.
    /// </summary>
    /// <param name="prog"></param>
    /// <param name="count"></param>
    /// <param name="length"></param>
    /// <param name="status"></param>
    public static void Report(this IProgress prog, float count, float length, string? status = null)
    {
        if (length > 0)
        {
            prog.Report(count / length, status);
        }
        else
        {
            prog.Report(1, status);
        }
    }

    public static Task WaitOnAsync(this IProgress parent, IProgress child, string? prefix = null)
    {
        return Task.Run(() =>
        {
            while (child.Progress < 1)
            {
                string? message = null;

                if (string.IsNullOrEmpty(child.Status))
                {
                    message = prefix;
                }
                else if (string.IsNullOrEmpty(prefix))
                {
                    message = child.Status;
                }
                else
                {
                    message = prefix + ": " + child.Status;
                }

                parent.Report(child.Progress, message);
                Task.Yield();
            }
        });
    }
}