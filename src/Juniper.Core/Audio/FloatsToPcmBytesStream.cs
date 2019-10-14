using System.IO;

namespace Juniper.Audio
{
    public class FloatsToPcmBytesStream : AbstractPcmConversionStream
    {
        private byte[] readBuffer;

        public FloatsToPcmBytesStream(Stream sourceStream, int bytesPerFloat)
            : base(sourceStream, bytesPerFloat)
        {
            readBuffer = new byte[sizeof(float)];
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

        protected override unsafe int InternalRead(byte[] buffer, int offset, int count)
        {
            int read = 0;
            while (read < count && sourceStream.Position < sourceStream.Length)
            {
                sourceStream.Read(readBuffer, 0, sizeof(float));

                uint uv = 0;
                for (var b = 0; b < sizeof(float); ++b)
                {
                    uv <<= 8;
                    var c = buffer[offset + b];
                    uv |= c;
                }

                var v = *(float*)&uv;
                int accum = (int)(v * scalar);
                accum >>= shift;

                for (var b = bytesPerFloat; b >= 0; --b)
                {
                    var c = (byte)(accum & 0xff);
                    buffer[offset + b] = c;
                    accum >>= 8;
                }

                read += bytesPerFloat;
            }

            return read;
        }
    }
}
