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
                return ToFloatSpace(sourceStream.Length);
            }
        }

        public override long Position
        {
            get
            {
                return ToFloatSpace(sourceStream.Position);
            }

            set
            {
                sourceStream.Position = ToPCMSpace(value);
            }
        }

        protected override void InternalSetLength(long value)
        {
            sourceStream.SetLength(ToPCMSpace(value));
        }

        protected override int InternalRead(byte[] buffer, int offset, int count)
        {
            int read = 0;
            while (read < count && sourceStream.Position < sourceStream.Length)
            {
                sourceStream.Read(tempBuffer, 0, bytesPerFloat);
                CopyUInt32(tempBuffer, 0, buffer, offset + read);
                read += sizeof(float);
            }

            return read;
        }

        protected override void InternalWrite(byte[] buffer, int offset, int count)
        {
            int wrote = 0;
            while (wrote < count)
            {
                CopyFloat(buffer, offset + wrote, tempBuffer, 0);
                sourceStream.Write(tempBuffer, 0, bytesPerFloat);
                wrote += sizeof(float);
            }
        }
    }
}
