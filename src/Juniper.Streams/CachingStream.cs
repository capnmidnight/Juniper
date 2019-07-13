using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace Juniper.Progress
{
    /// <summary>
    /// A stream that can cache contents out to a file.
    /// </summary>
    public class CachingStream : Stream
    {
        /// <summary>
        /// The stream to wrap.
        /// </summary>
        private readonly Stream inStream;

        /// <summary>
        /// The stream to which to write the cache data.
        /// </summary>
        private readonly Stream outStream;

        /// <summary>
        /// Creates a stream that wraps around another stream, writing the contents out to disk
        /// as they are being read.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="parent"></param>
        public CachingStream(Stream stream, FileInfo file)
        {
            inStream = new ErsatzSeekableStream(stream);
            file.Directory.Create();
            outStream = file.OpenWrite();
        }

        public CachingStream(Stream stream, string fileName)
            : this(stream, new FileInfo(fileName))
        {
        }

        /// <summary>
        /// Reset the length of the stream. This will change the progress of
        /// the stream read/write tracking.
        /// </summary>
        /// <param name="value"></param>
        public override void SetLength(long value)
        {
            inStream.SetLength(value);
        }

        /// <summary>
        /// Cleanup the underlying stream.
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                inStream.Dispose();
                outStream.Dispose();
            }
        }

        public override void Close()
        {
            inStream.Close();
            outStream.Close();
        }

        /// <summary>
        /// Returns true when the underlying stream can be read.
        /// </summary>
        public override bool CanRead
        {
            get
            {
                return inStream.CanRead;
            }
        }

        /// <summary>
        /// Returns true when the underlying stream can be randomly repositioned.
        /// </summary>
        public override bool CanSeek
        {
            get
            {
                return inStream.CanSeek;
            }
        }

        /// <summary>
        /// Returns true when the underlying stream can be written to.
        /// </summary>
        public override bool CanWrite
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Returns true when the underlying stream can time out.
        /// </summary>
        public override bool CanTimeout
        {
            get
            {
                return inStream.CanTimeout;
            }
        }

        /// <summary>
        /// Returns the length of the underlying stream.
        /// </summary>
        public override long Length
        {
            get
            {
                return inStream.Length;
            }
        }

        /// <summary>
        /// Returns the read/write position of the underlying stream.
        /// </summary>
        public override long Position
        {
            get
            {
                return inStream.Position;
            }

            set
            {
                inStream.Position = value;
                outStream.Position = value;
            }
        }

        /// <summary>
        /// Flushes the underlying stream.
        /// </summary>
        public override void Flush()
        {
            inStream.Flush();
            outStream.Flush();
        }

        [ComVisible(false)]
        public override Task FlushAsync(CancellationToken cancellationToken)
        {
            return Task.WhenAll(
                inStream.FlushAsync(cancellationToken),
                outStream.FlushAsync(cancellationToken));
        }

        /// <summary>
        /// Reads a set number of bytes from the underlying stream,
        /// updating progress along the way.
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public override int Read(byte[] buffer, int offset, int count)
        {
            var read = inStream.Read(buffer, offset, count);
            outStream.Write(buffer, offset, read);
            return read;
        }

        /// <summary>
        /// Moves the read/write head to a random point in the underlying stream.
        /// The progress tracker will then assume that the bytes up to the new point
        /// have been "read" or "written" for the purpose of tracking.
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="origin"></param>
        /// <returns></returns>
        public override long Seek(long offset, SeekOrigin origin)
        {
            outStream.Seek(offset, origin);
            return inStream.Seek(offset, origin);
        }

        /// <summary>
        /// Write a set number of bytes to the underlying stream,
        /// updating progress along the way.
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }

        private int lastRead;
        public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            async void wrappedCallback(IAsyncResult result)
            {
                lastRead = inStream.EndRead(result);
                await outStream.WriteAsync(buffer, offset, lastRead);
                callback(result);
            }

            return inStream.BeginRead(buffer, offset, count, wrappedCallback, state);
        }

        public override int EndRead(IAsyncResult asyncResult)
        {
            return lastRead;
        }

        public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            throw new NotSupportedException();
        }

        public override void EndWrite(IAsyncResult asyncResult)
        {
            throw new NotSupportedException();
        }

        [ComVisible(false)]
        public override async Task CopyToAsync(Stream destination, int bufferSize, CancellationToken cancellationToken)
        {
            var buffer = new byte[bufferSize];
            int read;
            while((read = await ReadAsync(buffer, 0, bufferSize, cancellationToken)) > 0)
            {
                await destination.WriteAsync(buffer, 0, read, cancellationToken);
            }
        }

        [ComVisible(false)]
        public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            var read = await inStream.ReadAsync(buffer, offset, count, cancellationToken);
            await outStream.WriteAsync(buffer, offset, read, cancellationToken);
            return read;
        }

        public override int ReadByte()
        {
            var b = inStream.ReadByte();
            if (b > -1)
            {
                outStream.WriteByte((byte)b);
            }
            return b;
        }

        [ComVisible(false)]
        public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            throw new NotSupportedException();
        }

        public override void WriteByte(byte value)
        {
            throw new NotSupportedException();
        }
    }
}
