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
        private readonly StreamWriter writer;
        private readonly ConcurrentQueue<string> logs = new ConcurrentQueue<string>();
        private readonly CancellationTokenSource canceller;
        private readonly Thread logger;

        public event EventHandler<StringEventArgs> Log;

        public NCSALogger(FileInfo file)
            : base(int.MaxValue - 1,
                  HttpProtocols.All,
                  HttpMethods.All,
                  AnyAuth,
                  MediaType.All)
        {
            if (file is null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            writer = file.AppendText();
            writer.AutoFlush = true;

            canceller = new CancellationTokenSource();
            logger = new Thread(WriteLogs);
            logger.Start();
        }

        private static FileInfo ValidateFileName(string fileName)
        {
            if (fileName is null)
            {
                throw new ArgumentNullException(nameof(fileName));
            }

            if (fileName.Length == 0)
            {
                throw new ArgumentException("path must not be empty string", nameof(fileName));
            }

            return new FileInfo(fileName);
        }

        public NCSALogger(string fileName)
            : this(ValidateFileName(fileName))
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

                    Thread.Yield();
                }
            }
        }

        public override bool IsMatch(HttpListenerContext context)
        {
            return true;
        }

        public override Task InvokeAsync(HttpListenerContext context)
        {
            var logMessage = FormatLogMessage(context);
            OnLog(logMessage);
            logs.Enqueue(logMessage);
            return Task.CompletedTask;
        }

        public static string FormatLogMessage(HttpListenerContext context)
        {
            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }

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
            return logMessage;
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        private void OnLog(string message)
        {
            Log?.Invoke(this, new StringEventArgs(message));
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        private void Dispose(bool disposing)
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
