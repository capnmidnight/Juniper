using System;
using System.Collections.Generic;

namespace Juniper.Sound
{
    public sealed class AudioFormat : IEquatable<AudioFormat>
    {
        public static readonly AudioFormat Raw24KHz16BitMonoPCM = new AudioFormat("raw-24khz-16bit-mono-pcm", MediaType.Audio.PCMA, 24000, Units.Bits.PER_SHORT, 1);
        public static readonly AudioFormat Raw16KHz16BitMonoPCM = new AudioFormat("raw-16khz-16bit-mono-pcm", MediaType.Audio.PCMA, 16000, Units.Bits.PER_SHORT, 1);
        public static readonly AudioFormat Raw8KHz8BitMonoMULAW = new AudioFormat("raw-8khz-8bit-mono-mulaw", MediaType.Audio.PCMA, 8000, Units.Bits.PER_BYTE, 1);

        public static readonly AudioFormat Riff24KHz16BitMonoPCM = new AudioFormat("riff-24khz-16bit-mono-pcm", MediaType.Audio.X_Wav, 24000, Units.Bits.PER_SHORT, 1);
        public static readonly AudioFormat Riff16KHz16BitMonoPCM = new AudioFormat("riff-16khz-16bit-mono-pcm", MediaType.Audio.X_Wav, 16000, Units.Bits.PER_SHORT, 1);
        public static readonly AudioFormat Riff8KHz8BitMonoALAW = new AudioFormat("riff-8khz-8bit-mono-alaw", MediaType.Audio.X_Wav, 8000, Units.Bits.PER_BYTE, 1);
        public static readonly AudioFormat Riff8KHz8BitMonoMULAW = new AudioFormat("riff-8khz-8bit-mono-mulaw", MediaType.Audio.X_Wav, 8000, Units.Bits.PER_BYTE, 1);

        public static readonly AudioFormat Audio16KHz128KbitrateMonoMP3 = new AudioFormat("audio-16khz-128kbitrate-mono-mp3", MediaType.Audio.Mpeg, 16000, Units.Bits.PER_SHORT, 1);
        public static readonly AudioFormat Audio16KHz64KbitrateMonoMP3 = new AudioFormat("audio-16khz-64kbitrate-mono-mp3", MediaType.Audio.Mpeg, 16000, Units.Bits.PER_SHORT, 1);
        public static readonly AudioFormat Audio16KHz32KbitrateMonoMP3 = new AudioFormat("audio-16khz-32kbitrate-mono-mp3", MediaType.Audio.Mpeg, 16000, Units.Bits.PER_SHORT, 1);

        public static readonly AudioFormat Audio24KHz160KbitrateMonoMP3 = new AudioFormat("audio-24khz-160kbitrate-mono-mp3", MediaType.Audio.Mpeg, 24000, Units.Bits.PER_SHORT, 1);
        public static readonly AudioFormat Audio24KHz94KbitrateMonoMP3 = new AudioFormat("audio-24khz-96kbitrate-mono-mp3", MediaType.Audio.Mpeg, 24000, Units.Bits.PER_SHORT, 1);
        public static readonly AudioFormat Audio24KHz48KbitrateMonoMP3 = new AudioFormat("audio-24khz-48kbitrate-mono-mp3", MediaType.Audio.Mpeg, 24000, Units.Bits.PER_SHORT, 1);

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
            return other is object
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
            var hashCode = -1416147052;
            hashCode = (hashCode * -1521134295) + EqualityComparer<string>.Default.GetHashCode(Name);
            hashCode = (hashCode * -1521134295) + EqualityComparer<MediaType.Audio>.Default.GetHashCode(ContentType);
            hashCode = (hashCode * -1521134295) + SampleRate.GetHashCode();
            hashCode = (hashCode * -1521134295) + BitsPerSample.GetHashCode();
            hashCode = (hashCode * -1521134295) + Channels.GetHashCode();
            return hashCode;
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
    }
}
