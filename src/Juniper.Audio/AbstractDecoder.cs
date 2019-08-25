using System.IO;

using Juniper.Serialization;

using NAudio.Wave;

namespace Juniper.Audio
{
    /// <summary>
    /// Decoders for a few types of audio files.
    /// </summary>
    public abstract class AbstractDecoder : IDeserializer<AudioData>
    {
        private readonly AudioFormat format;

        protected AbstractDecoder(AudioFormat format)
        {
            this.format = format;
        }

        /// <summary>
        /// Run the decoder and return the audio data with information.
        /// </summary>
        /// <param name="stream"></param>
        private AudioData Decode(WaveStream stream)
        {
            var format = stream.WaveFormat;
            var bytesPerSample = format.Channels * format.BitsPerSample / 8;

            var samples = stream.Length / bytesPerSample;
            var channels = format.Channels;
            var frequency = format.SampleRate;
            return new AudioData(this.format, samples, channels, frequency, stream);
        }

        public AudioData Deserialize(Stream stream)
        {
            return Decode(MakeDecodingStream(stream));
        }

        protected abstract WaveStream MakeDecodingStream(Stream stream);
    }
}