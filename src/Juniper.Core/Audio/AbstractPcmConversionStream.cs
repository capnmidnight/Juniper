using System.IO;

using static System.Math;

namespace Juniper.Audio
{
    public abstract class AbstractPcmConversionStream : Stream
    {
        protected Stream sourceStream;

        protected readonly int bytesPerFloat;
        protected readonly int shift;
        protected readonly float scalar;

        protected readonly byte[] readBuffer;


        protected AbstractPcmConversionStream(Stream sourceStream, int bytesPerFloat)
        {
            this.sourceStream = sourceStream;
            this.bytesPerFloat = bytesPerFloat;

            var bitsPerFloat = bytesPerFloat * 8;
            shift = sizeof(int) * 8 - bitsPerFloat;
            scalar = (float)Pow(2, sizeof(int) * 8 - 1);
            readBuffer = new byte[bytesPerFloat];
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                sourceStream.Dispose();
                sourceStream = null;
            }

            base.Dispose(disposing);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return sourceStream.Seek(offset, origin);
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return InternalRead(buffer, offset, count);
        }

        protected abstract int InternalRead(byte[] buffer, int offset, int count);

        public override bool CanRead
        {
            get
            {
                return true;
            }
        }

        public override bool CanSeek
        {
            get
            {
                return sourceStream.CanSeek;
            }
        }

        public override bool CanWrite
        {
            get
            {
                return false;
            }
        }

        public override void Flush()
        {
            throw new System.NotSupportedException();
        }

        public override void SetLength(long value)
        {
            throw new System.NotSupportedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new System.NotSupportedException();
        }
    }
}
