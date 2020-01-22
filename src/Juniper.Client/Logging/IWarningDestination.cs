namespace Juniper.Logging
{
    public interface IWarningDestination
    {
        void OnWarning(object source, StringEventArgs e);
    }
}
