using System.Runtime.InteropServices;

namespace Juniper.IO;

/// <summary>
/// A stream that can cache contents out to a file.
/// </summary>
public class CachingStream : Stream, IStreamWrapper
{
    /// <summary>
    /// The stream to which to write the cache data.
    /// </summary>
    private readonly Stream outStream;

    /// <summary>
    /// Creates a stream that wraps around another stream, writing the contents out to disk
    /// as they are being read.
    /// </summary>
    /// <param name="sourceStream"></param>
    /// <param name="outStream"></param>
    public CachingStream(Stream sourceStream, Stream outStream)
    {
        SourceStream = sourceStream ?? throw new ArgumentNullException(nameof(sourceStream));
        this.outStream = outStream ?? throw new ArgumentNullException(nameof(outStream));
    }

    public Stream SourceStream { get; }

    /// <summary>
    /// Reset the length of the stream. This will change the progress of
    /// the stream read/write tracking.
    /// </summary>
    /// <param name="value"></param>
    public override void SetLength(long value)
    {
        SourceStream.SetLength(value);
    }

    /// <summary>
    /// Cleanup the underlying stream.
    /// </summary>
    /// <param name="disposing"></param>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            SourceStream.Dispose();
            outStream.Dispose();
        }

        try
        {
            base.Dispose(disposing);
        }
        catch { }
    }

    public override void Close()
    {
        SourceStream.Close();
        outStream.Close();
    }

    /// <summary>
    /// Returns true when the underlying stream can be read.
    /// </summary>
    public override bool CanRead => SourceStream.CanRead;

    /// <summary>
    /// Returns true when the underlying stream can be randomly repositioned.
    /// </summary>
    public override bool CanSeek => SourceStream.CanSeek;

    /// <summary>
    /// Returns true when the underlying stream can be written to.
    /// </summary>
    public override bool CanWrite => false;

    /// <summary>
    /// Returns true when the underlying stream can time out.
    /// </summary>
    public override bool CanTimeout => SourceStream.CanTimeout;

    /// <summary>
    /// Returns the length of the underlying stream.
    /// </summary>
    public override long Length => SourceStream.Length;

    /// <summary>
    /// Returns the read/write position of the underlying stream.
    /// </summary>
    public override long Position
    {
        get => SourceStream.Position;

        set
        {
            SourceStream.Position = value;
            outStream.Position = value;
        }
    }

    /// <summary>
    /// Flushes the underlying stream.
    /// </summary>
    public override void Flush()
    {
        SourceStream.Flush();
        outStream.Flush();
    }

    [ComVisible(false)]
    public override Task FlushAsync(CancellationToken cancellationToken)
    {
        return Task.WhenAll(
            SourceStream.FlushAsync(cancellationToken),
            outStream.FlushAsync(cancellationToken));
    }

    /// <summary>
    /// Reads a set number of bytes from the underlying stream,
    /// updating progress along the way.
    /// </summary>
    /// <param name="buffer"></param>
    /// <param name="offset"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public override int Read(byte[] buffer, int offset, int count)
    {
        if (buffer is null)
        {
            throw new ArgumentNullException(nameof(buffer));
        }

        var read = SourceStream.Read(buffer, offset, count);
        outStream.Write(buffer, offset, read);
        return read;
    }

    /// <summary>
    /// Moves the read/write head to a random point in the underlying stream.
    /// The progress tracker will then assume that the bytes up to the new point
    /// have been "read" or "written" for the purpose of tracking.
    /// </summary>
    /// <param name="offset"></param>
    /// <param name="origin"></param>
    /// <returns></returns>
    public override long Seek(long offset, SeekOrigin origin)
    {
        outStream.Seek(offset, origin);
        return SourceStream.Seek(offset, origin);
    }

    /// <summary>
    /// Write a set number of bytes to the underlying stream,
    /// updating progress along the way.
    /// </summary>
    /// <param name="buffer"></param>
    /// <param name="offset"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public override void Write(byte[] buffer, int offset, int count)
    {
        throw new NotSupportedException();
    }

    private int lastRead;

    public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback? callback, object? state)
    {
        if (buffer is null)
        {
            throw new ArgumentNullException(nameof(buffer));
        }

        void wrappedCallback(IAsyncResult result)
        {
            lastRead = SourceStream.EndRead(result);
            outStream.Write(buffer, offset, lastRead);
            callback?.Invoke(result);
        }

        return SourceStream.BeginRead(buffer, offset, count, wrappedCallback, state);
    }

    public override int EndRead(IAsyncResult asyncResult)
    {
        return lastRead;
    }

    public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback? callback, object? state)
    {
        throw new NotSupportedException();
    }

    public override void EndWrite(IAsyncResult asyncResult)
    {
        throw new NotSupportedException();
    }

    [ComVisible(false)]
    public override async Task CopyToAsync(Stream destination, int bufferSize, CancellationToken cancellationToken)
    {
        if (destination is null)
        {
            throw new ArgumentNullException(nameof(destination));
        }

        var buffer = new byte[bufferSize];
        int read;
        while ((read = await ReadAsync(buffer, 0, bufferSize, cancellationToken).ConfigureAwait(false)) > 0)
        {
            await destination
                .WriteAsync(buffer, 0, read, cancellationToken)
                .ConfigureAwait(false);
        }
    }

    [ComVisible(false)]
    public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
    {
        if (buffer is null)
        {
            throw new ArgumentNullException(nameof(buffer));
        }

        var read = await SourceStream
            .ReadAsync(buffer, offset, count, cancellationToken)
            .ConfigureAwait(false);
        await outStream
            .WriteAsync(buffer, offset, read, cancellationToken)
            .ConfigureAwait(false);
        return read;
    }

    public override int ReadByte()
    {
        var b = SourceStream.ReadByte();
        if (b > -1)
        {
            outStream.WriteByte((byte)b);
        }

        return b;
    }

    [ComVisible(false)]
    public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
    {
        throw new NotSupportedException();
    }

    public override void WriteByte(byte value)
    {
        throw new NotSupportedException();
    }
}