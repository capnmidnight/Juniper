using System.IO;

namespace Juniper.Audio
{
    public class PcmBytesToFloatsStream : AbstractPcmConversionStream
    {
        protected readonly byte[] readBuffer;

        public PcmBytesToFloatsStream(Stream sourceStream, int bytesPerFloat)
            : base(sourceStream, bytesPerFloat)
        {
            readBuffer = new byte[bytesPerFloat];
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

        protected override unsafe int InternalRead(byte[] buffer, int offset, int count)
        {
            int read = 0;
            while (read < count && sourceStream.Position < sourceStream.Length)
            {
                sourceStream.Read(readBuffer, 0, bytesPerFloat);

                int accum = 0;
                for (var b = bytesPerFloat - 1; b >= 0; --b)
                {
                    accum <<= 8;
                    var c = readBuffer[b];
                    accum |= c;
                }

                accum <<= shift;
                var v = accum / scalar;
                uint uv = *(uint*)&v;

                for (var b = 0; b < sizeof(float); ++b)
                {
                    var c = (byte)uv;
                    buffer[offset + b] = c;
                    uv >>= 8;
                }

                read += sizeof(float);
            }

            return read;
        }
    }
}
