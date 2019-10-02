using Juniper.HTTP;
using Juniper.IO;

namespace Juniper.Audio
{
    public interface IAudioDecoder : IDeserializer<AudioData>
    {
        MediaType.Audio ContentType { get; }
    }
}