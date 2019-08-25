using System;
using System.IO;

namespace Juniper.Audio
{
    /// <summary>
    /// The raw bytes and dimensions of an audio file that has been loaded either off disk or across the 'net.
    /// </summary>
    public struct AudioData : IDisposable
    {
        /// <summary>
        /// Reads a stream and fills a PCM buffer with data.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="data"></param>
        public static void FillBuffer(Stream stream, float[] data)
        {
            var buf = new byte[sizeof(float) / sizeof(byte)];
            for (var i = 0; i < data.Length; ++i)
            {
                stream.Read(buf, 0, buf.Length);
                data[i] = BitConverter.ToSingle(buf, 0);
            }
        }

        public static string GetContentType(AudioFormat format)
        {
            switch (format)
            {
                case AudioFormat.WAV: return "audio/vnd.wav";
                case AudioFormat.MP3: return "audio/mpeg";
                case AudioFormat.Vorbis: return "audio/ogg";
                default: return "application/unknown";
            }
        }

        public static string GetExtension(AudioFormat format)
        {
            switch (format)
            {
                case AudioFormat.WAV: return "wav";
                case AudioFormat.MP3: return "mp3";
                case AudioFormat.Vorbis: return "ogg";
                default: return "raw";
            }
        }

        public readonly AudioFormat format;
        public readonly string contentType;
        public readonly string extension;
        public readonly Stream stream;
        public readonly long samples;
        public readonly int channels;
        public readonly int frequency;

        public AudioData(AudioFormat format, long samples, int channels, int frequency, Stream stream)
        {
            contentType = GetContentType(format);
            extension = GetExtension(format);
            this.format = format;
            this.samples = samples;
            this.channels = channels;
            this.frequency = frequency;
            this.stream = stream;
        }

        public void Dispose()
        {
            stream.Dispose();
        }

        public override bool Equals(object obj)
        {
            return obj != null
                && obj is AudioData aud
                && aud.stream.Equals(stream)
                && aud.samples.Equals(samples)
                && aud.channels.Equals(channels)
                && aud.frequency.Equals(frequency);
        }

        public override int GetHashCode()
        {
            return stream?.GetHashCode() ?? 0
                ^ samples.GetHashCode()
                ^ channels.GetHashCode()
                ^ frequency.GetHashCode();
        }

        public static bool operator ==(AudioData left, AudioData right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(AudioData left, AudioData right)
        {
            return !(left == right);
        }
    }
}