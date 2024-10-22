namespace Juniper.Input;

public class KeyChangeEventArgs : KeyEventArgs
{
    public bool State { get; }

    public KeyChangeEventArgs(string name, bool state)
        : base(name)
    {
        State = state;
    }
}
