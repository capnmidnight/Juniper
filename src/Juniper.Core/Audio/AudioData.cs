using System;
using System.IO;

namespace Juniper.Audio
{
    /// <summary>
    /// The raw bytes and dimensions of an audio file that has been loaded either off disk or across the 'net.
    /// </summary>
    public class AudioData : IDisposable
    {
        /// <summary>
        /// Reads a stream and fills a PCM buffer with data.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="data"></param>
        public static void FillBuffer(Stream stream, float[] data)
        {
            var buf = new byte[sizeof(float) / sizeof(byte)];
            for (var i = 0; i < data.Length; ++i)
            {
                stream.Read(buf, 0, buf.Length);
                data[i] = BitConverter.ToSingle(buf, 0);
            }
        }

        public readonly HTTP.MediaType.Audio contentType;
        public readonly Stream stream;
        public readonly long samples;
        public readonly int channels;
        public readonly int frequency;

        public AudioData(HTTP.MediaType.Audio contentType, long samples, int channels, int frequency, Stream stream)
        {
            this.contentType = contentType;
            this.samples = samples;
            this.channels = channels;
            this.frequency = frequency;
            this.stream = stream;
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    stream.Dispose();
                }

                disposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}