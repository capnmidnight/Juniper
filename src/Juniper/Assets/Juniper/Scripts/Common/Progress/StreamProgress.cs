using System;
using System.IO;

namespace Juniper.Progress
{
    public class StreamProgress : Stream, IProgress
    {
        private Stream stream;
        private long length;
        private IProgress parent;

        public StreamProgress(Stream stream, IProgress parent = null)
            : this(stream, stream.Length, parent)
        {
        }

        public StreamProgress(Stream stream, long length, IProgress parent = null)
        {
            this.parent = parent;
            this.stream = stream;
            this.length = length;
            TotalByteCount = 0;
        }

        private long totalRead;
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

        public override void SetLength(long value)
        {
            stream.SetLength(value);
            length = value;
            this.Report(totalRead, length);
        }

        void IProgress<float>.Report(float value)
        {
            parent?.Report(value);
        }

        void IProgress.Report(float progress, string status)
        {
            parent?.Report(progress, status);
        }

        public float Progress
        {
            get
            {
                return parent?.Progress ?? 1;
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

        public override void Write(byte[] buffer, int offset, int count)
        {
            stream.Write(buffer, offset, count);
            TotalByteCount += count;
        }
    }
}
