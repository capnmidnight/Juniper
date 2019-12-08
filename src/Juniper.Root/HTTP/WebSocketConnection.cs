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

        private readonly Queue<string> rxQueue = new Queue<string>();

        private Task tx = Task.CompletedTask;
        private Task rx = Task.CompletedTask;

        public bool QueueMessages
        {
            get;
            set;
        }

        public event EventHandler<string> Message;

        public WebSocketConnection(HttpListenerContext httpContext, WebSocketContext wsContext)
        {
            this.httpContext = httpContext;
            socket = wsContext.WebSocket;
            QueueMessages = true;
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
            while (rxQueue.Count > 0
                && Message != null)
            {
                var item = rxQueue.Dequeue();
                Message.Invoke(this, item);
            }

            if (!rx.IsRunning())
            {
                rx = rx.ContinueWith(HandleSocketRx, canceller.Token);
            }
        }

        public void Send(string item)
        {
            if (QueueMessages)
            {
                tx = tx.ContinueWith(HandleSocketTx(item), canceller.Token);
            }
            else if (!tx.IsRunning())
            {
                tx = Task.Run(() => SendAsync(item), canceller.Token);
            }
        }

        private Func<Task, Task> HandleSocketTx(string item)
        {
            return _ => SendAsync(item);
        }

        private Task SendAsync(string item)
        {
            var buffer = Encoding.UTF8.GetBytes(item);
            var segment = new ArraySegment<byte>(buffer);
            return socket.SendAsync(segment, WebSocketMessageType.Text, true, canceller.Token);
        }

        private async Task HandleSocketRx(Task _)
        {
            var data = new List<byte>();
            var buffer = new byte[BUFFER_SIZE];
            var seg = new ArraySegment<byte>(buffer);
            bool done;
            do
            {
                var result = await socket.ReceiveAsync(seg, canceller.Token)
                    .ConfigureAwait(false);
                var rx = new byte[result.Count];
                Array.Copy(buffer, 0, rx, 0, result.Count);
                data.AddRange(rx);
                done = result.EndOfMessage;
            } while (!done);

            var msg = Encoding.UTF8.GetString(data.ToArray());

            if (QueueMessages)
            {
                rxQueue.Enqueue(msg);
            }
            else
            {
                Message?.Invoke(this, msg);
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
                    canceller.Cancel();
                    canceller.Dispose();
                    httpContext.Response.Close();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

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
