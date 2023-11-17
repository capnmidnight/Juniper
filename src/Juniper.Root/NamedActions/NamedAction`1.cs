namespace Juniper;

public class NamedAction<T> : AbstractNamedAction<Action<T>>
{
    public static implicit operator NamedAction<T>((string, Action<T>) tuple)
    {
        return new NamedAction<T>(tuple.Item1, tuple.Item2);
    }

    public NamedAction(string name, Action<T> action)
        : base(name, action)
    { }

    public void Invoke(T obj)
    {
        Action(obj);
    }
}