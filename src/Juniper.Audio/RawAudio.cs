using System.IO;

namespace Juniper.Audio
{
    /// <summary>
    /// The raw bytes and dimensions of an audio file that has been loaded either off disk or across the 'net.
    /// </summary>
    public struct RawAudio
    {
        public Stream stream;
        public long samples;
        public int channels, frequency;
    }
}
