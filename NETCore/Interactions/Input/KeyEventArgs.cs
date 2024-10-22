namespace Juniper.Input;

public class KeyEventArgs : EventArgs
{
    public string Name { get; }
    public KeyEventArgs(string name)
    {
        Name = name;
    }
}
