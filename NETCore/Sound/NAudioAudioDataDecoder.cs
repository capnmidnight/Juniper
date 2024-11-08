using NAudio.Vorbis;
using NAudio.Wave;

using NLayer.NAudioSupport;

using System.Globalization;

namespace Juniper.Sound;

public class NAudioAudioDataDecoder : IAudioDecoder
{
    public static readonly MediaType.Audio[] SupportedFormats =
    [
        MediaType.Audio_Wave,
        MediaType.Audio_Mpeg,
        MediaType.Audio_OggVorbis,
        MediaType.Audio_Raw
    ];

    public NAudioAudioDataDecoder()
    { }

    public bool SupportsFormat(AudioFormat? format)
    {
        return format is not null
            && Array.IndexOf(SupportedFormats, format.ContentType) > -1;
    }

    private AudioFormat? format;

    public AudioFormat? Format
    {
        get { return format; }

        set
        {
            if (!SupportsFormat(value))
            {
                throw new NotSupportedException($"Don't know how to decode audio format {value?.ContentType}");
            }

            format = value;
        }
    }

    public MediaType.Audio? InputContentType
    {
        get
        {
            if (Format is null)
            {
                return null;
            }
            else
            {
                return Format.ContentType;
            }
        }
    }

#pragma warning disable CA1822 // Mark members as static
    public MediaType.Audio OutputContentType => MediaType.Audio_PCMA;
#pragma warning restore CA1822 // Mark members as static

    public WaveStream MakeDecodingStream(Stream stream)
    {
        if (stream is null)
        {
            throw new ArgumentNullException(nameof(stream));
        }

        if (Format is null)
        {
            throw new ArgumentNullException(nameof(Format));
        }

        if (!stream.CanSeek)
        {
            var mem = new MemoryStream();
            stream.CopyTo(mem);
            stream = mem;
        }

        if (Format.ContentType == MediaType.Audio_Wave)
        {
            return new WaveFileReader(stream);
        }
        else if (Format.ContentType == MediaType.Audio_Mpeg)
        {
            return new ManagedMpegStream(stream);
        }
        else if (Format.ContentType == MediaType.Audio_OggVorbis)
        {
            return new VorbisWaveReader(stream, true);
        }
        else if (Format.ContentType == MediaType.Audio_Raw)
        {
            var format = new WaveFormat(Format.SampleRate, Format.BitsPerSample, Format.Channels);
            return new RawSourceWaveStream(stream, format);
        }
        else
        {
            throw new NotSupportedException($"Don't know how to decode audio format {Format.ContentType}");
        }
    }

    public AudioData Deserialize(Stream stream)
    {
        var waveStream = MakeDecodingStream(stream);
        var format = waveStream.WaveFormat;
        var sampleRate = format.SampleRate;
        var bitsPerSample = format.BitsPerSample;
        var bytesPerSample = (int)Units.Bits.Bytes(bitsPerSample);
        var channels = format.Channels;
        var samples = waveStream.Length / bytesPerSample;

        ValidateFormat(sampleRate, bitsPerSample, channels);

        var audioFormat = MakeAudioFormat(sampleRate, bitsPerSample, channels);
        var pcmStream = new PcmBytesToFloatsStream(waveStream, bytesPerSample);
        return new AudioData(audioFormat, pcmStream, samples, stream);
    }

    public Stream ToWave(Stream stream)
    {
        var mem = new MemoryStream();
        var waveStream = MakeDecodingStream(stream);
        WaveFileWriter.WriteWavFileToStream(mem, waveStream);
        mem.Flush();
        mem.Position = 0;
        return mem;
    }

    private void ValidateFormat(int sampleRate, int bitsPerSample, int channels)
    {
        if (Format is null)
        {
            throw new InvalidOperationException($"Audio format is not specified.");
        }

        if (sampleRate != Format.SampleRate)
        {
            throw new InvalidOperationException($"Sample Rate does not match between audio format and audio file. Expected: {Format.SampleRate.ToString(CultureInfo.CurrentCulture)}. Actual: {sampleRate.ToString(CultureInfo.CurrentCulture)}");
        }

        if (bitsPerSample != Format.BitsPerSample)
        {
            throw new InvalidOperationException($"Sample Size does not match between audio format and audio file. Expected: {Format.BitsPerSample.ToString(CultureInfo.CurrentCulture)}. Actual: {bitsPerSample.ToString(CultureInfo.CurrentCulture)}");
        }

        if (channels != Format.Channels)
        {
            throw new InvalidOperationException($"Channel Count does not match between audio format and audio file. Expected: {Format.Channels.ToString(CultureInfo.CurrentCulture)}. Actual: {channels.ToString(CultureInfo.CurrentCulture)}");
        }
    }

    private static AudioFormat MakeAudioFormat(int sampleRate, int bitsPerSample, int channels)
    {
        var channelString = channels == 1 ? "mono" : "stereo";
        var sampsStr = (sampleRate / 1000).ToString(CultureInfo.CurrentCulture);
        var formatName = $"float-{sampsStr}khz-{bitsPerSample.ToString(CultureInfo.CurrentCulture)}bit-{channelString}-pcm";
        var audioFormat = new AudioFormat(
            formatName,
            MediaType.Audio_PCMA,
            sampleRate,
            bitsPerSample,
            channels);
        return audioFormat;
    }
}
