using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

using static System.Math;

namespace Juniper.Serial
{
    public class MockSerialPortListing : ISerialPortListing
    {
        public double MinConnectionCheckTime { get { return 0; } }
        public double MinPortCheckTime { get { return 0; } }
        public double MinReconnectTime { get { return 2; } }

        public virtual string[] PortNames
        {
            get
            {
                return FakePortNames.ToArray();
            }
        }

#if NETSTANDARD
        private static int MAX = 0;
#endif

        public static string MakeFakePort(int offset)
        {
#if NETSTANDARD
            var max = MAX++;
#else
            var max = 0;
            var realPorts = System.IO.Ports.SerialPort.GetPortNames();
            foreach (var realPort in realPorts)
            {
                var match = NUMBERED_PORT_REGEX.Match(realPort);
                if (match != null)
                {
                    var numberPart = match.Groups[1].Value;
                    var number = int.Parse(numberPart, CultureInfo.InvariantCulture);
                    max = Max(max, number);
                }
            }
#endif
            var portStr = (max + offset).ToString(CultureInfo.InvariantCulture);
            return "COM" + portStr;
        }

        internal static List<string> FakePortNames = new List<string>();
        private static readonly Regex NUMBERED_PORT_REGEX = new Regex("COM(\\d+)", RegexOptions.Compiled);
    }
}