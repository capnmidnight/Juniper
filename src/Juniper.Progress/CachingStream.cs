using System.IO;

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
        public CachingStream(Stream stream, string filename)
        {
            inStream = stream;
            outStream = File.OpenWrite(filename);
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
                return inStream.CanWrite;
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
            inStream.Write(buffer, offset, count);
            outStream.Write(buffer, offset, count);
        }
    }
}
