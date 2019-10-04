namespace Juniper.Serial
{
    public interface ISerialPortFactory : ISerialPortListing
    {
        ISerialPort MakeSerialPort();
    }
}
