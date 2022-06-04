namespace Juniper
{
    public class NamedFunc<TResult> : AbstractNamedAction<Func<TResult>>
    {
        public static implicit operator NamedFunc<TResult>((string, Func<TResult>) tuple)
        {
            return new NamedFunc<TResult>(tuple.Item1, tuple.Item2);
        }

        public NamedFunc(string name, Func<TResult> func)
            : base(name, func)
        { }

        public TResult Invoke()
        {
            return Action();
        }
    }
}