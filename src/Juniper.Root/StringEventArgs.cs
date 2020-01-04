namespace Juniper
{
    public class StringEventArgs : EventArgs<string>
    {
        public StringEventArgs(string value)
            : base(value)
        { }
    }
}