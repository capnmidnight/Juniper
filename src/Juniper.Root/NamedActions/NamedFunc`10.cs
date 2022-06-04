namespace Juniper
{
    public class NamedFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> : AbstractNamedAction<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>>
    {
        public static implicit operator NamedFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>((string, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>) tuple)
        {
            return new NamedFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(tuple.Item1, tuple.Item2);
        }

        public NamedFunc(string name, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> func)
            : base(name, func)
        { }

        public TResult Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9)
        {
            return Action(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
        }
    }
}