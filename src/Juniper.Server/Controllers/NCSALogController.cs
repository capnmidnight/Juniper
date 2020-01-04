using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Juniper.HTTP.Server.Controllers
{
    public sealed class NCSALogController :
        AbstractRequestHandler,
        INCSALogSource,
        IDisposable
    {
        private readonly FileInfo file;

        public event EventHandler<StringEventArgs> Log;

        private readonly Mutex mutex = new Mutex(false, "File Logging");

        public NCSALogController(FileInfo file)
            : base(null, int.MaxValue - 1)
        {
            if (file is null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            this.file = file;
        }

        public override bool IsMatch(HttpListenerContext context)
        {
            return true;
        }

        public override async Task InvokeAsync(HttpListenerContext context)
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

            while (!mutex.WaitOne(0))
            {
                await Task.Yield();
            }

            using var stream = file.Open(FileMode.Append, FileAccess.Write, FileShare.None);
            using var writer = new StreamWriter(stream);
            await writer.WriteLineAsync(logMessage)
                .ConfigureAwait(false);

            mutex.ReleaseMutex();
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
                    mutex.Dispose();
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
