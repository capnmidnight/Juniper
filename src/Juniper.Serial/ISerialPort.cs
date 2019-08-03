using System;

namespace Juniper.Serial
{
    public interface ISerialPort : IDisposable
    {
        System.IO.Stream BaseStream { get; }
        int BaudRate { get; set; }
        bool IsOpen { get; }
        string PortName { get; set; }
        int ReadTimeout { set; }
        bool RtsEnable { get; set; }

        void Close();

        void DiscardInBuffer();

        void DiscardOutBuffer();

        void Open();

        string ReadExisting();

        string ReadLine();

        void WriteLine(string msg);
    }
}
