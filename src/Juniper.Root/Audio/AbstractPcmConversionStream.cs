using System.IO;

namespace Juniper.Audio
{
    /// <summary>
    /// An abstract class for which implementing classes my converting representations
    /// of PCM streams to different byte orders, specifically for conversions
    /// between streams of bytes and floats.
    /// </summary>
    public abstract class AbstractPcmConversionStream : Stream
    {
        private const int BITS_PER_INT = sizeof(int) * 8;

        /// <summary>
        /// The original source of PCM data
        /// </summary>
        protected Stream sourceStream;

        /// <summary>
        /// The number of bytes per sample.
        /// </summary>
        protected readonly int bytesPerFloat;

        /// <summary>
        /// The amount to shift the unsigned integer value into a signed Int32
        /// to get the correct handling of negative values.
        /// </summary>
        protected readonly int shift;

        /// <summary>
        /// The amount by which to multiply or divide the input value to get
        /// the right range of output values.
        /// </summary>
        protected readonly float scalar;

        /// <summary>
        /// Creates the PCM conversion handler, for a given sample size in bytes.
        /// </summary>
        /// <param name="sourceStream">The stream from which to read PCM bytes</param>
        /// <param name="bytesPerFloat">The number of bytes per sample.</param>
        protected AbstractPcmConversionStream(Stream sourceStream, int bytesPerFloat)
        {
            this.sourceStream = sourceStream;
            this.bytesPerFloat = bytesPerFloat;

            var bitsPerFloat = bytesPerFloat * 8;
            shift = BITS_PER_INT - bitsPerFloat;
            scalar = (float)System.Math.Pow(2, BITS_PER_INT - 1);
        }

        /// <summary>
        /// Cleans up the underlying stream.
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                sourceStream.Dispose();
                sourceStream = null;
            }

            base.Dispose(disposing);
        }

        /// <summary>
        /// Returns true if the underlying stream is readable.
        /// </summary>
        public override bool CanRead
        {
            get { return sourceStream.CanRead; }
        }

        /// <summary>
        /// Returns true if the underlying stream is seekable.
        /// </summary>summary>
        public override bool CanSeek
        {
            get { return sourceStream.CanSeek; }
        }

        /// <summary>
        /// Returns true if the underlying stream is writable.
        /// </summary>
        public override bool CanWrite
        {
            get { return sourceStream.CanWrite; }
        }

        /// <summary>
        /// Reposition the head of the underlying stream.
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="origin"></param>
        /// <returns></returns>
        public override long Seek(long offset, SeekOrigin origin)
        {
            return sourceStream.Seek(offset, origin);
        }

        /// <summary>
        /// Flushes the underlying stream.
        /// </summary>
        public override void Flush()
        {
            sourceStream.Flush();
        }

        /// <summary>
        /// Change the size of the underlying stream.
        /// </summary>
        /// <param name="value"></param>
        public override void SetLength(long value)
        {
            InternalSetLength(value);
        }

        /// <summary>
        /// Read in bytes from the source format, perform the <see cref="InternalRead(byte[], int, int)"/>
        /// conversion, then return out the bytes that it generated.
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public override int Read(byte[] buffer, int offset, int count)
        {
            return InternalRead(buffer, offset, count);
        }

        /// <summary>
        /// Perform the <see cref="InternalWrite(byte[], int, int)"/> conversion,
        /// then write out the bytes that it generated.
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        public override void Write(byte[] buffer, int offset, int count)
        {
            InternalWrite(buffer, offset, count);
        }

        /// <summary>
        /// Converts a size/position value from n-byte space to 32-bit space
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        protected long ToFloatSpace(long value)
        {
            return value * sizeof(float) / bytesPerFloat;
        }

        /// <summary>
        /// Converts a size/position value from 32-bit space to n-byte space.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        protected long ToPCMSpace(long value)
        {
            return value * bytesPerFloat / sizeof(float);
        }

        /// <summary>
        /// Read in a 4-byte floating point value and convert it to an N-byte PCM value.
        /// </summary>
        /// <param name="inBuffer"></param>
        /// <param name="inOffset"></param>
        /// <param name="outBuffer"></param>
        /// <param name="outOffset"></param>
        protected unsafe void FloatToPCM(byte[] inBuffer, int inOffset, byte[] outBuffer, int outOffset)
        {
            uint uv = 0;
            for (var b = 0; b < sizeof(float); ++b)
            {
                uv <<= 8;
                var c = inBuffer[inOffset + b];
                uv |= c;
            }

            var v = *(float*)&uv;
            var accum = (int)(v * scalar);
            accum >>= shift;

            for (var b = bytesPerFloat - 1; b >= 0; --b)
            {
                var c = (byte)(accum & 0xff);
                outBuffer[outOffset + b] = c;
                accum >>= 8;
            }
        }

        /// <summary>
        /// Read in an N-byte PCM value and convert it to a 4-byte floating point value.
        /// </summary>
        /// <param name="inBuffer"></param>
        /// <param name="inOffset"></param>
        /// <param name="outBuffer"></param>
        /// <param name="outOffset"></param>
        protected unsafe void PCMToFloat(byte[] inBuffer, int inOffset, byte[] outBuffer, int outOffset)
        {
            var accum = 0;
            for (var b = bytesPerFloat - 1; b >= 0; --b)
            {
                accum <<= 8;
                var c = inBuffer[inOffset + b];
                accum |= c;
            }

            accum <<= shift;
            var v = accum / scalar;
            var uv = *(uint*)&v;

            for (var b = 0; b < sizeof(float); ++b)
            {
                var c = (byte)uv;
                outBuffer[outOffset + b] = c;
                uv >>= 8;
            }
        }

        /// <summary>
        /// Implementing classes override this method to provide the correct
        /// conversion process for their given scenario.
        /// </summary>
        /// <param name="value"></param>
        protected abstract void InternalSetLength(long value);

        /// <summary>
        /// Implementing classes override this method to provide the correct
        /// conversion process for their given scenario.
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        protected abstract int InternalRead(byte[] buffer, int offset, int count);

        /// <summary>
        /// Implementing classes override this method to provide the correct
        /// conversion process for their given scenario.
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        protected abstract void InternalWrite(byte[] buffer, int offset, int count);
    }
}
