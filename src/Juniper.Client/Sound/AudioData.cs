using System;
using System.IO;

namespace Juniper.Sound
{
    /// <summary>
    /// The raw bytes and dimensions of an audio file that has been loaded either off disk or across the 'net.
    /// </summary>
    public sealed class AudioData : IDisposable
    {
        public AudioFormat Format { get; }
        public long Samples { get; }
        public Stream DataStream { get; }

        private readonly Stream baseStream;

        public AudioData(AudioFormat format, Stream dataStream, long samples, Stream baseStream)
        {
            Format = format;
            DataStream = dataStream;
            Samples = samples;
            this.baseStream = baseStream;
        }

        #region IDisposable Support
        private bool disposedValue = false;

        private void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    DataStream.Dispose();
                    baseStream.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}