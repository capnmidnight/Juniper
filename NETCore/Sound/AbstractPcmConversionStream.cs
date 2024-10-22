namespace Juniper.Sound;

/// <summary>
/// An abstract class for which implementing classes my converting representations
/// of PCM streams to different byte orders, specifically for conversions
/// between streams of bytes and floats.
/// </summary>
public abstract class AbstractPcmConversionStream : Stream
{
    /// <summary>
    /// The original source of PCM data
    /// </summary>
    protected Stream SourceStream { get; set; }

    /// <summary>
    /// The number of bytes per sample.
    /// </summary>
    protected int BytesPerSample { get; }

    /// <summary>
    /// The amount to shift the unsigned integer value into a signed Int32
    /// to get the correct handling of negative values.
    /// </summary>
    protected int Shift { get; }

    /// <summary>
    /// The amount by which to multiply or divide the input value to get
    /// the right range of output values.
    /// </summary>
    protected float Scalar { get; }

    /// <summary>
    /// Creates the PCM conversion handler, for a given sample size in bytes.
    /// </summary>
    /// <param name="sourceStream">The stream from which to read PCM bytes</param>
    /// <param name="bytesPerSample">The number of bytes per sample.</param>
    protected AbstractPcmConversionStream(Stream sourceStream, int bytesPerSample)
    {
        SourceStream = sourceStream;
        BytesPerSample = bytesPerSample;

        var bitsPerSample = (int)Units.Bytes.Bits(bytesPerSample);
        Shift = Units.Bits.PER_INT - bitsPerSample;
        Scalar = (float)Math.Pow(2, Units.Bits.PER_INT - 1);
    }

    /// <summary>
    /// Cleans up the underlying stream.
    /// </summary>
    /// <param name="disposing"></param>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            SourceStream.Dispose();
        }

        base.Dispose(disposing);
    }

    /// <summary>
    /// Returns true if the underlying stream is readable.
    /// </summary>
    public override bool CanRead =>
        SourceStream.CanRead;

    /// <summary>
    /// Returns true if the underlying stream is seekable.
    /// </summary>summary>
    public override bool CanSeek =>
        SourceStream.CanSeek;

    /// <summary>
    /// Returns true if the underlying stream is writable.
    /// </summary>
    public override bool CanWrite =>
        SourceStream.CanWrite;

    /// <summary>
    /// Reposition the head of the underlying stream.
    /// </summary>
    /// <param name="offset"></param>
    /// <param name="origin"></param>
    /// <returns></returns>
    public override long Seek(long offset, SeekOrigin origin)
    {
        return SourceStream.Seek(offset, origin);
    }

    /// <summary>
    /// Flushes the underlying stream.
    /// </summary>
    public override void Flush()
    {
        SourceStream.Flush();
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
        return value * sizeof(float) / BytesPerSample;
    }

    /// <summary>
    /// Converts a size/position value from 32-bit space to n-byte space.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    protected long ToPCMSpace(long value)
    {
        return value * BytesPerSample / sizeof(float);
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
        if (inBuffer is null)
        {
            throw new System.ArgumentNullException(nameof(inBuffer));
        }

        if (outBuffer is null)
        {
            throw new System.ArgumentNullException(nameof(outBuffer));
        }

        uint uv = 0;
        for (var b = 0; b < sizeof(float); ++b)
        {
            uv <<= Units.Bits.PER_BYTE;
            var c = inBuffer[inOffset + b];
            uv |= c;
        }

        var v = *(float*)&uv;
        var accum = (int)(v * Scalar);
        accum >>= Shift;

        for (var b = BytesPerSample - 1; b >= 0; --b)
        {
            var c = (byte)(accum & byte.MaxValue);
            outBuffer[outOffset + b] = c;
            accum >>= Units.Bits.PER_BYTE;
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
        if (inBuffer is null)
        {
            throw new System.ArgumentNullException(nameof(inBuffer));
        }

        if (outBuffer is null)
        {
            throw new System.ArgumentNullException(nameof(outBuffer));
        }

        var accum = 0;
        for (var b = BytesPerSample - 1; b >= 0; --b)
        {
            accum <<= Units.Bits.PER_BYTE;
            var c = inBuffer[inOffset + b];
            accum |= c;
        }

        accum <<= Shift;
        var v = accum / Scalar;
        var uv = *(uint*)&v;

        for (var b = 0; b < sizeof(float); ++b)
        {
            var c = (byte)uv;
            outBuffer[outOffset + b] = c;
            uv >>= Units.Bits.PER_BYTE;
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
