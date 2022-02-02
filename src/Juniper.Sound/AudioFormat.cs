using System;

namespace Juniper.Sound
{
    public sealed class AudioFormat : IEquatable<AudioFormat>
    {
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
