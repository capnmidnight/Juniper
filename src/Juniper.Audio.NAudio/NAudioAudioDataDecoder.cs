using System;
using System.IO;

using Juniper.Serialization;

using NAudio.Vorbis;
using NAudio.Wave;

using NLayer.NAudioSupport;

namespace Juniper.Audio.NAudio
{
    public class NAudioAudioDataDecoder : IDeserializer<AudioData>
    {
        public NAudioAudioDataDecoder(HTTP.MediaType.Audio format)
        {
            Format = format;
            if (Format != HTTP.MediaType.Audio.X_Wav
                && Format != HTTP.MediaType.Audio.Mpeg
                && Format != HTTP.MediaType.Audio.Vorbis)
            {
                throw new NotSupportedException($"Don't know how to decode audio format {Format}");
            }
        }

        public HTTP.MediaType.Audio Format { get; private set; }

        private WaveStream MakeDecodingStream(Stream stream)
        {
            if (Format == HTTP.MediaType.Audio.X_Wav)
            {
                return new WaveFileReader(stream);
            }
            else if(Format == HTTP.MediaType.Audio.Mpeg)
            {
                return new Mp3FileReader(stream, wf => new Mp3FrameDecompressor(wf));
            }
            else if(Format == HTTP.MediaType.Audio.Vorbis)
            {
                return new VorbisWaveReader(stream);
            }
            else
            {
                throw new NotSupportedException($"Don't know how to decode audio format {Format}");
            }
        }

        public AudioData Deserialize(Stream stream)
        {
            using (var waveStream = MakeDecodingStream(stream))
            {
                var format = waveStream.WaveFormat;
                var bytesPerSample = format.Channels * format.BitsPerSample / 8;

                var samples = waveStream.Length / bytesPerSample;
                var channels = format.Channels;
                var frequency = format.SampleRate;
                return new AudioData(Format, samples, channels, frequency, waveStream);
            }
        }
    }
}