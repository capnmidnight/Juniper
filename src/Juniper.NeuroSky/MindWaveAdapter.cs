using System;
using System.Globalization;

using libStreamSDK;

using static libStreamSDK.NativeMethods;

namespace Juniper.NeuroSky
{
    /// <summary>
    /// An adapter for the NeuroSky MindWave device, converting the raw data elements to an object model.
    /// </summary>
    public sealed class MindWaveAdapter : IDisposable
    {
        /// <summary>
        /// The firmware version for the device.
        /// </summary>
        public static int Version =>
            TG_GetVersion();

        /// <summary>
        /// The serial communication baud rate to try.
        /// </summary>
        private const Baudrate DEFAULT_BAUD_RATE = Baudrate.TG_BAUD_9600;

        /// <summary>
        /// The mains electricity frequency to filter out of the signal.
        /// </summary>
        private const FilterType DEFAULT_MAINS_FILTER = FilterType.MWM15_FILTER_TYPE_60HZ;

        /// <summary>
        /// The format of the serial packets being sent by the device.
        /// </summary>
        private const SerialDataFormat DEFAULT_SERIAL_FORMAT = SerialDataFormat.TG_STREAM_PACKETS;

        /// <summary>
        /// Search the serial ports for a device that conforms to the expected MindWave behaviour.
        /// </summary>
        /// <param name="baudrate">  </param>
        /// <param name="format">    </param>
        /// <param name="filterType"></param>
        /// <returns></returns>
        public static MindWaveAdapter FindAdapter(Baudrate baudrate = DEFAULT_BAUD_RATE, SerialDataFormat format = DEFAULT_SERIAL_FORMAT, FilterType filterType = DEFAULT_MAINS_FILTER)
        {
            foreach (var portName in System.IO.Ports.SerialPort.GetPortNames())
            {
                try
                {
                    return new MindWaveAdapter(portName, baudrate, format, filterType);
                }
#pragma warning disable CA1031 // Do not catch general exception types
                catch
                {
                }
#pragma warning restore CA1031 // Do not catch general exception types
            }

            return null;
        }

        /// <summary>
        /// Search the serial ports for a device that conforms to the expected MindWave behaviour.
        /// </summary>
        /// <param name="baudrate">  </param>
        /// <param name="filterType"></param>
        /// <returns></returns>
        public static MindWaveAdapter FindAdapter(Baudrate baudrate, FilterType filterType = DEFAULT_MAINS_FILTER)
        {
            return FindAdapter(baudrate, DEFAULT_SERIAL_FORMAT, filterType);
        }

        /// <summary>
        /// Search the serial ports for a device that conforms to the expected MindWave behaviour.
        /// </summary>
        /// <param name="format">    </param>
        /// <param name="filterType"></param>
        /// <returns></returns>
        public static MindWaveAdapter FindAdapter(SerialDataFormat format, FilterType filterType = DEFAULT_MAINS_FILTER)
        {
            return FindAdapter(DEFAULT_BAUD_RATE, format, filterType);
        }

        /// <summary>
        /// Search the serial ports for a device that conforms to the expected MindWave behaviour.
        /// </summary>
        /// <param name="filterType"></param>
        /// <returns></returns>
        public static MindWaveAdapter FindAdapter(FilterType filterType)
        {
            return FindAdapter(DEFAULT_BAUD_RATE, DEFAULT_SERIAL_FORMAT, filterType);
        }

        /// <summary>
        /// The ID to use when calling the ThinkGear C-API functions.
        /// </summary>
        private readonly int connectionId;

        /// <summary>
        /// The file to read from, when reading data from a stored data file.
        /// </summary>
        private string streamFileName;

        /// <summary>
        /// File to which to log data.
        /// </summary>
        private string dataFileName;

        /// <summary>
        /// Flag to indicate whether or not a connection to the device has been made.
        /// </summary>
        private bool connected;

        /// <summary>
        /// Flag to indicate whether or not the connection is being used on a data log file, rather than a real device.
        /// </summary>
        private bool streamLogEnabled;

        /// <summary>
        /// Flag to indicate whether or not data is being logged to a file.
        /// </summary>
        private bool dataLogEnabled;

        /// <summary>
        /// Flag to indicate whether or not the device should be sending packets without requesting them.
        /// </summary>
        private bool autoReadEnabled;

        /// <summary>
        /// Serial communications rate.
        /// </summary>
        private Baudrate? baudrate;

        /// <summary>
        /// Mains electricity frequency for filtering it out of the brainwave signal.
        /// </summary>
        private FilterType mainsFilter;

        /// <summary>
        /// Sets up a new connection to the MindWave device.
        /// </summary>
        /// <exception cref="IndexOutOfRangeException">
        /// Thrown when too many connections have been made to the device.
        /// </exception>
        /// <exception cref="OutOfMemoryException">
        /// Thrown when there isn't enough memory on the device to allocate a new connection.
        /// </exception>
        public MindWaveAdapter()
        {
            connectionId = TG_GetNewConnectionId();
            switch (connectionId)
            {
                case -1:
                throw new IndexOutOfRangeException("Too many connections have been made to the device.");
                case -2:
                throw new OutOfMemoryException("Not enough free memory to allocate a new connection.");
            }
        }

        /// <summary>
        /// Setups up a new connection to the MindWave device.
        /// </summary>
        /// <param name="serialPortName"></param>
        /// <param name="baudrate"></param>
        /// <param name="format"></param>
        /// <param name="filterType"></param>
        /// <exception cref="IndexOutOfRangeException">
        /// Thrown when too many connections have been made to the device.
        /// </exception>
        /// <exception cref="OutOfMemoryException">
        /// Thrown when there isn't enough memory on the device to allocate a new connection.
        /// </exception>
        public MindWaveAdapter(string serialPortName, Baudrate baudrate = DEFAULT_BAUD_RATE, SerialDataFormat format = DEFAULT_SERIAL_FORMAT, FilterType filterType = DEFAULT_MAINS_FILTER)
            : this()
        {
            Connect(serialPortName, baudrate, format);
            MainsFrequency = filterType;
        }

        /// <summary>
        /// Setups up a new connection to the MindWave device.
        /// </summary>
        /// <param name="serialPortName"></param>
        /// <param name="baudrate"></param>
        /// <param name="filterType"></param>
        /// <exception cref="IndexOutOfRangeException">
        /// Thrown when too many connections have been made to the device.
        /// </exception>
        /// <exception cref="OutOfMemoryException">
        /// Thrown when there isn't enough memory on the device to allocate a new connection.
        /// </exception>
        public MindWaveAdapter(string serialPortName, Baudrate baudrate, FilterType filterType = DEFAULT_MAINS_FILTER)
            : this(serialPortName, baudrate, DEFAULT_SERIAL_FORMAT, filterType) { }

        /// <summary>
        /// Setups up a new connection to the MindWave device.
        /// </summary>
        /// <param name="serialPortName"></param>
        /// <param name="format"></param>
        /// <param name="filterType"></param>
        /// <exception cref="IndexOutOfRangeException">
        /// Thrown when too many connections have been made to the device.
        /// </exception>
        /// <exception cref="OutOfMemoryException">
        /// Thrown when there isn't enough memory on the device to allocate a new connection.
        /// </exception>
        public MindWaveAdapter(string serialPortName, SerialDataFormat format, FilterType filterType = DEFAULT_MAINS_FILTER)
            : this(serialPortName, DEFAULT_BAUD_RATE, format, filterType) { }

        /// <summary>
        /// Setups up a new connection to the MindWave device.
        /// </summary>
        /// <param name="serialPortName"></param>
        /// <param name="filterType"></param>
        /// <exception cref="IndexOutOfRangeException">
        /// Thrown when too many connections have been made to the device.
        /// </exception>
        /// <exception cref="OutOfMemoryException">
        /// Thrown when there isn't enough memory on the device to allocate a new connection.
        /// </exception>
        public MindWaveAdapter(string serialPortName, FilterType filterType)
            : this(serialPortName, DEFAULT_BAUD_RATE, DEFAULT_SERIAL_FORMAT, filterType) { }

        /// <summary>
        /// Get or set the frequency of the mains electricity that needs to be filtered out of the brainwave signal.
        /// </summary>
        public FilterType MainsFrequency
        {
            get
            {
                if (connected)
                {
                    var value = MWM15_getFilterType(connectionId);
                    mainsFilter = Enum.IsDefined(typeof(FilterType), value)
                        ? (FilterType)value
                        : FilterType.None;
                }

                if (mainsFilter == FilterType.None)
                {
                    return DEFAULT_MAINS_FILTER;
                }
                else
                {
                    return mainsFilter;
                }
            }
            set
            {
                if (MWM15_setFilterType(connectionId, (int)value) < 0)
                {
                    throw new Exception("Error while setting Filter Type on device.");
                }

                mainsFilter = value;
            }
        }

        /// <summary>
        /// Get or set the serial communication rate.
        /// </summary>
        public Baudrate SerialBaudRate
        {
            get
            {
                return baudrate ?? DEFAULT_BAUD_RATE;
            }

            set
            {
                if (!connected)
                {
                    throw new InvalidOperationException("Cannot write to a closed connection. Please call " + nameof(Connect));
                }

                switch (TG_SetBaudrate(connectionId, (int)value))
                {
                    case -1:
                    throw new InvalidOperationException($"Invalid connection ID: {connectionId.ToString(CultureInfo.CurrentCulture)}");
                    case -2:
                    throw new InvalidOperationException(nameof(value) + " is not a valid TG_BAUD_* value.");
                    case -3:
                    throw new Exception("An error occured while attempting to set the baudrate on the device.");
                    case -4:
                    throw new InvalidOperationException("Device is connected to a file stream, not a device.");
                }

                baudrate = value;
            }
        }

        /// <summary>
        /// Connect to a device.
        /// </summary>
        /// <param name="serialPortName"></param>
        /// <param name="format"></param>
        public void Connect(string serialPortName, SerialDataFormat format)
        {
            Connect(serialPortName, SerialBaudRate, format);
        }

        /// <summary>
        /// Connect to a device.
        /// </summary>
        /// <param name="serialPortName"></param>
        /// <param name="baudrate"></param>
        /// <param name="format"></param>
        /// <exception cref="InvalidOperationException">
        /// Thrown when a valid connection ID hasn't been allocated, or when <paramref name="baudrate"/> is invalid for the device, or when <paramref name="format"/> is invalid for the device.
        /// </exception>
        /// <exception cref="Exception">
        /// Thrown when <paramref name="serialPortName"/> can't be opened for communication.
        /// </exception>
        public void Connect(string serialPortName, Baudrate baudrate = DEFAULT_BAUD_RATE, SerialDataFormat format = DEFAULT_SERIAL_FORMAT)
        {
            if (connected)
            {
                Disconnect();
            }

            switch (TG_Connect(connectionId, serialPortName, (int)baudrate, (int)format))
            {
                case -1:
                throw new InvalidOperationException($"Invalid connection ID: {connectionId.ToString(CultureInfo.CurrentCulture)}");
                case -2:
                throw new Exception(serialPortName + " could not be opened as a serial communication port. Check that the name is a valid COM port on your system.");
                case -3:
                throw new InvalidOperationException($"{nameof(baudrate)} is not a valid TG_BAUD_* value.");
                case -4:
                throw new InvalidOperationException($"{nameof(format)} is not a valid TG_STREAM_* type.");
            }

            this.baudrate = baudrate;
            connected = true;
        }

        public bool EnableAutoRead
        {
            get
            {
                return autoReadEnabled;
            }

            set
            {
                switch (TG_EnableAutoRead(connectionId, value ? 1 : 0))
                {
                    case -1:
                    throw new InvalidOperationException($"Invalid connection ID: {connectionId.ToString(CultureInfo.CurrentCulture)}");
                    case -2:
                    throw new Exception("Unable to start auto-reading.");
                    case -3:
                    throw new Exception("Error while disabling auto-reading.");
                }

                autoReadEnabled = value;
            }
        }

        public string StreamLogFileName
        {
            get
            {
                return streamFileName;
            }

            set
            {
                streamFileName = value;
                EnableStreamLog = !string.IsNullOrEmpty(streamFileName);
            }
        }

        public bool EnableStreamLog
        {
            get
            {
                return streamLogEnabled;
            }

            set
            {
                if (string.IsNullOrEmpty(streamFileName))
                {
                    value = false;
                }

                streamLogEnabled = false;

                switch (TG_SetStreamLog(connectionId, value ? streamFileName : null))
                {
                    case -1:
                    throw new InvalidOperationException($"Invalid connection ID: {connectionId.ToString(CultureInfo.CurrentCulture)}");
                    case -2:
                    throw new Exception(streamFileName + " could not be opened for writing.");
                }

                streamLogEnabled = true;
            }
        }

        public void WriteStreamLog(int insertTimestamp, string msg)
        {
            if (!EnableStreamLog)
            {
                EnableStreamLog = true;
            }

            if (EnableStreamLog)
            {
                switch (TG_WriteStreamLog(connectionId, insertTimestamp, msg))
                {
                    case -1:
                    throw new InvalidOperationException($"Invalid connection ID: {connectionId.ToString(CultureInfo.CurrentCulture)}");
                    case -2:
                    throw new Exception("Stream log is not open for writing.");
                }
            }
        }

        public string DataLogFileName
        {
            get
            {
                return dataFileName;
            }

            set
            {
                dataFileName = value;
                EnableDataLog = !string.IsNullOrEmpty(dataFileName);
            }
        }

        public bool EnableDataLog
        {
            get
            {
                return dataLogEnabled;
            }

            set
            {
                if (string.IsNullOrEmpty(dataFileName))
                {
                    value = false;
                }

                dataLogEnabled = false;

                switch (TG_SetDataLog(connectionId, value ? dataFileName : null))
                {
                    case -1:
                    throw new InvalidOperationException($"Invalid connection ID: {connectionId.ToString(CultureInfo.CurrentCulture)}");
                    case -2:
                    throw new Exception(dataFileName + " could not be opened for writing.");
                }

                dataLogEnabled = true;
            }
        }

        public void WriteDataLog(int insertTimestamp, string msg)
        {
            if (!EnableDataLog)
            {
                EnableDataLog = true;
            }

            if (EnableDataLog)
            {
                switch (TG_WriteDataLog(connectionId, insertTimestamp, msg))
                {
                    case -1:
                    throw new InvalidOperationException($"Invalid connection ID: {connectionId.ToString(CultureInfo.CurrentCulture)}");
                    case -2:
                    throw new Exception("Stream log is not open for writing.");
                }
            }
        }

        public int ReadPackets(int numPackets)
        {
            if (autoReadEnabled)
            {
                return -1;
            }

            if (!connected)
            {
                throw new InvalidOperationException("Cannot read from a closed connection. Please call " + nameof(Connect));
            }

            var bytesRead = TG_ReadPackets(connectionId, numPackets);
            return bytesRead switch
            {
                -1 => throw new InvalidOperationException($"Invalid connection ID: {connectionId.ToString(CultureInfo.CurrentCulture)}"),
                -2 => 0,
                -3 => throw new Exception("I/O Error"),
                _ => bytesRead,
            };
        }

        public bool DataValueChanged(DataType dataType)
        {
            if (!connected)
            {
                throw new InvalidOperationException("Cannot read from a closed connection. Please call " + nameof(Connect));
            }

            return TG_GetValueStatus(connectionId, dataType) != 0;
        }

        public float? GetDataValue(DataType dataType)
        {
            if (connected)
            {
                return TG_GetValue(connectionId, dataType);
            }
            else
            {
                return null;
            }
        }

        public float? Battery => GetDataValue(DataType.TG_DATA_BATTERY);

        public float? PoorSignal => GetDataValue(DataType.TG_DATA_POOR_SIGNAL);

        public float? Attention => GetDataValue(DataType.TG_DATA_ATTENTION);

        public float? Meditation => GetDataValue(DataType.TG_DATA_MEDITATION);

        public float? Delta => GetDataValue(DataType.TG_DATA_DELTA);

        public float? Theta => GetDataValue(DataType.TG_DATA_THETA);

        public float? Alpha1 => GetDataValue(DataType.TG_DATA_ALPHA1);

        public float? Alpha2 => GetDataValue(DataType.TG_DATA_ALPHA2);

        public float? Beta1 => GetDataValue(DataType.TG_DATA_BETA1);

        public float? Beta2 => GetDataValue(DataType.TG_DATA_BETA2);

        public float? Gamma1 => GetDataValue(DataType.TG_DATA_GAMMA1);

        public float? Gamma2 => GetDataValue(DataType.TG_DATA_GAMMA2);

        /// <summary>
        /// Send a single byte to the device.
        /// </summary>
        /// <param name="b">The byte to send.</param>
        /// <exception cref="InvalidOperationException">
        /// Thrown when there is no connection, or if the connectionID is invalid, or if ThinkGear
        /// adapter is connected to a file stream log of data, rather than a real device.
        /// </exception>
        /// <exception cref="Exception">
        /// Thrown when some other, unexpected error occurs on the device.
        /// </exception>
        public void SendByte(byte b)
        {
            if (!connected)
            {
                throw new InvalidOperationException("Cannot write to a closed connection. Please call " + nameof(Connect));
            }

            switch (TG_SendByte(connectionId, b))
            {
                case -1:
                throw new InvalidOperationException($"Invalid connection ID: {connectionId.ToString(CultureInfo.CurrentCulture)}");
                case -2:
                throw new InvalidOperationException("Device is connected to a file stream, not a device.");
                case -3:
                throw new Exception("I/O Error");
            }
        }

        /// <summary>
        /// Release the connection to the device.
        /// </summary>
        public void Disconnect()
        {
            connected = false;
            TG_Disconnect(connectionId);
        }

        /// <summary>
        /// Tracks whether or not the connection has already been disposed.
        /// </summary>
        private bool valueDisposed;

        /// <summary>
        /// Release and clean up the connection to the device.
        /// </summary>
        /// <param name="disposing"></param>
        private void Dispose(bool disposing)
        {
            if (!valueDisposed)
            {
                if (disposing)
                {
                    if (connected)
                    {
                        Disconnect();
                    }

                    TG_FreeConnection(connectionId);
                }

                valueDisposed = true;
            }
        }

        /// <summary>
        /// Release and clean up the connection to the device.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }
    }
}