using System;
using System.IO;

namespace Juniper.Audio
{
    /// <summary>
    /// The raw bytes and dimensions of an audio file that has been loaded either off disk or across the 'net.
    /// </summary>
    public struct RawAudio : IDisposable
    {
        public Stream stream;
        public long samples;
        public int channels;
        public int frequency;

        public override bool Equals(object obj)
        {
            return obj != null
                && obj is RawAudio aud
                && aud.stream.Equals(stream)
                && aud.samples.Equals(samples)
                && aud.channels.Equals(channels)
                && aud.frequency.Equals(frequency);
        }

        public override int GetHashCode()
        {
            return stream?.GetHashCode() ?? 0
                ^ samples.GetHashCode()
                ^ channels.GetHashCode()
                ^ frequency.GetHashCode();
        }

        public static bool operator ==(RawAudio left, RawAudio right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(RawAudio left, RawAudio right)
        {
            return !(left == right);
        }

        public void Dispose()
        {
            ((IDisposable)stream).Dispose();
        }
    }
}
