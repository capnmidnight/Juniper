using System.IO;

namespace Juniper.IO
{
    /// <summary>
    /// A stream that can report progress on how much it has been consumed.
    /// </summary>
    public class ProgressStream : Stream, IProgress, IStreamWrapper
    {
        /// <summary>
        /// The stream to wrap.
        /// </summary>
        public Stream BaseStream { get; }

        private readonly bool ownStream;

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
        public ProgressStream(Stream stream, long length, IProgress parent, bool ownStream)
        {
            BaseStream = stream;
            this.length = length;
            this.parent = parent;
            this.ownStream = ownStream;
            TotalByteCount = 0;
        }

        public Stream SourceStream => BaseStream;

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
            get { return totalRead; }

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
            BaseStream.SetLength(value);
            length = value;
            this.Report(totalRead, length);
        }

        /// <summary>
        /// Send the progress report up to the parent.
        /// </summary>
        /// <param name="progress"></param>
        /// <param name="status"></param>
        public void ReportWithStatus(float progress, string status)
        {
            Status = status;
            Progress = progress;
            parent.Report(Progress, Status);
        }

        public string Status { get; private set; }

        /// <summary>
        /// How far we've gotten through the stream.
        /// </summary>
        public float Progress { get; private set; }

        /// <summary>
        /// Cleanup the underlying stream.
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && ownStream)
            {
                BaseStream.Dispose();
            }

            base.Dispose(disposing);
        }

        /// <summary>
        /// Returns true when the underlying stream can be read.
        /// </summary>
        public override bool CanRead => BaseStream.CanRead;

        /// <summary>
        /// Returns true when the underlying stream can be randomly repositioned.
        /// </summary>
        public override bool CanSeek => BaseStream.CanSeek;

        /// <summary>
        /// Returns true when the underlying stream can be written to.
        /// </summary>
        public override bool CanWrite => BaseStream.CanWrite;

        /// <summary>
        /// Returns the length of the underlying stream.
        /// </summary>
        public override long Length => length;

        /// <summary>
        /// Returns the read/write position of the underlying stream.
        /// </summary>
        public override long Position
        {
            get { return BaseStream.Position; }
            set { BaseStream.Position = value; }
        }

        /// <summary>
        /// Flushes the underlying stream.
        /// </summary>
        public override void Flush()
        {
            BaseStream.Flush();
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
            var read = BaseStream.Read(buffer, offset, count);
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
            return TotalByteCount = BaseStream.Seek(offset, origin);
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
            BaseStream.Write(buffer, offset, count);
            TotalByteCount += count;
        }
    }
}