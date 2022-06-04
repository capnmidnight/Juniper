namespace Juniper.Logging
{
    public interface IErrorSource
    {
        event EventHandler<ErrorEventArgs> Err;
    }
}
