using System;
using System.IO;
using System.Linq;

using Juniper.HTTP;
using Juniper.IO;
using Juniper.Progress;
using NAudio.Vorbis;
using NAudio.Wave;

namespace Juniper.Audio.NAudio
{
    public class NAudioAudioDataDecoder : IAudioDecoder
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

        public MediaType.Audio ContentType { get; }

        private WaveStream MakeDecodingStream(Stream stream)
        {
            if (ContentType == MediaType.Audio.X_Wav)
            {
                return new WaveFileReader(stream);
            }
            else if (ContentType == MediaType.Audio.Mpeg)
            {
                return new Mp3FileReader(new ErsatzSeekableStream(stream), Mp3FileReader.CreateAcmFrameDecompressor);
            }
            else if (ContentType == MediaType.Audio.Vorbis)
            {
                return new VorbisWaveReader(stream);
            }
            else
            {
                throw new NotSupportedException($"Don't know how to decode audio format {ContentType}");
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
                    (MediaType.Audio)ContentType,
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
