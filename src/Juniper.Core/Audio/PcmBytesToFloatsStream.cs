using System.IO;

namespace Juniper.Audio
{
    public class PcmBytesToFloatsStream : AbstractPcmConversionStream
    {
        protected readonly byte[] tempBuffer;

        public PcmBytesToFloatsStream(Stream sourceStream, int bytesPerFloat)
            : base(sourceStream, bytesPerFloat)
        {
            tempBuffer = new byte[bytesPerFloat];
        }

        public override long Length
        {
            get
            {
                return sourceStream.Length * sizeof(float) / bytesPerFloat;
            }
        }

        public override long Position
        {
            get
            {
                return sourceStream.Position * sizeof(float) / bytesPerFloat;
            }

            set
            {
                sourceStream.Position = value * bytesPerFloat / sizeof(float);
            }
        }

        protected override void InternalSetLength(long value)
        {
            sourceStream.SetLength(value * bytesPerFloat / sizeof(float));
        }

        protected override unsafe int InternalRead(byte[] buffer, int offset, int count)
        {
            int read = 0;
            while (read < count && sourceStream.Position < sourceStream.Length)
            {
                sourceStream.Read(tempBuffer, 0, bytesPerFloat);

                int accum = 0;
                for (var b = bytesPerFloat - 1; b >= 0; --b)
                {
                    accum <<= 8;
                    var c = tempBuffer[b];
                    accum |= c;
                }

                accum <<= shift;
                var v = accum / scalar;
                uint uv = *(uint*)&v;

                for (var b = 0; b < sizeof(float); ++b)
                {
                    var c = (byte)uv;
                    buffer[offset + read + b] = c;
                    uv >>= 8;
                }

                read += sizeof(float);
            }

            return read;
        }

        protected override unsafe void InternalWrite(byte[] buffer, int offset, int count)
        {
            int wrote = 0;
            while (wrote < count)
            {
                uint uv = 0;
                for (var b = 0; b < sizeof(float); ++b)
                {
                    uv <<= 8;
                    var c = buffer[offset + wrote + b];
                    uv |= c;
                }

                var v = *(float*)&uv;
                int accum = (int)(v * scalar);
                accum >>= shift;

                for (var b = bytesPerFloat; b >= 0; --b)
                {
                    var c = (byte)(accum & 0xff);
                    tempBuffer[b] = c;
                    accum >>= 8;
                }

                sourceStream.Write(tempBuffer, 0, bytesPerFloat);

                wrote += sizeof(float);
            }
        }
    }
}
