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


        protected AbstractPcmConversionStream(Stream sourceStream, int bytesPerFloat)
        {
            this.sourceStream = sourceStream;
            this.bytesPerFloat = bytesPerFloat;

            var bitsPerFloat = bytesPerFloat * 8;
            shift = sizeof(int) * 8 - bitsPerFloat;
            scalar = (float)Pow(2, sizeof(int) * 8 - 1);
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

        public override bool CanRead
        {
            get
            {
                return sourceStream.CanRead;
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
                return sourceStream.CanWrite;
            }
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return sourceStream.Seek(offset, origin);
        }

        public override void Flush()
        {
            sourceStream.Flush();
        }

        public override void SetLength(long value)
        {
            InternalSetLength(value);
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return InternalRead(buffer, offset, count);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            InternalWrite(buffer, offset, count);
        }

        protected long ToFloatSpace(long value)
        {
            return value * sizeof(float) / bytesPerFloat;
        }

        protected long ToPCMSpace(long value)
        {
            return value * bytesPerFloat / sizeof(float);
        }

        protected unsafe void CopyFloat(byte[] inBuffer, int inOffset, byte[] outBuffer, int outOffset)
        {
            uint uv = 0;
            for (var b = 0; b < sizeof(float); ++b)
            {
                uv <<= 8;
                var c = inBuffer[inOffset + b];
                uv |= c;
            }

            var v = *(float*)&uv;
            int accum = (int)(v * scalar);
            accum >>= shift;

            for (var b = bytesPerFloat - 1; b >= 0; --b)
            {
                var c = (byte)(accum & 0xff);
                outBuffer[outOffset + b] = c;
                accum >>= 8;
            }
        }

        protected unsafe void CopyUInt32(byte[] inBuffer, int inOffset, byte[] outBuffer, int outOffset)
        {
            int accum = 0;
            for (var b = bytesPerFloat - 1; b >= 0; --b)
            {
                accum <<= 8;
                var c = inBuffer[inOffset + b];
                accum |= c;
            }

            accum <<= shift;
            var v = accum / scalar;
            uint uv = *(uint*)&v;

            for (var b = 0; b < sizeof(float); ++b)
            {
                var c = (byte)uv;
                outBuffer[outOffset + b] = c;
                uv >>= 8;
            }
        }

        protected abstract void InternalSetLength(long value);

        protected abstract int InternalRead(byte[] buffer, int offset, int count);

        protected abstract void InternalWrite(byte[] buffer, int offset, int count);
    }
}
