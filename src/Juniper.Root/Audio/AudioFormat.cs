using System;

namespace Juniper.Audio
{
    public sealed class AudioFormat : IEquatable<AudioFormat>
    {
        public static AudioFormat Raw24KHz16BitMonoPCM = new AudioFormat("raw-24khz-16bit-mono-pcm", MediaType.Audio.PCMA, 24000, Units.Bits.PER_SHORT, 1);
        public static AudioFormat Raw16KHz16BitMonoPCM = new AudioFormat("raw-16khz-16bit-mono-pcm", MediaType.Audio.PCMA, 16000, Units.Bits.PER_SHORT, 1);
        public static AudioFormat Raw8KHz8BitMonoMULAW = new AudioFormat("raw-8khz-8bit-mono-mulaw", MediaType.Audio.PCMA, 8000, Units.Bits.PER_BYTE, 1);

        public static AudioFormat Riff24KHz16BitMonoPCM = new AudioFormat("riff-24khz-16bit-mono-pcm", MediaType.Audio.X_Wav, 24000, Units.Bits.PER_SHORT, 1);
        public static AudioFormat Riff16KHz16BitMonoPCM = new AudioFormat("riff-16khz-16bit-mono-pcm", MediaType.Audio.X_Wav, 16000, Units.Bits.PER_SHORT, 1);
        public static AudioFormat Riff8KHz8BitMonoALAW = new AudioFormat("riff-8khz-8bit-mono-alaw", MediaType.Audio.X_Wav, 8000, Units.Bits.PER_BYTE, 1);
        public static AudioFormat Riff8KHz8BitMonoMULAW = new AudioFormat("riff-8khz-8bit-mono-mulaw", MediaType.Audio.X_Wav, 8000, Units.Bits.PER_BYTE, 1);

        public static AudioFormat Audio16KHz128KbitrateMonoMP3 = new AudioFormat("audio-16khz-128kbitrate-mono-mp3", MediaType.Audio.Mpeg, 16000, Units.Bits.PER_SHORT, 1);
        public static AudioFormat Audio16KHz64KbitrateMonoMP3 = new AudioFormat("audio-16khz-64kbitrate-mono-mp3", MediaType.Audio.Mpeg, 16000, Units.Bits.PER_SHORT, 1);
        public static AudioFormat Audio16KHz32KbitrateMonoMP3 = new AudioFormat("audio-16khz-32kbitrate-mono-mp3", MediaType.Audio.Mpeg, 16000, Units.Bits.PER_SHORT, 1);

        public static AudioFormat Audio24KHz160KbitrateMonoMP3 = new AudioFormat("audio-24khz-160kbitrate-mono-mp3", MediaType.Audio.Mpeg, 24000, Units.Bits.PER_SHORT, 1);
        public static AudioFormat Audio24KHz94KbitrateMonoMP3 = new AudioFormat("audio-24khz-96kbitrate-mono-mp3", MediaType.Audio.Mpeg, 24000, Units.Bits.PER_SHORT, 1);
        public static AudioFormat Audio24KHz48KbitrateMonoMP3 = new AudioFormat("audio-24khz-48kbitrate-mono-mp3", MediaType.Audio.Mpeg, 24000, Units.Bits.PER_SHORT, 1);

        public readonly string Name;
        public readonly MediaType.Audio ContentType;
        public readonly int sampleRate;
        public readonly int bitsPerSample;
        public readonly int channels;

        public AudioFormat(string formatName, MediaType.Audio audioFormat, int sampleRate, int bitsPerSample, int channels)
        {
            Name = formatName;
            ContentType = audioFormat;
            this.sampleRate = sampleRate;
            this.bitsPerSample = bitsPerSample;
            this.channels = channels;
        }

        public bool Equals(AudioFormat other)
        {
            return other is object
                && other.ContentType == ContentType
                && other.sampleRate == sampleRate
                && other.bitsPerSample == bitsPerSample
                && other.channels == channels;
        }

        public override bool Equals(object obj)
        {
            return obj is AudioFormat format
                && Equals(format);
        }

        public static bool operator ==(AudioFormat left, AudioFormat right)
        {
            return (left is null && right is null)
                || (left is object && left.Equals(right));
        }

        public static bool operator !=(AudioFormat left, AudioFormat right)
        {
            return !(left == right);
        }

        public override int GetHashCode()
        {
            return ContentType.GetHashCode()
                ^ sampleRate.GetHashCode()
                ^ bitsPerSample.GetHashCode()
                ^ channels.GetHashCode();
        }
    }
}
