namespace Juniper;

public class NamedAction : AbstractNamedAction<Action>
{
    public static implicit operator NamedAction((string, Action) tuple)
    {
        return new NamedAction(tuple.Item1, tuple.Item2);
    }

    public NamedAction(string name, Action action)
        : base(name, action)
    { }

    public void Invoke()
    {
        Action();
    }
}