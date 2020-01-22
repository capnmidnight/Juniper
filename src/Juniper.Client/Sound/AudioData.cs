using System.IO;

namespace Juniper.Sound
{
    /// <summary>
    /// The raw bytes and dimensions of an audio file that has been loaded either off disk or across the 'net.
    /// </summary>
    public class AudioData
    {
        public AudioFormat Format { get; }
        public long Samples { get; }
        public Stream DataStream { get; }

        public AudioData(AudioFormat format, Stream dataStream, long samples)
        {
            Format = format;
            DataStream = dataStream;
            Samples = samples;
        }
    }
}