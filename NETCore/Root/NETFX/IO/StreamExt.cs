using System.Buffers;

namespace System.IO;
public static class StreamExt
{

    public static Task WriteToAsync(this Stream stream, Stream body) =>
        stream.WriteToAsync(null, null, body, CancellationToken.None);

    public static Task WriteToAsync(this Stream stream, Stream body, CancellationToken cancellationToken) =>
        stream.WriteToAsync(null, null, body, cancellationToken);

    public static Task WriteToAsync(this Stream stream, Stream body, long rangeStart, long rangeEnd) =>
        stream.WriteToAsync(rangeStart, rangeEnd, body, CancellationToken.None);

    public static Task WriteToAsync(this Stream stream, Stream body, long rangeStart, long rangeEnd, CancellationToken cancellationToken) =>
        stream.WriteToAsync(rangeStart, rangeEnd, body, cancellationToken);

    public static async Task WriteToAsync(this Stream stream, long? rangeStart, long? rangeEnd, Stream body, CancellationToken cancellationToken)
    {
        if (rangeStart.HasValue && rangeEnd.HasValue)
        {
            const int FRAME_SIZE = 64 * 1024;
            if (rangeStart.Value > 0)
            {
                using var mem = new PooledMemory(FRAME_SIZE);
                var toBurn = rangeStart.Value;
                while (toBurn > 0)
                {
                    var shouldBurn = (int)Math.Min(toBurn, FRAME_SIZE);
                    var wasBurned = await stream.ReadAsync(mem.Mem[..shouldBurn], cancellationToken);
                    toBurn -= wasBurned;
                }
            }

            var rangeLength = rangeEnd.Value - rangeStart.Value;
            await CopyToAsync(stream, body, rangeLength, FRAME_SIZE, cancellationToken);
        }
        else
        {
            await stream.CopyToAsync(body, cancellationToken)
                .ConfigureAwait(false);
        }
    }



    /// <summary>Asynchronously reads the given number of bytes from the source stream and writes them to another stream, using a specified buffer size.</summary>
    /// <returns>A task that represents the asynchronous copy operation.</returns>
    /// <param name="source">The stream from which the contents will be copied.</param>
    /// <param name="destination">The stream to which the contents of the current stream will be copied.</param>
    /// <param name="count">The count of bytes to be copied.</param>
    /// <param name="bufferSize">The size, in bytes, of the buffer. This value must be greater than zero. The default size is 4096.</param>
    /// <param name="cancel">The token to monitor for cancellation requests. The default value is <see cref="P:System.Threading.CancellationToken.None" />.</param>
    public static async Task CopyToAsync(Stream source, Stream destination, long? count, int bufferSize, CancellationToken cancel)
    {
        long? bytesRemaining = count;

        var buffer = ArrayPool<byte>.Shared.Rent(bufferSize);
        try
        {
            while (true)
            {
                // The natural end of the range.
                if (bytesRemaining.HasValue && bytesRemaining.GetValueOrDefault() <= 0)
                {
                    return;
                }

                cancel.ThrowIfCancellationRequested();

                int readLength = buffer.Length;
                if (bytesRemaining.HasValue)
                {
                    readLength = (int)Math.Min(bytesRemaining.GetValueOrDefault(), (long)readLength);
                }
                int read = await source.ReadAsync(buffer.AsMemory(0, readLength), cancel);

                if (bytesRemaining.HasValue)
                {
                    bytesRemaining -= read;
                }

                // End of the source stream.
                if (read == 0)
                {
                    return;
                }

                cancel.ThrowIfCancellationRequested();

                await destination.WriteAsync(buffer.AsMemory(0, read), cancel);
            }
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(buffer);
        }
    }

    private class PooledMemory : IDisposable
    {
        private static readonly ArrayPool<byte> arrayPool = ArrayPool<byte>.Create();
        private readonly byte[] buffer;
        public Memory<byte> Mem { get; private set; }

        public PooledMemory(int size)
        {
            buffer = arrayPool.Rent(size);
            Mem = new Memory<byte>(buffer);
        }

        public void Dispose()
        {
            arrayPool.Return(buffer);
        }
    }
}
