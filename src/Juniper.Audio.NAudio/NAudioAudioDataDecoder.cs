using System;
using System.IO;
using System.Linq;
using Juniper.HTTP;
using Juniper.Serialization;

using NAudio.Vorbis;
using NAudio.Wave;

using NLayer.NAudioSupport;

namespace Juniper.Audio.NAudio
{
    public class NAudioAudioDataDecoder : IDeserializer<AudioData>
    {
        public static readonly MediaType.Audio[] SupportedFormats =
        {
            MediaType.Audio.X_Wav,
            MediaType.Audio.Mpeg,
            MediaType.Audio.Vorbis
        };

        public NAudioAudioDataDecoder(MediaType.Audio format)
        {
            ContentType = format;
            if (!SupportedFormats.Contains(ContentType))
            {
                throw new NotSupportedException($"Don't know how to decode audio format {ContentType}");
            }
        }

        public MediaType ContentType { get; private set; }

        private WaveStream MakeDecodingStream(Stream stream)
        {
            if (ContentType == MediaType.Audio.X_Wav)
            {
                return new WaveFileReader(stream);
            }
            else if(ContentType == MediaType.Audio.Mpeg)
            {
                return new Mp3FileReader(stream, wf => new Mp3FrameDecompressor(wf));
            }
            else if(ContentType == MediaType.Audio.Vorbis)
            {
                return new VorbisWaveReader(stream);
            }
            else
            {
                throw new NotSupportedException($"Don't know how to decode audio format {ContentType}");
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
                return new AudioData((MediaType.Audio)ContentType, samples, channels, frequency, waveStream);
            }
        }
    }
}