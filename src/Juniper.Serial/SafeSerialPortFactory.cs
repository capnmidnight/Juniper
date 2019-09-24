namespace Juniper.Serial
{
    public class SafeSerialPortFactory : SafeSerialPortListing, ISerialPortFactory
    {
        public ISerialPort MakeSerialPort()
        {
            return new SafeSerialPort();
        }
    }
}
