using Juniper.HTTP;

namespace Juniper.Audio
{
    /// <summary>
    /// The raw bytes and dimensions of an audio file that has been loaded either off disk or across the 'net.
    /// </summary>
    public class AudioData
    {
        public readonly AudioFormat format;
        public readonly float[] data;

        public AudioData(AudioFormat format, float[] data)
        {
            this.format = format;
            this.data = data;
        }
    }
}