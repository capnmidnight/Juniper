using System.IO;

namespace Juniper.Progress
{
    /// <summary>
    /// A stream that can report progress on how much it has been consumed.
    /// </summary>
    public class ProgressStream : Stream, IProgress
    {
        /// <summary>
        /// The stream to wrap.
        /// </summary>
        public readonly Stream stream;

        /// <summary>
        /// The length of the stream being wrapped.
        /// </summary>
        private long length;

        /// <summary>
        /// The parent progress tracker to which to forward progress reports.
        /// </summary>
        private readonly IProgress parent;

        /// <summary>
        /// Creates a progress tracker for a stream, using a set length for the tracking.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="length"></param>
        /// <param name="parent"></param>
        public ProgressStream(Stream stream, long length, IProgress parent = null)
        {
            this.parent = parent;
            this.stream = stream;
            this.length = length;
            TotalByteCount = 0;
        }

        /// <summary>
        /// Backing field for <see cref="TotalByteCount"/>.
        /// </summary>
        private long totalRead;

        /// <summary>
        /// The total number of bytes that have been read out of or written
        /// to the wrapped stream.
        /// </summary>
        private long TotalByteCount
        {
            get
            {
                return totalRead;
            }

            set
            {
                totalRead = value;
                this.Report(totalRead, length);
            }
        }

        /// <summary>
        /// Reset the length of the stream. This will change the progress of
        /// the stream read/write tracking.
        /// </summary>
        /// <param name="value"></param>
        public override void SetLength(long value)
        {
            stream.SetLength(value);
            length = value;
            this.Report(totalRead, length);
        }

        /// <summary>
        /// Send the progress report up to the parent.
        /// </summary>
        /// <param name="progress"></param>
        public void Report(float progress)
        {
            Progress = progress;
            parent?.Report(progress);
        }

        /// <summary>
        /// Send the progress report up to the parent.
        /// </summary>
        /// <param name="progress"></param>
        /// <param name="status"></param>
        public void Report(float progress, string status)
        {
            Progress = progress;
            parent?.Report(progress, status);
        }

        /// <summary>
        /// How far we've gotten through the stream.
        /// </summary>
        public float Progress
        {
            get;
            private set;
        }

        /// <summary>
        /// Cleanup the underlying stream.
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                stream.Dispose();
            }
        }

        /// <summary>
        /// Returns true when the underlying stream can be read.
        /// </summary>
        public override bool CanRead
        {
            get
            {
                return stream.CanRead;
            }
        }

        /// <summary>
        /// Returns true when the underlying stream can be randomly repositioned.
        /// </summary>
        public override bool CanSeek
        {
            get
            {
                return stream.CanSeek;
            }
        }

        /// <summary>
        /// Returns true when the underlying stream can be written to.
        /// </summary>
        public override bool CanWrite
        {
            get
            {
                return stream.CanWrite;
            }
        }

        /// <summary>
        /// Returns the length of the underlying stream.
        /// </summary>
        public override long Length
        {
            get
            {
                return length;
            }
        }

        /// <summary>
        /// Returns the read/write position of the underlying stream.
        /// </summary>
        public override long Position
        {
            get
            {
                return stream.Position;
            }

            set
            {
                stream.Position = value;
            }
        }

        /// <summary>
        /// Flushes the underlying stream.
        /// </summary>
        public override void Flush()
        {
            stream.Flush();
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
            var read = stream.Read(buffer, offset, count);
            if (read == 0)
            {
                length = TotalByteCount;
            }
            TotalByteCount += read;
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
            return TotalByteCount = stream.Seek(offset, origin);
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
            stream.Write(buffer, offset, count);
            TotalByteCount += count;
        }
    }
}