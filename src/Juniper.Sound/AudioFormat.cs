using System;

namespace Juniper.Sound
{
    public sealed class AudioFormat : IEquatable<AudioFormat>
    {
        public static readonly AudioFormat Raw24KHz16BitMonoPCM = new("raw-24khz-16bit-mono-pcm", MediaType.Audio.PCMA, 24000, Units.Bits.PER_SHORT, 1);
        public static readonly AudioFormat Raw16KHz16BitMonoPCM = new("raw-16khz-16bit-mono-pcm", MediaType.Audio.PCMA, 16000, Units.Bits.PER_SHORT, 1);
        public static readonly AudioFormat Raw8KHz8BitMonoMULAW = new("raw-8khz-8bit-mono-mulaw", MediaType.Audio.PCMA, 8000, Units.Bits.PER_BYTE, 1);

        public static readonly AudioFormat Riff24KHz16BitMonoPCM = new("riff-24khz-16bit-mono-pcm", MediaType.Audio.X_Wav, 24000, Units.Bits.PER_SHORT, 1);
        public static readonly AudioFormat Riff16KHz16BitMonoPCM = new("riff-16khz-16bit-mono-pcm", MediaType.Audio.X_Wav, 16000, Units.Bits.PER_SHORT, 1);
        public static readonly AudioFormat Riff8KHz8BitMonoALAW = new("riff-8khz-8bit-mono-alaw", MediaType.Audio.X_Wav, 8000, Units.Bits.PER_BYTE, 1);
        public static readonly AudioFormat Riff8KHz8BitMonoMULAW = new("riff-8khz-8bit-mono-mulaw", MediaType.Audio.X_Wav, 8000, Units.Bits.PER_BYTE, 1);

        public static readonly AudioFormat Audio16KHz128KbitrateMonoMP3 = new("audio-16khz-128kbitrate-mono-mp3", MediaType.Audio.Mpeg, 16000, Units.Bits.PER_SHORT, 1);
        public static readonly AudioFormat Audio16KHz64KbitrateMonoMP3 = new("audio-16khz-64kbitrate-mono-mp3", MediaType.Audio.Mpeg, 16000, Units.Bits.PER_SHORT, 1);
        public static readonly AudioFormat Audio16KHz32KbitrateMonoMP3 = new("audio-16khz-32kbitrate-mono-mp3", MediaType.Audio.Mpeg, 16000, Units.Bits.PER_SHORT, 1);

        public static readonly AudioFormat Audio24KHz160KbitrateMonoMP3 = new("audio-24khz-160kbitrate-mono-mp3", MediaType.Audio.Mpeg, 24000, Units.Bits.PER_SHORT, 1);
        public static readonly AudioFormat Audio24KHz94KbitrateMonoMP3 = new("audio-24khz-96kbitrate-mono-mp3", MediaType.Audio.Mpeg, 24000, Units.Bits.PER_SHORT, 1);
        public static readonly AudioFormat Audio24KHz48KbitrateMonoMP3 = new("audio-24khz-48kbitrate-mono-mp3", MediaType.Audio.Mpeg, 24000, Units.Bits.PER_SHORT, 1);

        public string Name { get; }

        public MediaType.Audio ContentType { get; }

        public int SampleRate { get; }

        public int BitsPerSample { get; }

        public int Channels { get; }

        public AudioFormat(string formatName, MediaType.Audio audioFormat, int sampleRate, int bitsPerSample, int channels)
        {
            Name = formatName;
            ContentType = audioFormat;
            SampleRate = sampleRate;
            BitsPerSample = bitsPerSample;
            Channels = channels;
        }

        public bool Equals(AudioFormat other)
        {
            return other is not null
                && other.ContentType == ContentType
                && other.SampleRate == SampleRate
                && other.BitsPerSample == BitsPerSample
                && other.Channels == Channels;
        }

        public override bool Equals(object obj)
        {
            return obj is AudioFormat format
                && Equals(format);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name, ContentType, SampleRate, BitsPerSample, Channels);
        }

        public static bool operator ==(AudioFormat left, AudioFormat right)
        {
            return (left is null && right is null)
                || (left is not null && left.Equals(right));
        }

        public static bool operator !=(AudioFormat left, AudioFormat right)
        {
            return !(left == right);
        }
    }
}
