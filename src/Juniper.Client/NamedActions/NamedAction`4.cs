using System;

namespace Juniper
{
    public class NamedAction<T1, T2, T3, T4> : AbstractNamedAction<Action<T1, T2, T3, T4>>
    {
        public static implicit operator NamedAction<T1, T2, T3, T4>((string, Action<T1, T2, T3, T4>) tuple)
        {
            return new NamedAction<T1, T2, T3, T4>(tuple.Item1, tuple.Item2);
        }

        public NamedAction(string name, Action<T1, T2, T3, T4> action)
            : base(name, action)
        { }

        public void Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            Action(arg1, arg2, arg3, arg4);
        }
    }
}