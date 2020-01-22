namespace Juniper.Logging
{
    public interface INCSALogDestination
    {
        void OnLog(object source, StringEventArgs e);
    }
}
