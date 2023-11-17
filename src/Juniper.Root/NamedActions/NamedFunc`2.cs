namespace Juniper;

public class NamedFunc<T, TResult> : AbstractNamedAction<Func<T, TResult>>
{
    public static implicit operator NamedFunc<T, TResult>((string, Func<T, TResult>) tuple)
    {
        return new NamedFunc<T, TResult>(tuple.Item1, tuple.Item2);
    }

    public NamedFunc(string name, Func<T, TResult> func)
        : base(name, func)
    { }

    public TResult Invoke(T obj)
    {
        return Action(obj);
    }
}