using System;
using System.IO;
using System.Net;

namespace Juniper.HTTP
{
    public class StreamResult : Result<Stream>, IDisposable
    {
        public StreamResult(HttpStatusCode status, string mime, Stream value) :
            base(status, mime, value)
        { }

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
