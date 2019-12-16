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
        private const int DEFAULT_RX_BUFFER_SIZE = 1000;

        /// <summary>
        /// The default data buffer size is 10MiB
        /// </summary>
        private const int DEFAULT_DATA_BUFFER_SIZE = 10000000;

        protected readonly WebSocket socket;
        protected CancellationTokenSource canceller = new CancellationTokenSource();

        public event EventHandler<string> Message;
        public event EventHandler<byte[]> Data;
        public event EventHandler<DataMessage> DataMessage;
        public event EventHandler<Exception> Error;
        public event EventHandler Connecting;
        public event EventHandler Connected;
        public event EventHandler Closing;
        public event EventHandler Closed;
        public event EventHandler Aborted;

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

            Task.Run(Update, canceller.Token)
                .ConfigureAwait(false);
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
                while (socket.State == WebSocketState.None
                    && !canceller.IsCancellationRequested)
                {
                    await Task.Yield();
                }

                if (socket.State == WebSocketState.Connecting
                    && !canceller.IsCancellationRequested)
                {
                    OnConnecting();
                }

                while (socket.State == WebSocketState.Connecting
                    && !canceller.IsCancellationRequested)
                {
                    await Task.Yield();
                }

                if (socket.State == WebSocketState.Open
                    && !canceller.IsCancellationRequested)
                {
                    OnConnected();
                }

                while (socket.State == WebSocketState.Open
                    && !canceller.IsCancellationRequested)
                {
                    await ReceiveAsync()
                        .ContinueWith(LogError, TaskContinuationOptions.OnlyOnFaulted)
                        .ConfigureAwait(false);
                }

                if (socket.State == WebSocketState.Aborted
                    && !canceller.IsCancellationRequested)
                {
                    OnAborted();
                }

                while (socket.State == WebSocketState.CloseSent
                    && !canceller.IsCancellationRequested)
                {
                    await Task.Yield();
                }
            }
            finally
            {
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
                    .ReceiveAsync(rxSegment, canceller.Token)
                    .ConfigureAwait(false);
                msgType = result.MessageType;
                accum.AddRange(rxBuffer.Take(result.Count));
                done = result.EndOfMessage;

                if (accum.Count > dataBufferSize)
                {
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
                OnClosing();
                await CloseAsync()
                    .ConfigureAwait(false);
            }
        }

        public Task SendAsync(string msg)
        {
            return SendAsync(Encoding.UTF8.GetBytes(msg), WebSocketMessageType.Text);
        }

        public Task SendAsync(byte[] buffer)
        {
            return SendAsync(buffer, WebSocketMessageType.Binary);
        }

        public Task SendAsync<T>(string message, T value, ISerializer<T> serializer)
        {
            var data = serializer.Serialize(value);
            var dataMessage = new DataMessage(message, data);
            var msgSerializer = new BinaryFactory<DataMessage>();
            return SendAsync(msgSerializer.Serialize(dataMessage));
        }

        private void WebSocketConnection_Data(object sender, byte[] data)
        {
            var dataMessageDeserializer = new BinaryFactory<DataMessage>();
            if (dataMessageDeserializer.TryDeserialize(data, out var dataMsg))
            {
                OnDataMessage(dataMsg);
            }
        }

        private async Task SendAsync(byte[] buffer, WebSocketMessageType messageType)
        {
            var segment = new ArraySegment<byte>(buffer);
            await socket
                .SendAsync(segment, messageType, true, canceller.Token)
                .ContinueWith(LogError, TaskContinuationOptions.OnlyOnFaulted)
                .ConfigureAwait(false);
        }

        public async Task CloseAsync(WebSocketCloseStatus closeState = WebSocketCloseStatus.NormalClosure)
        {
            canceller.Cancel();
            canceller.Dispose();

            OnClosing();

            canceller = new CancellationTokenSource();

            await socket
                .CloseAsync(closeState, closeState.ToString(), canceller.Token)
                .ContinueWith(LogError, TaskContinuationOptions.OnlyOnFaulted)
                .ConfigureAwait(false);
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    canceller.Cancel();
                    canceller.Dispose();
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

        private void LogError(Task t)
        {
            OnError(t.Exception);
        }

        private void OnError(Exception exp)
        {
            Error?.Invoke(this, exp);
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
