using System.IO;

namespace Juniper.Progress
{
    public class StreamProgress : Stream, IProgress
    {
        private Stream stream;
        private long totalRead;
        private long length;
        private IProgressReceiver parent;

        public StreamProgress()
        {
        }

        public StreamProgress(Stream stream, IProgressReceiver parent = null)
        {
            SetStream(stream, stream.Length, parent);
        }

        internal void SetStream(Stream stream, long length, IProgressReceiver parent = null)
        {
            this.parent = parent;
            this.stream = stream;
            this.length = length;
        }

        private long TotalByteCount
        {
            get
            {
                return totalRead;
            }

            set
            {
                totalRead = value;
                parent?.SetProgress(totalRead, length);
            }
        }

        public float Progress
        {
            get
            {
                if (stream == null)
                {
                    return 0;
                }
                else if (length == 0)
                {
                    return 1;
                }
                else
                {
                    return (float)TotalByteCount / length;
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                stream.Dispose();
            }
        }

        public override bool CanRead
        {
            get
            {
                return stream.CanRead;
            }
        }

        public override bool CanSeek
        {
            get
            {
                return stream.CanSeek;
            }
        }

        public override bool CanWrite
        {
            get
            {
                return stream.CanWrite;
            }
        }

        public override long Length
        {
            get
            {
                return stream.Length;
            }
        }

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

        public override void Flush()
        {
            stream.Flush();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            var read = stream.Read(buffer, offset, count);
            TotalByteCount += read;
            return read;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return TotalByteCount = stream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            stream.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            stream.Write(buffer, offset, count);
            TotalByteCount += count;
        }
    }
}