using System;
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
        /// <summary>
        /// Run the decoder and return the audio data with information.
        /// </summary>
        /// <param name="reader"></param>
        protected static AudioData Decode(WaveStream reader)
        {
            var format = reader.WaveFormat;
            var bytesPerSample = format.Channels * format.BitsPerSample / 8;
            return new AudioData
            {
                stream = reader,
                samples = reader.Length / bytesPerSample,
                channels = format.Channels,
                frequency = format.SampleRate
            };
        }

        public AudioData Deserialize(Stream stream)
        {
            return Decode(MakeDecodingStream(stream));
        }

        protected abstract WaveStream MakeDecodingStream(Stream stream);

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
    }
}