using System;

namespace Juniper
{
    public class NamedFunc<T1, T2, T3, T4, T5, T6, T7, TResult> : AbstractNamedAction<Func<T1, T2, T3, T4, T5, T6, T7, TResult>>
    {
        public static implicit operator NamedFunc<T1, T2, T3, T4, T5, T6, T7, TResult>((string, Func<T1, T2, T3, T4, T5, T6, T7, TResult>) tuple)
        {
            return new NamedFunc<T1, T2, T3, T4, T5, T6, T7, TResult>(tuple.Item1, tuple.Item2);
        }

        public NamedFunc(string name, Func<T1, T2, T3, T4, T5, T6, T7, TResult> func)
            : base(name, func)
        { }

        public TResult Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
        {
            return Action(arg1, arg2, arg3, arg4, arg5, arg6, arg7);
        }
    }
}