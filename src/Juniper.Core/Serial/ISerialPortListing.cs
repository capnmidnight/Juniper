namespace Juniper.Serial
{
    public interface ISerialPortListing
    {
        double MinConnectionCheckTime { get; }
        double MinPortCheckTime { get; }
        double MinReconnectTime { get; }
        string[] PortNames { get; }
    }
}
