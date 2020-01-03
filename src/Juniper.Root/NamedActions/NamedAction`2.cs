using System;

namespace Juniper
{
    public class NamedAction<T1, T2> : AbstractNamedAction<Action<T1, T2>>
    {
        public static implicit operator NamedAction<T1, T2>((string, Action<T1, T2>) tuple)
        {
            return new NamedAction<T1, T2>(tuple.Item1, tuple.Item2);
        }

        public NamedAction(string name, Action<T1, T2> action)
            : base(name, action)
        { }

        public void Invoke(T1 arg1, T2 arg2)
        {
            Action(arg1, arg2);
        }
    }
}