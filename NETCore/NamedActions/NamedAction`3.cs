namespace Juniper;

public class NamedAction<T1, T2, T3> : AbstractNamedAction<Action<T1, T2, T3>>
{
    public static implicit operator NamedAction<T1, T2, T3>((string, Action<T1, T2, T3>) tuple)
    {
        return new NamedAction<T1, T2, T3>(tuple.Item1, tuple.Item2);
    }

    public NamedAction(string name, Action<T1, T2, T3> action)
        : base(name, action)
    { }

    public void Invoke(T1 arg1, T2 arg2, T3 arg3)
    {
        Action(arg1, arg2, arg3);
    }
}