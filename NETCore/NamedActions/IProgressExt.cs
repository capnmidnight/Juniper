namespace Juniper.Progress;


/// <summary>
/// Progress reporting interface mixin methods.
/// </summary>
public static class IProgressExt
{
    private static IEnumerable<(IProgress prog, T act)> Split<T>(this IProgress? parent, params T[] actors)
        where T : INamedAction
    {
        return parent
            .Split(actors
                .Select(a => a.Name)
                .ToArray())
            .Select((p, i) => (p, actors[i]));
    }

    public static void Run(this IProgress parent, params NamedAction<IProgress>[] actors)
    {
        foreach ((var prog, var act) in parent.Split(actors))
        {
            prog.Report(0);
            act.Invoke(prog);
            prog.Report(1);
        }
    }

    public static void Run(this IProgress parent, params NamedAction[] actors)
    {
        foreach ((var prog, var act) in parent.Split(actors))
        {
            prog.Report(0);
            act.Invoke();
            prog.Report(1);
        }
    }

    public static async Task RunAsync(this IProgress parent, params NamedFunc<IProgress, Task>[] actors)
    {
        foreach ((var prog, var act) in parent.Split(actors))
        {
            prog.Report(0);
            await act.Invoke(prog)
                .ConfigureAwait(false);
            prog.Report(1);
        }
    }

    public static async Task RunAsync(this IProgress parent, params NamedFunc<Task>[] actors)
    {
        foreach ((var prog, var act) in parent.Split(actors))
        {
            prog.Report(0);
            await act.Invoke()
                .ConfigureAwait(false);
            prog.Report(1);
        }
    }
}