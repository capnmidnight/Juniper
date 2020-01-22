using System;

namespace Juniper
{
    public class NamedAction<T1, T2, T3, T4, T5, T6, T7> : AbstractNamedAction<Action<T1, T2, T3, T4, T5, T6, T7>>
    {
        public static implicit operator NamedAction<T1, T2, T3, T4, T5, T6, T7>((string, Action<T1, T2, T3, T4, T5, T6, T7>) tuple)
        {
            return new NamedAction<T1, T2, T3, T4, T5, T6, T7>(tuple.Item1, tuple.Item2);
        }

        public NamedAction(string name, Action<T1, T2, T3, T4, T5, T6, T7> action)
            : base(name, action)
        { }

        public void Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
        {
            Action(arg1, arg2, arg3, arg4, arg5, arg6, arg7);
        }
    }
}