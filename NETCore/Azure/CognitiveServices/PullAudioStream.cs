using Microsoft.CognitiveServices.Speech.Audio;

namespace Juniper.Speech.Azure.CognitiveServices;

public class PullAudioStream : PullAudioInputStreamCallback
{
    private readonly Stream stream;

    public PullAudioStream(Stream stream)
    {
        this.stream = stream;
    }

    public override int Read(byte[] dataBuffer, uint size)
    {
        return stream.Read(dataBuffer, 0, (int)size);
    }
}
