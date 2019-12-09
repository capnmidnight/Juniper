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
    public class WebSocketConnection : IDisposable
    {
        private const int BUFFER_SIZE = 1000;

        private readonly HttpListenerContext httpContext;
        private readonly CancellationTokenSource canceller = new CancellationTokenSource();
        private readonly WebSocket socket;

        private Task tx = Task.CompletedTask;
        private Task rx;

        public event EventHandler<string> Message;
        public event EventHandler<byte[]> Data;
        public event EventHandler<Exception> Error;

        public WebSocketConnection(HttpListenerContext httpContext, WebSocketContext wsContext)
        {
            this.httpContext = httpContext;
            socket = wsContext.WebSocket;
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

        public void Update()
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
            var msgType = WebSocketMessageType.Close;
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

        public void Send(byte[] buffer)
        {
            Send(buffer, WebSocketMessageType.Binary);
        }

        private void Send(byte[] buffer, WebSocketMessageType messageType)
        {
            var segment = new ArraySegment<byte>(buffer);
            tx = tx.ContinueWith(
                _ => socket.SendAsync(segment, messageType, true, canceller.Token),
                canceller.Token)
                .ContinueWith(LogError, TaskContinuationOptions.OnlyOnFaulted);
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
                    httpContext.Response.Close();
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
