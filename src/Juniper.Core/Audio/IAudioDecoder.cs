using Juniper.IO;

namespace Juniper.Audio
{
    public interface IAudioDecoder : IDeserializer<AudioData>
    {
        bool SupportsFormat(AudioFormat format);

        AudioFormat Format { get; set; }
    }
}