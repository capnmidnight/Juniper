using System;
using System.IO;

using NAudio.Vorbis;
using NAudio.Wave;

using NLayer.NAudioSupport;

namespace Juniper.Audio
{
    /// <summary>
    /// Decoders for a few types of audio files.
    /// </summary>
    public static class Decoder
    {
        /// <summary>
        /// Run the decoder and return the audio data with information.
        /// </summary>
        /// <param name="reader"></param>
        private static RawAudio Decode(WaveStream reader)
        {
            var format = reader.WaveFormat;
            var bytesPerSample = format.Channels * format.BitsPerSample / 8;
            return new RawAudio
            {
                stream = reader,
                samples = reader.Length / bytesPerSample,
                channels = format.Channels,
                frequency = format.SampleRate
            };
        }

        /// <summary>
        /// Decodes MP3 files into a raw stream of PCM bytes.
        /// </summary>
        /// <param name="audioStream"></param>
        /// <returns></returns>
        public static RawAudio DecodeMP3(Stream audioStream)
        {
            return Decode(new Mp3FileReader(audioStream, wf => new Mp3FrameDecompressor(wf)));
        }

        /// <summary>
        /// Decodes WAV files into a raw stream of PCM bytes.
        /// </summary>
        /// <param name="audioStream"></param>
        /// <returns></returns>
        public static RawAudio DecodeWAV(Stream audioStream)
        {
            return Decode(new WaveFileReader(audioStream));
        }

        /// <summary>
        /// Decodes OGG files into a raw stream of PCM bytes.
        /// </summary>
        /// <param name="audioStream"></param>
        /// <returns></returns>
        public static RawAudio DecodeVorbis(Stream audioStream)
        {
            return Decode(new VorbisWaveReader(audioStream));
        }

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