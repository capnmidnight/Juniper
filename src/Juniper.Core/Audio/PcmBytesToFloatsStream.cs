using System.IO;

using Juniper.Progress;

namespace Juniper.Audio
{
    public class PcmBytesToFloatsStream : AbstractPcmConversionStream
    {
        public PcmBytesToFloatsStream(Stream sourceStream, int bytesPerFloat)
            : base(sourceStream, bytesPerFloat)
        { }

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

        protected override int InternalRead(byte[] buffer, int offset, int count)
        {
            var mem = new MemoryStream(buffer, offset, count);
            var writer = new BinaryWriter(mem);
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
                writer.Write(v);

                read += sizeof(float);
            }

            return read;
        }
    }
}
