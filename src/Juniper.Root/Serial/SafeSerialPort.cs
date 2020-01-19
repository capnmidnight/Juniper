#if !NETSTANDARD && !NETCOREAPP
using System;
using System.IO;
using System.IO.Ports;

namespace Juniper.Serial
{
    public class SafeSerialPort : SerialPort, ISerialPort
    {
        public SafeSerialPort(string portName, int baudRate, Parity parity, int dataBits, StopBits stopBits)
            : base(portName, baudRate, parity, dataBits, stopBits)
        {
        }

        public SafeSerialPort(string portName, int baudRate)
            : base(portName, baudRate)
        {
        }

        public SafeSerialPort()
            : base()
        {
        }

        public new void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA1816:Dispose methods should call SuppressFinalize", Justification = "This fixes a long-standing issue with .NET's serial port implementation not releasing ports when the application crashes.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public new void Open()
        {
            try
            {
                base.Open();
                theBaseStream = BaseStream;
                GC.SuppressFinalize(BaseStream);
            }
            catch
            {
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        protected override void Dispose(bool disposing)
        {
            if (disposing && Container is object)
            {
                Container.Dispose();
            }

            try
            {
                if (theBaseStream.CanRead)
                {
                    theBaseStream.Close();
                    GC.ReRegisterForFinalize(theBaseStream);
                }
            }
            catch
            {
                // ignore exception - bug with USB - serial adapters.
            }

            base.Dispose(disposing);
        }

        private Stream theBaseStream;
    }
}
#endif