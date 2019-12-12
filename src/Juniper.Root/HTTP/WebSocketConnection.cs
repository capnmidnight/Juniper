using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Juniper.HTTP
{
    public class ServerWebSocketConnection : WebSocketConnection
    {
        public readonly HttpListenerContext httpContext;

        public ServerWebSocketConnection(HttpListenerContext httpContext, WebSocket socket)
            : base(socket)
        {
            this.httpContext = httpContext;
        }

        private bool disposedValue;
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (!disposedValue)
            {
                if (disposing)
                {
                    httpContext.Response.Close();
                }

                disposedValue = true;
            }
        }
    }
    public class WebSocketConnection :
        IDisposable,
        IUpdatable
    {
        private const int BUFFER_SIZE = 1000;

        private readonly CancellationTokenSource canceller = new CancellationTokenSource();
        private readonly WebSocket socket;
        private readonly Uri connectionURI;

        private Task tx = Task.CompletedTask;
        private Task rx;

        public event EventHandler<string> Message;
        public event EventHandler<byte[]> Data;
        public event EventHandler<Exception> Error;

        public event EventHandler Disposed;

        private void OnDisposed()
        {
            Disposed?.Invoke(this, EventArgs.Empty);
        }

        public WebSocketConnection(WebSocket socket)
        {
            this.socket = socket;
        }

        public WebSocketConnection(Uri uri)
        {
            socket = new ClientWebSocket();
            connectionURI = uri;
        }

        public async Task ConnectAsync()
        {
            if(socket is ClientWebSocket clientSocket)
            {
                await clientSocket.ConnectAsync(connectionURI, canceller.Token); 
            }
            else
            {
                throw new InvalidOperationException("This is not a client socket");
            }
        }

        public WebSocketState State
        {
            get
            {
                return socket.State;
            }
        }

        public void Close()
        {
            if (tx.IsRunning()
                || rx.IsRunning())
            {
                canceller.Cancel();
            }

            tx = rx = socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Close requested", canceller.Token);
        }

        public void Update(object sender, EventArgs args)
        {
            if (!rx.IsRunning())
            {
                rx = Task.Run(HandleSocketRx, canceller.Token)
                    .ContinueWith(LogError, TaskContinuationOptions.OnlyOnFaulted);
            }
        }

        private void OnError(Exception exp)
        {
            Error?.Invoke(this, exp);
        }

        private void LogError(Task t)
        {
            OnError(t.Exception);
        }

        private async Task HandleSocketRx()
        {
            var data = new List<byte>();
            var buffer = new byte[BUFFER_SIZE];
            var seg = new ArraySegment<byte>(buffer);
            bool done;
            WebSocketMessageType msgType;
            do
            {
                var result = await socket.ReceiveAsync(seg, canceller.Token)
                    .ConfigureAwait(false);
                msgType = result.MessageType;
                var rx = new byte[result.Count];
                Array.Copy(buffer, 0, rx, 0, result.Count);
                data.AddRange(rx);
                done = result.EndOfMessage;
            } while (!done);

            buffer = data.ToArray();

            if (msgType == WebSocketMessageType.Text)
            {
                var msg = Encoding.UTF8.GetString(buffer);
                Message?.Invoke(this, msg);
            }
            else if (msgType == WebSocketMessageType.Binary)
            {
                Data?.Invoke(this, buffer);
            }
            else if (msgType == WebSocketMessageType.Close)
            {
                Close();
            }
        }

        public void Send(string msg)
        {
            Send(Encoding.UTF8.GetBytes(msg), WebSocketMessageType.Text);
        }

        public Task SendAsync(string msg)
        {
            return SendAsync(Encoding.UTF8.GetBytes(msg), WebSocketMessageType.Text);
        }

        public void Send(byte[] buffer)
        {
            Send(buffer, WebSocketMessageType.Binary);
        }

        public Task SendAsync(byte[] buffer)
        {
            return SendAsync(buffer, WebSocketMessageType.Binary);
        }

        private void Send(byte[] buffer, WebSocketMessageType messageType)
        {
            tx = tx.ContinueWith( _=> SendAsync(buffer, messageType))
                .ContinueWith(LogError, TaskContinuationOptions.OnlyOnFaulted);
        }

        private Task SendAsync(byte[] buffer, WebSocketMessageType messageType)
        {
            var segment = new ArraySegment<byte>(buffer);
            return socket.SendAsync(segment, messageType, true, canceller.Token);
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    OnDisposed();
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

    }
}
