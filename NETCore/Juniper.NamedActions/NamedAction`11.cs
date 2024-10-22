namespace Juniper;

public class NamedAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> : AbstractNamedAction<Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>>
{
    public static implicit operator NamedAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>((string, Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>) tuple)
    {
        return new NamedAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(tuple.Item1, tuple.Item2);
    }

    public NamedAction(string name, Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> action)
        : base(name, action)
    { }

    public void Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11)
    {
        Action(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11);
    }
}