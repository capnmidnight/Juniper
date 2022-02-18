using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Juniper.IO
{
    /// <summary>
    /// When using Entity Framework in ASP.NET Core, this ActionResult can be used to retrieve and stream
    /// a single BLOB to the Response body.
    /// </summary>
    public abstract class AbstractWrappedStream : Stream
    {
        protected abstract Stream GetStream();
        private Stream _sourceStream;
        private Stream SourceStream => _sourceStream ??= GetStream();

        /// <summary>
        /// Creates a new ActionResult that can perform an ADO.NET query against a database and stream the results
        /// to the client through the request body.
        /// </summary>
        /// <param name="db">The Entity Framework database context through which the query will be made.</param>
        /// <param name="size">The size of the file that will be sent. This should be retrieved separately.</param>
        /// <param name="contentType">The content type of the file that will be sent. This should be retrieved separately.</param>
        /// <param name="fileName">The name of the file that will be sent. This should be retrieved separately.</param>
        /// <param name="cacheTime">The number of seconds to tell the client to cache the result.</param>
        /// <param name="makeCommand">A callback function to construct the Command that will perform the query to retrieve the file stream.</param>
        public AbstractWrappedStream()
            : base()
        {
        }

        public override bool CanRead => SourceStream.CanRead;

        public override bool CanSeek => SourceStream.CanSeek;

        public override bool CanWrite => SourceStream.CanWrite;

        public override long Length => SourceStream.Length;

        public override long Position
        {
            get => SourceStream.Position;
            set => SourceStream.Position = value;
        }

        public override void Flush()
        {
            SourceStream.Flush();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return SourceStream.Read(buffer, offset, count);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return SourceStream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            SourceStream.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            SourceStream.Write(buffer, offset, count);
        }

        public override void Close()
        {
            SourceStream.Close();
            base.Close();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                SourceStream.Dispose();
            }
            base.Dispose(disposing);
        }

        public override int WriteTimeout { get => SourceStream.WriteTimeout; set => SourceStream.WriteTimeout = value; }
        public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            return SourceStream.BeginRead(buffer, offset, count, callback, state);
        }
        public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            return SourceStream.BeginWrite(buffer, offset, count, callback, state);
        }
        public override bool CanTimeout => SourceStream.CanTimeout;
        public override void CopyTo(Stream destination, int bufferSize)
        {
            SourceStream.CopyTo(destination, bufferSize);
        }
        public override Task CopyToAsync(Stream destination, int bufferSize, CancellationToken cancellationToken)
        {
            return SourceStream.CopyToAsync(destination, bufferSize, cancellationToken);
        }
        public override async ValueTask DisposeAsync()
        {
            await SourceStream.DisposeAsync();
            await base.DisposeAsync();
        }
        public override int EndRead(IAsyncResult asyncResult)
        {
            return SourceStream.EndRead(asyncResult);
        }
        public override void EndWrite(IAsyncResult asyncResult)
        {
            SourceStream.EndWrite(asyncResult);
        }
        public override Task FlushAsync(CancellationToken cancellationToken)
        {
            return SourceStream.FlushAsync(cancellationToken);
        }
        public override int Read(Span<byte> buffer)
        {
            return SourceStream.Read(buffer);
        }
        public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            return SourceStream.ReadAsync(buffer, offset, count, cancellationToken);
        }
        public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
        {
            return SourceStream.ReadAsync(buffer, cancellationToken);
        }
        public override int ReadByte()
        {
            return SourceStream.ReadByte();
        }
        public override int ReadTimeout { get => SourceStream.ReadTimeout; set => SourceStream.ReadTimeout = value; }
        public override void Write(ReadOnlySpan<byte> buffer)
        {
            SourceStream.Write(buffer);
        }
        public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            return SourceStream.WriteAsync(buffer, offset, count, cancellationToken);
        }
        public override ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
        {
            return SourceStream.WriteAsync(buffer, cancellationToken);
        }
        public override void WriteByte(byte value)
        {
            SourceStream.WriteByte(value);
        }
    }
}
