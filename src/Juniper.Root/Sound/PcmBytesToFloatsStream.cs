using System.IO;

namespace Juniper.Sound
{
    /// <summary>
    /// Converts streams of n-bit PCM samples to 32-bit floating point samples,
    /// where n is 8, 16, 24, or 32 so that it can be used with <see cref="BinaryReader"/>
    /// and <see cref="BinaryWriter"/> to read and write the floating point values
    /// easily.
    /// </summary>
    public class PcmBytesToFloatsStream : AbstractPcmConversionStream
    {
        /// <summary>
        /// A small buffer for reading in a single sample at a time.
        /// </summary>
        protected readonly byte[] tempBuffer;

        /// <summary>
        /// Creates a wrapper around a stream to convert PCM data to floating
        /// point samples.
        /// </summary>
        /// <param name="sourceStream">The PCM data stream</param>
        /// <param name="bytesPerFloat">The number of bytes per sample (1, 2, 3, or 4)</param>
        public PcmBytesToFloatsStream(Stream sourceStream, int bytesPerFloat)
            : base(sourceStream, bytesPerFloat)
        {
            tempBuffer = new byte[bytesPerFloat];
        }

        /// <summary>
        /// The length of the stream, after conversion.
        /// </summary>
        /// <remarks>The length of the underlying stream will be multiplied if
        /// the bytesPerFloat value is smaller than 4. For example, a 2-byte-per-float
        /// PCM stream of length 10 will show a length of 20, as there are 2 times
        /// as many bytes in the output floating-point sample stream.</remarks>
        public override long Length
        {
            get { return ToFloatSpace(sourceStream.Length); }
        }

        /// <summary>
        /// Gets or sets the position of the stream read head relative to the
        /// floating-point output length.
        /// </summary>
        public override long Position
        {
            get { return ToFloatSpace(sourceStream.Position); }


            set { sourceStream.Position = ToPCMSpace(value); }
        }

        /// <summary>
        /// Sets the length of the underlying stream according to the converted
        /// length of float bytes.
        /// </summary>
        /// <param name="value"></param>
        protected override void InternalSetLength(long value)
        {
            sourceStream.SetLength(ToPCMSpace(value));
        }

        /// <summary>
        /// Reads <see cref="AbstractPcmConversionStream.bytesPerSample"/> PCM bytes
        /// at a time, outputting 4 floating-point bytes per sample read.
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        protected override int InternalRead(byte[] buffer, int offset, int count)
        {
            var read = 0;
            while (read + sizeof(float) - 1 < count)
            {
                sourceStream.Read(tempBuffer, 0, bytesPerSample);
                PCMToFloat(tempBuffer, 0, buffer, offset + read);
                read += sizeof(float);
            }

            return read;
        }

        /// <summary>
        /// Writes <see cref="AbstractPcmConversionStream.bytesPerSample"/> PCM bytes
        /// at a time, inputting from 4 floating-point bytes per sample write.
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        protected override void InternalWrite(byte[] buffer, int offset, int count)
        {
            var wrote = 0;
            while (wrote < count)
            {
                FloatToPCM(buffer, offset + wrote, tempBuffer, 0);
                sourceStream.Write(tempBuffer, 0, bytesPerSample);
                wrote += sizeof(float);
            }
        }
    }
}
