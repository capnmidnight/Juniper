using System.IO;

namespace Juniper.Audio
{
    public class FloatsToPcmBytesStream : AbstractPcmConversionStream
    {
        private byte[] tempBuffer;

        public FloatsToPcmBytesStream(Stream sourceStream, int bytesPerFloat)
            : base(sourceStream, bytesPerFloat)
        {
            tempBuffer = new byte[sizeof(float)];
        }

        public override long Length
        {
            get
            {
                return sourceStream.Length * bytesPerFloat / sizeof(float);
            }
        }

        public override long Position
        {
            get
            {
                return sourceStream.Position * bytesPerFloat / sizeof(float);
            }

            set
            {
                sourceStream.Position = value * sizeof(float) / bytesPerFloat;
            }
        }

        protected override void InternalSetLength(long value)
        {
            sourceStream.SetLength(value * sizeof(float) / bytesPerFloat);
        }

        protected override unsafe int InternalRead(byte[] buffer, int offset, int count)
        {
            int read = 0;
            while (read < count && sourceStream.Position < sourceStream.Length)
            {
                sourceStream.Read(tempBuffer, 0, sizeof(float));

                uint uv = 0;
                for (var b = 0; b < sizeof(float); ++b)
                {
                    uv <<= 8;
                    var c = tempBuffer[b];
                    uv |= c;
                }

                var v = *(float*)&uv;
                int accum = (int)(v * scalar);
                accum >>= shift;

                for (var b = bytesPerFloat; b >= 0; --b)
                {
                    var c = (byte)(accum & 0xff);
                    buffer[offset + read + b] = c;
                    accum >>= 8;
                }

                read += bytesPerFloat;
            }

            return read;
        }

        protected override unsafe void InternalWrite(byte[] buffer, int offset, int count)
        {
            int wrote = 0;
            while(wrote < count)
            {
                int accum = 0;
                for(var b = bytesPerFloat; b >= 0; --b)
                {
                    accum <<= 8;
                    var c = buffer[offset + wrote + b];
                    accum |= c;
                }

                accum <<= shift;
                var v = accum / scalar;
                uint uv = *(uint*)&v;

                for(var b = 0; b < sizeof(float); ++b)
                {
                    var c = (byte)uv;
                    tempBuffer[b] = c;
                    uv >>= 8;
                }

                sourceStream.Write(tempBuffer, 0, sizeof(float));

                wrote += bytesPerFloat;
            }
        }
    }
}
