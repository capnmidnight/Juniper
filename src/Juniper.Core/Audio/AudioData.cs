namespace Juniper.Audio
{
    /// <summary>
    /// The raw bytes and dimensions of an audio file that has been loaded either off disk or across the 'net.
    /// </summary>
    public class AudioData
    {
        public readonly HTTP.MediaType.Audio contentType;
        public readonly int samplesPerChannel;
        public readonly int numChannels;
        public readonly int frequency;
        public readonly float[] data;

        public AudioData(HTTP.MediaType.Audio contentType, int samplesPerChannel, int numChannels, int frequency, float[] data)
        {
            this.contentType = contentType;
            this.samplesPerChannel = samplesPerChannel;
            this.numChannels = numChannels;
            this.frequency = frequency;
            this.data = data;
        }
    }
}