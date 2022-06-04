namespace Juniper.Logging
{
    public interface IWarningSource
    {
        event EventHandler<StringEventArgs> Warning;
    }
}
