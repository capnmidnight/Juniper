using System.IO;

namespace Juniper.Audio
{
    /// <summary>
    /// The raw bytes and dimensions of an audio file that has been loaded either off disk or across the 'net.
    /// </summary>
    public class AudioData
    {
        public readonly AudioFormat format;
        public readonly long samples;
        public readonly Stream dataStream;

        public AudioData(AudioFormat format, Stream dataStream, long samples)
        {
            this.format = format;
            this.dataStream = dataStream;
            this.samples = samples;
        }
    }
}