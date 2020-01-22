namespace Juniper.Logging
{
    public interface IInfoDestination
    {
        void OnInfo(object source, StringEventArgs e);
    }
}
