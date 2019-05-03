using System;
using System.IO;
using System.Net;

namespace Juniper.Data
{
    public class Result<T>
    {
        public readonly HttpStatusCode Status;
        public readonly T Value;
        public Result(HttpStatusCode status, T value)
        {
            Status = status;
            Value = value;
        }
    }

    public class StreamResult : Result<Stream>, IDisposable
    {
        internal StreamResult(HttpStatusCode status, Stream value) : base(status, value) { }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Value.Dispose();
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
