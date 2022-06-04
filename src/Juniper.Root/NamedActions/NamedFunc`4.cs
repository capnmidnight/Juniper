namespace Juniper
{
    public class NamedFunc<T1, T2, T3, TResult> : AbstractNamedAction<Func<T1, T2, T3, TResult>>
    {
        public static implicit operator NamedFunc<T1, T2, T3, TResult>((string, Func<T1, T2, T3, TResult>) tuple)
        {
            return new NamedFunc<T1, T2, T3, TResult>(tuple.Item1, tuple.Item2);
        }

        public NamedFunc(string name, Func<T1, T2, T3, TResult> func)
            : base(name, func)
        { }

        public TResult Invoke(T1 arg1, T2 arg2, T3 arg3)
        {
            return Action(arg1, arg2, arg3);
        }
    }
}