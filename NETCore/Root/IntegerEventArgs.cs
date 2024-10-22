namespace Juniper;

public class IntegerEventArgs : EventArgs<int>
{
    public IntegerEventArgs(int arg)
        : base(arg)
    { }
}