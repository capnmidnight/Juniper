using Juniper.IO;

namespace Juniper.Sound;

public interface IAudioDecoder : IDeserializer<AudioData, MediaType.Audio>
{
    bool SupportsFormat(AudioFormat? format);

    Stream ToWave(Stream stream);

    AudioFormat? Format { get; set; }
}