using System.Linq;

namespace Juniper.Serial
{
    public class SafeSerialPortListing : ISerialPortListing
    {
        public double MinConnectionCheckTime { get { return 1; } }

        public double MinPortCheckTime { get { return 1; } }

        // The FTDI chip crashes if you try to reconnect before 10 seconds after it has restarted
        public double MinReconnectTime { get { return 10; } }

        public string[] PortNames
        {
            get
            {
                return System.IO.Ports.SerialPort.GetPortNames().Where(p => p.StartsWith("COM")).ToArray();
            }
        }
    }
}
