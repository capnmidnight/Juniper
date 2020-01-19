namespace Juniper.HTTP.Server
{
    public interface INCSALogDestination
    {
        void OnLog(object source, StringEventArgs e);
    }
}
