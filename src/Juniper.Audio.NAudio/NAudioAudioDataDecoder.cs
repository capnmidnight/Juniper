using System;
using System.IO;
using System.Linq;

using Juniper.HTTP;
using Juniper.Progress;
using Juniper.Serialization;
using Juniper.Streams;
using NAudio.Vorbis;
using NAudio.Wave;

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
            ReadContentType = format;
            if (!SupportedFormats.Contains(ReadContentType))
            {
                throw new NotSupportedException($"Don't know how to decode audio format {ReadContentType}");
            }
        }

        public MediaType ReadContentType { get; private set; }

        private WaveStream MakeDecodingStream(Stream stream)
        {
            if (ReadContentType == MediaType.Audio.X_Wav)
            {
                return new WaveFileReader(stream);
            }
            else if (ReadContentType == MediaType.Audio.Mpeg)
            {
                return new Mp3FileReader(new ErsatzSeekableStream(stream), Mp3FileReader.CreateAcmFrameDecompressor);
            }
            else if (ReadContentType == MediaType.Audio.Vorbis)
            {
                return new VorbisWaveReader(stream);
            }
            else
            {
                throw new NotSupportedException($"Don't know how to decode audio format {ReadContentType}");
            }
        }

        public AudioData Deserialize(Stream stream, IProgress prog)
        {
            prog.Report(0);
            using (stream)
            using (var waveStream = MakeDecodingStream(stream))
            {
                var format = waveStream.WaveFormat;
                var bytesPerSample = format.BitsPerSample / 8;
                var numChannels = format.Channels;
                var numSamples = waveStream.Length / bytesPerSample;
                var samplesPerChannel = numSamples / numChannels;
                var data = new float[numSamples];
                var scalar = (float)Math.Pow(2, 1 - format.BitsPerSample);
                var buf = new byte[bytesPerSample];
                for (var s = 0; s < numSamples; ++s)
                {
                    int read = waveStream.Read(buf, 0, bytesPerSample);
                    short accum = 0;
                    for (var b = read - 1; b >= 0; --b)
                    {
                        accum = (short)((accum << 8) | buf[b]);
                    }

                    data[s] = accum * scalar;
                }

                var aud = new AudioData(
                    (MediaType.Audio)ReadContentType,
                    (int)samplesPerChannel,
                    numChannels,
                    format.SampleRate,
                    data);
                prog.Report(1);
                return aud;
            }
        }
    }
}
