using System.IO;

using Juniper.Progress;

namespace Juniper.Audio
{
    public class FloatsToPcmBytesStream : AbstractPcmConversionStream
    {
        private BinaryReader floatReader;

        public FloatsToPcmBytesStream(Stream sourceStream, int bytesPerFloat)
            : base(sourceStream, bytesPerFloat)
        {
            floatReader = new BinaryReader(sourceStream);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                floatReader.Dispose();
                floatReader = null;
            }

            base.Dispose(disposing);
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

        protected override int InternalRead(byte[] buffer, int offset, int count)
        {
            int read = 0;
            var mem = new MemoryStream(buffer, offset, count);
            while (read < count && sourceStream.Position < sourceStream.Length)
            {
                var v = floatReader.ReadSingle();

                int accum = (int)(v * scalar);
                accum >>= shift;
                for (var b = 0; b < bytesPerFloat; ++b)
                {
                    var c = (byte)(accum & 0xff);
                    mem.WriteByte(c);
                    accum >>= 8;
                }

                read += bytesPerFloat;
            }

            return read;
        }
    }
}
