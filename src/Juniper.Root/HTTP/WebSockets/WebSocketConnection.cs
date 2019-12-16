using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Juniper.IO;

namespace Juniper.HTTP.WebSockets
{
    public abstract class WebSocketConnection :
        IDisposable
    {
        /// <summary>
        /// The default rx buffer size 1KiB
        /// </summary>
        public const int DEFAULT_RX_BUFFER_SIZE = 1000;

        /// <summary>
        /// The default data buffer size is 10MiB
        /// </summary>
        public const int DEFAULT_DATA_BUFFER_SIZE = 10000000;

        protected readonly WebSocket socket;

        public event EventHandler<string> Message;
        public event EventHandler<byte[]> Data;
        public event EventHandler<DataMessage> DataMessage;
        public event EventHandler<Exception> Error;
        public event EventHandler Connecting;
        public event EventHandler Connected;
        public event EventHandler Closing;
        public event EventHandler Closed;
        public event EventHandler Aborted;

#if DEBUG
        public event EventHandler<string> Debug;
#endif

        private readonly byte[] rxBuffer;
        private readonly ArraySegment<byte> rxSegment;
        private readonly int dataBufferSize;

        protected WebSocketConnection(WebSocket socket, int rxBufferSize = DEFAULT_RX_BUFFER_SIZE, int dataBufferSize = DEFAULT_DATA_BUFFER_SIZE)
        {
            this.socket = socket;
            this.dataBufferSize = dataBufferSize;

            rxBuffer = new byte[rxBufferSize];
            rxSegment = new ArraySegment<byte>(rxBuffer);

            Data += WebSocketConnection_Data;

            Task.Run(Update).ConfigureAwait(false);
        }

        public WebSocketState State
        {
            get
            {
                return socket.State;
            }
        }

        private async Task Update()
        {
            try
            {
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
                var result = await socket
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

        private void WebSocketConnection_Data(object sender, byte[] data)
        {
            if (DataMessage != null)
            {
                var dataMessageDeserializer = new BinaryFactory<DataMessage>();
                if (dataMessageDeserializer.TryDeserialize(data, out var dataMsg))
                {
                    OnDataMessage(dataMsg);
                }
            }
        }

        public Task SendAsync(string msg)
        {
            OnDebug($"Send: {msg}");
            return SendAsync(Encoding.UTF8.GetBytes(msg), WebSocketMessageType.Text);
        }

        public Task SendAsync<T>(string message, T value, ISerializer<T> serializer)
        {
            OnDebug($"Send: {value} => {message}");
            var data = serializer.Serialize(value);
            var dataMessage = new DataMessage(message, data);
            var msgSerializer = new BinaryFactory<DataMessage>();
            return SendAsync(msgSerializer.Serialize(dataMessage));
        }

        public Task SendAsync(byte[] buffer)
        {
            return SendAsync(buffer, WebSocketMessageType.Binary);
        }

        private async Task SendAsync(byte[] buffer, WebSocketMessageType messageType)
        {
            var segment = new ArraySegment<byte>(buffer);
            OnDebug($"Send: {buffer.Length} bytes. Type: {messageType}.");
            await socket
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

            await socket
                .CloseAsync(closeState, closeMessage, CancellationToken.None)
                .ConfigureAwait(false);

            while (State == WebSocketState.Open
                || State == WebSocketState.CloseSent)
            {
                await Task.Yield();
            }
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    socket.Dispose();
                }

                disposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
        }
        #endregion

        private void OnDebug(string msg)
        {
#if DEBUG
            Debug?.Invoke(this, msg);
#endif
        }

        private void OnMessage(string msg)
        {
            Message?.Invoke(this, msg);
        }

        private void OnData(byte[] data)
        {
            Data?.Invoke(this, data);
        }

        private void OnDataMessage(DataMessage dataMsg)
        {
            DataMessage?.Invoke(this, dataMsg);
        }

        private void OnError(string label, Exception exp)
        {
            Error?.Invoke(this, new Exception(label, exp));
        }

        private void OnConnecting()
        {
            Connecting?.Invoke(this, EventArgs.Empty);
        }

        private void OnConnected()
        {
            Connected?.Invoke(this, EventArgs.Empty);
        }

        private void OnClosing()
        {
            Closing?.Invoke(this, EventArgs.Empty);
        }

        private void OnClosed()
        {
            Closed?.Invoke(this, EventArgs.Empty);
        }

        private void OnAborted()
        {
            Aborted?.Invoke(this, EventArgs.Empty);
        }
    }
}
