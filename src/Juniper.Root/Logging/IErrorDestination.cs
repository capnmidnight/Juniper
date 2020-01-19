namespace Juniper.Logging
{
    public interface IErrorDestination
    {
        void OnError(object source, ErrorEventArgs e);
    }
}
