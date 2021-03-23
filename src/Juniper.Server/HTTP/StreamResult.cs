using System;
using System.IO;
using System.Threading.Tasks;

namespace Juniper.HTTP
{
    /// <summary>
    /// Copy a stream to the response
    /// </summary>
    public class StreamResult : AbstractStreamResult
    {
        private readonly Func<Task<Stream>> getStream;

        /// <summary>
        /// Creates a new ActionResult that can write a stream the the response.
        /// </summary>
        /// <param name="contentType">The content type of the stream that will be written. This should be retrieved separately.</param>
        /// <param name="fileName">The name of the file that will be sent. This should be retrieved separately.</param>
        /// <param name="getStream">A callback function to construct the stream to write.</param>
        public StreamResult(string contentType, string fileName, Func<Task<Stream>> getStream)
            : base(contentType, fileName)
        {
            this.getStream = getStream;
        }

        public StreamResult(string contentType, Func<Task<Stream>> getStream)
            : this(contentType, null, getStream)
        { }

        protected override long GetStreamLength(Stream stream)
        {
            return stream.Length;
        }

        protected override async Task ExecuteAsync(Func<Stream, Task> writeStream)
        {
            using var stream = await getStream()
                .ConfigureAwait(false);
            await writeStream(stream)
                .ConfigureAwait(false);
        }
    }
}
