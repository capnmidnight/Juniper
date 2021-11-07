using Juniper.IO;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.WebSockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Juniper.HTTP
{
    public abstract class WebSocketConnection<WebSocketT> :
        IDisposable
        where WebSocketT : WebSocket
    {
        /// <summary>
        /// The default rx buffer size 1KiB
        /// </summary>
        public const int DEFAULT_RX_BUFFER_SIZE = 1000;

        /// <summary>
        /// The default data buffer size is 10MiB
        /// </summary>
        public const int DEFAULT_DATA_BUFFER_SIZE = 10000000;

        private WebSocketT socket;
        protected WebSocketT Socket
        {
            get => socket;
            set
            {
                socket?.Dispose();
                socket = value;
            }
        }

        public event EventHandler<StringEventArgs> Message;
        public event EventHandler<BufferEventArgs> Data;
        public event EventHandler<DataMessageEventArgs> DataMessage;
        public event EventHandler<ErrorEventArgs> Error;
        public event EventHandler Connecting;
        public event EventHandler Connected;
        public event EventHandler Closing;
        public event EventHandler Closed;
        public event EventHandler Aborted;

#if DEBUG
        public event EventHandler<StringEventArgs> Debug;
#endif

        private readonly byte[] rxBuffer;
        private readonly ArraySegment<byte> rxSegment;
        private readonly int dataBufferSize;

        protected WebSocketConnection(int rxBufferSize = DEFAULT_RX_BUFFER_SIZE, int dataBufferSize = DEFAULT_DATA_BUFFER_SIZE)
        {
            this.dataBufferSize = dataBufferSize;

            rxBuffer = new byte[rxBufferSize];
            rxSegment = new ArraySegment<byte>(rxBuffer);

            Data += WebSocketConnection_Data;

            _ = Task.Run(UpdateAsync).ConfigureAwait(false);
        }

        public WebSocketState State => Socket.State;

        private async Task UpdateAsync()
        {
            try
            {
                while (Socket is null)
                {
                    await Task.Yield();
                }

                while (State == WebSocketState.None)
                {
                    await Task.Yield();
                }

                if (State == WebSocketState.Connecting)
                {
                    OnConnecting();
                }

                while (State == WebSocketState.Connecting)
                {
                    await Task.Yield();
                }

                if (State == WebSocketState.Open)
                {
                    OnConnected();
                }

                while (State == WebSocketState.Open
                    || State == WebSocketState.CloseSent)
                {
                    await ReceiveAsync()
                        .ConfigureAwait(false);
                }
            }
            catch (Exception exp)
            {
                OnError("Update", exp);
            }
            finally
            {
                if (State == WebSocketState.Aborted)
                {
                    OnAborted();
                }
                else if (State == WebSocketState.CloseReceived)
                {
                    await CloseAsync()
                        .ConfigureAwait(false);
                }

                OnClosed();
            }
        }

        private async Task ReceiveAsync()
        {
            var accum = new List<byte>();
            bool done;
            WebSocketMessageType msgType;
            do
            {
                var result = await Socket
                    .ReceiveAsync(rxSegment, CancellationToken.None)
                    .ConfigureAwait(false);
                msgType = result.MessageType;
                accum.AddRange(rxBuffer.Take(result.Count));
                done = result.EndOfMessage;

                if (accum.Count > dataBufferSize)
                {
                    done = true;
                    await CloseAsync(WebSocketCloseStatus.MessageTooBig)
                        .ConfigureAwait(false);
                }

            } while (!done);

            var data = accum.ToArray();

            if (msgType == WebSocketMessageType.Text)
            {
                OnMessage(Encoding.UTF8.GetString(data));
            }
            else if (msgType == WebSocketMessageType.Binary)
            {
                OnData(data);
            }
            else if (msgType == WebSocketMessageType.Close)
            {
                OnDebug(msgType.ToString());
                await CloseAsync()
                    .ConfigureAwait(false);
            }
        }

        private void WebSocketConnection_Data(object sender, BufferEventArgs e)
        {
            if (DataMessage is object)
            {
                var dataMessageDeserializer = new JsonFactory<DataMessageEventArgs>();
                if (dataMessageDeserializer.TryDeserialize(e.Value.ToArray(), out var dataMsg))
                {
                    OnDataMessage(dataMsg);
                }
            }
        }

        public Task SendAsync(string msg)
        {
            if (msg is null)
            {
                throw new ArgumentNullException(nameof(msg));
            }

            OnDebug($"Send: {msg}");
            return SendAsync(Encoding.UTF8.GetBytes(msg), WebSocketMessageType.Text);
        }

        public Task SendAsync<ValueT>(string message, ValueT value, ISerializer<ValueT> serializer)
        {
            if (serializer is null)
            {
                throw new ArgumentNullException(nameof(serializer));
            }

            OnDebug($"Send: {value} => {message}");
            var data = serializer.Serialize(value);
            var dataMessage = new DataMessage(message, data);
            var msgSerializer = new JsonFactory<DataMessage>();
            return SendAsync(msgSerializer.Serialize(dataMessage));
        }

        public Task SendAsync(byte[] buffer)
        {
            if (buffer is null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }

            return SendAsync(buffer, WebSocketMessageType.Binary);
        }

        public Task SendAsync(IReadOnlyCollection<byte> buffer)
        {
            if (buffer is null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }

            return SendAsync(buffer.ToArray(), WebSocketMessageType.Binary);
        }

        private async Task SendAsync(byte[] buffer, WebSocketMessageType messageType)
        {
            if (buffer is null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }

            var segment = new ArraySegment<byte>(buffer.ToArray());
            OnDebug($"Send: {buffer.Length.ToString(CultureInfo.CurrentCulture)} bytes. Type: {messageType}.");
            await Socket
                .SendAsync(segment, messageType, true, CancellationToken.None)
                .ConfigureAwait(false);
        }

        public async Task CloseAsync(WebSocketCloseStatus closeState = WebSocketCloseStatus.NormalClosure)
        {
            OnDebug("Closing");
            OnClosing();

            var closeMessage = closeState == WebSocketCloseStatus.NormalClosure
                ? null
                : closeState.ToString();

            await Socket
                .CloseAsync(closeState, closeMessage, CancellationToken.None)
                .ConfigureAwait(false);

            while (State == WebSocketState.Open
                || State == WebSocketState.CloseSent)
            {
                await Task.Yield();
            }
        }

        #region IDisposable Support
        private bool disposedValue; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Socket.Dispose();
                }

                disposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void OnDebug(string msg)
        {
#if DEBUG
            Debug?.Invoke(this, new StringEventArgs(msg));
#endif
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void OnMessage(string msg)
        {
            Message?.Invoke(this, new StringEventArgs(msg));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void OnData(byte[] data)
        {
            Data?.Invoke(this, new BufferEventArgs(data));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void OnDataMessage(DataMessageEventArgs dataMsg)
        {
            DataMessage?.Invoke(this, dataMsg);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void OnError(string label, Exception exp)
        {
            Error?.Invoke(this, new ErrorEventArgs(new Exception(label, exp)));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void OnConnecting()
        {
            Connecting?.Invoke(this, EventArgs.Empty);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void OnConnected()
        {
            Connected?.Invoke(this, EventArgs.Empty);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void OnClosing()
        {
            Closing?.Invoke(this, EventArgs.Empty);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void OnClosed()
        {
            Closed?.Invoke(this, EventArgs.Empty);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void OnAborted()
        {
            Aborted?.Invoke(this, EventArgs.Empty);
        }
    }
}
