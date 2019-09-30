using System;
using System.Collections.Generic;
using System.Linq;

namespace Juniper.Serial
{
    public abstract class SerialPortDevice<RecordType, PortFactoryType> : IDisposable where PortFactoryType : ISerialPortFactory, new()
    {
        public SerialPortDevice(string portName, int baudRate)
        {
            dataQueue = new Queue<RecordType>();
            buffer = string.Empty;
            IsRecording = false;
            handshakeComplete = false;
            port = serialPortFactory.MakeSerialPort();
            this.baudRate = baudRate;
            PortName = portName;
        }

        public SerialPortDevice(int baudRate)
            : this(null, baudRate)
        {
        }

        public bool IsOpen { get { return port != null && port.IsOpen; } }
        public bool IsPortAvailable { get { return serialPortFactory.PortNames.Contains(PortName); } }
        public bool IsRecording { get; private set; }
        public string LastTestedString { get; private set; }
        public RecordType LastValue { get; private set; }
        public string PortName { get; private set; }

        public bool RTS
        {
            get
            {
                return IsOpen && port.RtsEnable;
            }
            set
            {
                if (IsOpen)
                {
                    port.RtsEnable = value;
                }
            }
        }

        public void BeginRecording()
        {
            IsRecording = true;
        }

        public void Close()
        {
            WithLock("Close", _Close);
        }

        private void _Close()
        {
            buffer = string.Empty;
            if (IsOpen)
            {
                port.DiscardInBuffer();
                port.DiscardOutBuffer();
                port.Close();
            }
        }

        public Queue<RecordType> CopyQueue()
        {
            var output = new Queue<RecordType>();
            lock (dataQueue)
            {
                while (dataQueue.Count > 0)
                {
                    output.Enqueue(dataQueue.Dequeue());
                }
            }

            return output;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void EndRecording()
        {
            IsRecording = false;
        }

        public void Open()
        {
            port.Open();
        }

        public bool Open(string portName, bool autoSearch, params string[] skipPorts)
        {
            return WithLock("Open", OpenInternal, this, portName, autoSearch, skipPorts);
        }

        private static bool OpenInternal(SerialPortDevice<RecordType, PortFactoryType> device, string portName, bool autoSearch, string[] skipPorts)
        {
            device.LastTestedString = null;
            device.PortName = portName;
            if (!device.PortName.StartsWith("COM"))
            {
                device.PortName = null;
            }

            bool good = false;
            if (!string.IsNullOrEmpty(device.PortName))
            {
                good = device.HandshakePort(skipPorts);
                if (!good)
                {
                    device.PortName = null;
                }
            }

            return good || autoSearch && device.FindMatchingPort(skipPorts);
        }

        public List<Exception> ReadData()
        {
            List<Exception> errors = null;
            if (IsOpen)
            {
                errors = WithLock("ReadData", ProcessBuffer);
            }

            if (errors == null)
            {
                errors = new List<Exception>();
            }

            return errors;
        }

        protected static PortFactoryType serialPortFactory = new PortFactoryType();

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _Close();
                port.Dispose();
                port = null;
                handshakeComplete = false;
                IsRecording = false;
            }
        }

        protected abstract bool Evaluate(string line);

        protected abstract RecordType Process(string line);

        private readonly int baudRate;
        private readonly Queue<RecordType> dataQueue;
        private string buffer;
        private bool handshakeComplete;
        private string lockedOn;
        private ISerialPort port;

        private bool FindMatchingPort(string[] skipPorts)
        {
            var portNames = serialPortFactory.PortNames;
            foreach (var name in portNames)
            {
                PortName = name;
                if (HandshakePort(skipPorts))
                {
                    return true;
                }
            }
            PortName = null;
            return false;
        }

        private bool HandshakePort(string[] skipPorts)
        {
            handshakeComplete = false;
            if (skipPorts == null || !skipPorts.Contains(PortName))
            {
                try
                {
                    _Close();
                    port.PortName = PortName;
                    port.BaudRate = baudRate;
                    port.ReadTimeout = 1000;
                    port.Open();
                    port.DiscardInBuffer();
                    port.RtsEnable = false;
                    for (var i = 0; i < 4 && !handshakeComplete; i++)
                    {
                        LastTestedString = port.ReadLine();
                        if (Evaluate(LastTestedString))
                        {
                            // recapture the input that was used for the handshake
                            MaybeEnqueue(LastTestedString);
                            handshakeComplete = true;
                            if (!IsOpen)
                            {
                                port.Open();
                            }
                        }
                    }
                }
                catch (Exception exp)
                {
                    LastTestedString = exp.Message;
                }
                finally
                {
                    if (!handshakeComplete)
                    {
                        _Close();
                    }
                }
            }
            return handshakeComplete;
        }

        private void MaybeEnqueue(string line)
        {
            var temp = Process(line);
            if (temp != null)
            {
                LastValue = temp;
                if (IsRecording)
                {
                    lock (dataQueue)
                    {
                        dataQueue.Enqueue(temp);
                    }
                }
            }
        }

        private List<Exception> ProcessBuffer()
        {
            var errors = new List<Exception>();
            var bufferBeforeRead = buffer;
            try
            {
                buffer += port.ReadExisting();
                // the Trimble GPS device seems to close when you've read to the end of its buffer
                if (!IsOpen)
                {
                    port.Open();
                }

                if (buffer.Length > 0)
                {
                    buffer = buffer
                        .Replace('\r', '\n')
                        .Replace("\n\n", "\n");
                    if (buffer.Contains('\n'))
                    {
                        var parts = buffer.SplitX('\n');
                        for (var i = 0; i < parts.Length - 1; ++i)
                        {
                            try
                            {
                                MaybeEnqueue(parts[i]);
                            }
                            catch (Exception exp)
                            {
                                var message = string.Format(
                                    "port: {0}\r\ncurrentLine: \"{1}\"\r\nrest:\"{2}\"\r\n",
                                    PortName,
                                    bufferBeforeRead,
                                    buffer);
                                errors.Add(new Exception(message, exp));
                            }
                        }

                        // If the buffer had ended with a line feed, then the last element of the
                        // parts array will be an empty string. If the buffer had not ended with a
                        // line feed, then the port.ReadExisting call had retrieved incomplete
                        // information, so buffer is set to that last chunk of data so that the next
                        // port.ReadExisting() call will complete it.

                        buffer = parts.Last();
                    }
                }
            }
            // this occurs occasionally if the connection is lost between the IsOpen check and the
            // ReadExisting call. Just ignore it, and next time IsOpen should report False.
            catch (UnauthorizedAccessException) { }
            catch (Exception exp)
            {
                var message = string.Format(
                    "port: {0}\r\ncurrentLine: \"{1}\"\r\nrest:\"{2}\"\r\n",
                    PortName,
                    bufferBeforeRead,
                    buffer);
                errors.Add(new Exception(message, exp));
            }
            return errors;
        }

        /// <summary>
        /// The serial port reading is really finnicky about threading issues. This WithLock function
        /// takes a function to call that will receive exclusive control of the port. If a lock is
        /// already in place, then it will not wait on the lock, but instead return immediately.
        ///
        /// There is a race condition between obtaining a lock and signalling that the lock was
        /// obtained. In that case, the losing thread will block on the lock, until the winning
        /// thread releases it. This may not be the desired behavior, but it might be recoverable on
        /// its own.
        /// </summary>
        /// <param name="lockName">Naming the lock assists in debugging where the lock was acquired</param>
        /// <param name="act"></param>
        private void WithLock(string lockName, Action act)
        {
            if (lockedOn == null && port != null)
            {
                lock (port)
                {
                    lockedOn = lockName;
                    try
                    {
                        act();
                    }
                    finally
                    {
                        lockedOn = null;
                    }
                }
            }
        }

        private T WithLock<T>(string lockName, Func<T> act)
        {
            if (lockedOn == null && port != null)
            {
                lock (port)
                {
                    lockedOn = lockName;
                    try
                    {
                        return act();
                    }
                    finally
                    {
                        lockedOn = null;
                    }
                }
            }

            return default;
        }

        private T WithLock<V, W, X, T>(
            string lockName,
            Func<SerialPortDevice<RecordType, PortFactoryType>, V, W, X, T> act,
            SerialPortDevice<RecordType, PortFactoryType> a,
            V b,
            W c,
            X d)
        {
            if (lockedOn == null && port != null)
            {
                lock (port)
                {
                    lockedOn = lockName;
                    try
                    {
                        return act(a, b, c, d);
                    }
                    finally
                    {
                        lockedOn = null;
                    }
                }
            }

            return default;
        }
    }
}
