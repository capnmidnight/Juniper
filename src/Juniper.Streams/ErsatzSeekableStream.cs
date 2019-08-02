using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace Juniper.Streams
{
    /// <summary>
    /// Wraps a non-seekable stream (like <see cref="System.Net.ConnectStream"/>) for use
    /// with poorly-designed libraries that expect seekable streams.
    /// </summary>
    public class ErsatzSeekableStream : Stream, IStreamWrapper
    {
        /// <summary>
        /// The stream to wrap.
        /// </summary>
        private Stream stream;

        private long currentPosition;

        /// <summary>
        /// The stream to which to write the cache data.
        /// </summary>
        private MemoryStream ersatzStream;

        /// <summary>
        /// Creates a stream that wraps around another stream, writing the contents out to disk
        /// as they are being read.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="parent"></param>
        public ErsatzSeekableStream(Stream stream)
        {
            this.stream = stream;
            currentPosition = 0;
        }

        private Stream SeekableStream
        {
            get
            {
                if (stream?.CanSeek == true)
                {
                    return stream;
                }
                else if (ersatzStream == null)
                {
                    ersatzStream = new MemoryStream();
                    using (stream)
                    {
                        stream.CopyTo(ersatzStream);
                    }
                    ersatzStream.Flush();
                    ersatzStream.Position = currentPosition;
                }

                return ersatzStream;
            }
        }

        public Stream UnderlyingStream { get { return stream; } }

        private Stream EitherStream
        {
            get
            {
                return ersatzStream ?? stream;
            }
        }

        /// <summary>
        /// Reset the length of the stream. This will change the progress of
        /// the stream read/write tracking.
        /// </summary>
        /// <param name="value"></param>
        public override void SetLength(long value)
        {
            SeekableStream.SetLength(value);
        }

        /// <summary>
        /// Cleanup the underlying stream.
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                stream?.Dispose();
                ersatzStream?.Dispose();
            }
        }

        public override void Close()
        {
            EitherStream.Close();
        }

        /// <summary>
        /// Returns true when the underlying stream can be read.
        /// </summary>
        public override bool CanRead
        {
            get
            {
                return EitherStream.CanRead;
            }
        }

        /// <summary>
        /// Returns true when the underlying stream can be randomly repositioned.
        /// </summary>
        public override bool CanSeek
        {
            get
            {
                return EitherStream.CanSeek;
            }
        }

        /// <summary>
        /// Returns true when the underlying stream can be written to.
        /// </summary>
        public override bool CanWrite
        {
            get
            {
                return EitherStream.CanWrite;
            }
        }

        /// <summary>
        /// Returns true when the underlying stream can time out.
        /// </summary>
        public override bool CanTimeout
        {
            get
            {
                return EitherStream.CanTimeout;
            }
        }

        /// <summary>
        /// Returns the length of the underlying stream.
        /// </summary>
        public override long Length
        {
            get
            {
                return SeekableStream.Length;
            }
        }

        /// <summary>
        /// Returns the read/write position of the underlying stream.
        /// </summary>
        public override long Position
        {
            get
            {
                return SeekableStream.Position;
            }

            set
            {
                SeekableStream.Position = value;
            }
        }

        /// <summary>
        /// Flushes the underlying stream.
        /// </summary>
        public override void Flush()
        {
            EitherStream.Flush();
        }

        [ComVisible(false)]
        public override Task FlushAsync(CancellationToken cancellationToken)
        {
            return EitherStream.FlushAsync(cancellationToken);
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
            return EitherStream.Read(buffer, offset, count);
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
            return currentPosition = SeekableStream.Seek(offset, origin);
        }

        public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            return EitherStream.BeginRead(buffer, offset, count, callback, state);
        }

        public override int EndRead(IAsyncResult asyncResult)
        {
            var read = EitherStream.EndRead(asyncResult);
            currentPosition += read;
            return read;
        }

        [ComVisible(false)]
        public override async Task CopyToAsync(Stream destination, int bufferSize, CancellationToken cancellationToken)
        {
            var buffer = new byte[bufferSize];
            int read;
            while ((read = await ReadAsync(buffer, 0, bufferSize, cancellationToken)) > 0)
            {
                await destination.WriteAsync(buffer, 0, read, cancellationToken);
            }
        }

        [ComVisible(false)]
        public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            var read = await EitherStream.ReadAsync(buffer, offset, count, cancellationToken);
            currentPosition += read;
            return read;
        }

        public override int ReadByte()
        {
            var b = EitherStream.ReadByte();
            if (b > -1)
            {
                ++currentPosition;
            }
            return b;
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

        public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            throw new NotSupportedException();
        }

        public override void EndWrite(IAsyncResult asyncResult)
        {
            throw new NotSupportedException();
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