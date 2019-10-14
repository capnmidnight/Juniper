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
                return ToPCMSpace(sourceStream.Length);
            }
        }

        public override long Position
        {
            get
            {
                return ToPCMSpace(sourceStream.Position);
            }

            set
            {
                sourceStream.Position = ToFloatSpace(value);
            }
        }

        protected override void InternalSetLength(long value)
        {
            sourceStream.SetLength(ToFloatSpace(value));
        }

        protected override int InternalRead(byte[] buffer, int offset, int count)
        {
            int read = 0;
            while (read < count && sourceStream.Position < sourceStream.Length)
            {
                sourceStream.Read(tempBuffer, 0, sizeof(float));
                CopyFloat(tempBuffer, 0, buffer, offset + read);
                read += bytesPerFloat;
            }

            return read;
        }

        protected override void InternalWrite(byte[] buffer, int offset, int count)
        {
            int wrote = 0;
            while(wrote < count)
            {
                CopyUInt32(buffer, offset + wrote, tempBuffer, 0);
                sourceStream.Write(tempBuffer, 0, sizeof(float));
                wrote += bytesPerFloat;
            }
        }
    }
}
