using System;
using System.Collections.Concurrent;
using System.Globalization;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Juniper.HTTP.Server.Controllers
{
    public sealed class NCSALogger :
        AbstractResponse,
        INCSALogSource,
        IDisposable
    {
        private readonly Stream stream;
        private readonly StreamWriter writer;
        private readonly ConcurrentQueue<string> logs = new ConcurrentQueue<string>();
        private readonly CancellationTokenSource canceller;
        private readonly Thread logger;

        public event EventHandler<StringEventArgs> Log;

        public NCSALogger(FileInfo file)
            : base(int.MaxValue - 1)
        {
            if (file is null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            stream = file.Open(FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
            writer = new StreamWriter(stream)
            {
                AutoFlush = true
            };

            Protocol = HttpProtocols.All;
            Verb = HttpMethods.All;
            ExpectedStatus = 0;

            canceller = new CancellationTokenSource();
            logger = new Thread(WriteLogs);
            logger.Start();
        }

        public NCSALogger(string fileName)
            : this(new FileInfo(fileName.ValidateFileName()))
        { }

        private void WriteLogs()
        {
            lock (canceller)
            {
                while (!canceller.Token.IsCancellationRequested)
                {
                    if (logs.TryDequeue(out var log))
                    {
                        writer.WriteLine(log);
                    }
                }
            }
        }

        public override bool IsMatch(HttpListenerContext context)
        {
            return true;
        }

        public override Task InvokeAsync(HttpListenerContext context)
        {
            var response = context.Response;
            var request = context.Request;
            var remoteAddr = request.RemoteEndPoint.Address;
            var name = context?.User?.Identity?.Name ?? "-";
            var dateStr = DateTime.Now.ToString("dd/MMM/yyyy:HH:mm:ss K", CultureInfo.InvariantCulture);
            var method = request.HttpMethod;
            var path = request.Url.PathAndQuery;
            var status = response.StatusCode;
            var contentLength = response.ContentLength64;

            var logMessage = $"{remoteAddr} - {name} [{dateStr}] \"{method} {path} HTTP/{request.ProtocolVersion}\" {status} {contentLength}";
            OnLog(logMessage);

            logs.Enqueue(logMessage);

            return Task.CompletedTask;
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        private void OnLog(string message)
        {
            Log?.Invoke(this, new StringEventArgs(message));
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    canceller.Cancel();
                    lock (canceller)
                    {
                        canceller.Dispose();
                    }
                    writer.Flush();
                    writer.Dispose();
                    stream.Dispose();
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
