using System;
using System.Globalization;
using System.IO;

using Juniper.IO;
using Juniper.Progress;

using NAudio.Wave;

namespace Juniper.Sound
{
    public class NAudioAudioDataDecoder : IAudioDecoder
    {
        public static readonly MediaType.Audio[] SupportedFormats =
        {
            MediaType.Audio.X_Wav,
            MediaType.Audio.Mpeg,
            MediaType.Audio.PCMA
        };

        public NAudioAudioDataDecoder()
        { }

        public bool SupportsFormat(AudioFormat format)
        {
            return Array.IndexOf(SupportedFormats, format?.ContentType) > -1;
        }

        private AudioFormat format;

        public AudioFormat Format
        {
            get { return format; }

            set
            {
                if (!SupportsFormat(value))
                {
                    throw new NotSupportedException($"Don't know how to decode audio format {value?.ContentType}");
                }

                format = value;
            }
        }

        public MediaType.Audio ContentType
        {
            get
            {
                if (Format is null)
                {
                    return null;
                }
                else
                {
                    return Format.ContentType;
                }
            }
        }

        public WaveStream MakeDecodingStream(Stream stream)
        {
            if (Format.ContentType == MediaType.Audio.X_Wav)
            {
                return new WaveFileReader(stream);
            }
            else if (Format.ContentType == MediaType.Audio.Mpeg)
            {
                return new Mp3FileReader(stream, CreateMp3FrameDecompressor);
            }
            else if (Format.ContentType == MediaType.Audio.PCMA)
            {
                var format = new WaveFormat(Format.SampleRate, Format.BitsPerSample, Format.Channels);
                return new RawSourceWaveStream(stream, format);
            }
            else
            {
                throw new NotSupportedException($"Don't know how to decode audio format {Format.ContentType}");
            }
        }

        private IMp3FrameDecompressor CreateMp3FrameDecompressor(WaveFormat format)
        {
            if (Environment.OSVersion.Platform == PlatformID.Unix
                || Environment.OSVersion.Platform == PlatformID.MacOSX)
            {
                // This works on Android, but only seems to work for some Mp3 files.
                return new NLayer.NAudioSupport.Mp3FrameDecompressor(format);
            }
            else
            {
                // This seems to work for all Mp3 files, but only works on Windows.
                return Mp3FileReader.CreateAcmFrameDecompressor(format);
            }
        }

        public AudioData Deserialize(Stream stream)
        {
            if (stream is null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            if (!stream.CanSeek)
            {
                var mem = new MemoryStream();
                stream.CopyTo(mem);
                stream = mem;
            }

            var waveStream = MakeDecodingStream(stream);
            var format = waveStream.WaveFormat;
            var sampleRate = format.SampleRate;
            var bitsPerSample = format.BitsPerSample;
            var bytesPerSample = (int)Units.Bits.Bytes(bitsPerSample);
            var channels = format.Channels;
            var samples = waveStream.Length / bytesPerSample;

            ValidateFormat(sampleRate, bitsPerSample, channels);

            var audioFormat = MakeAudioFormat(sampleRate, bitsPerSample, channels);
            var pcmStream = new PcmBytesToFloatsStream(waveStream, bytesPerSample);
            return new AudioData(audioFormat, pcmStream, samples);
        }

        private void ValidateFormat(int sampleRate, int bitsPerSample, int channels)
        {
            if (sampleRate != Format.SampleRate)
            {
                throw new InvalidOperationException($"Sample Rate does not match between audio format and audio file. Expected: {Format.SampleRate.ToString(CultureInfo.CurrentCulture)}. Actual: {sampleRate.ToString(CultureInfo.CurrentCulture)}");
            }

            if (bitsPerSample != Format.BitsPerSample)
            {
                throw new InvalidOperationException($"Sample Size does not match between audio format and audio file. Expected: {Format.BitsPerSample.ToString(CultureInfo.CurrentCulture)}. Actual: {bitsPerSample.ToString(CultureInfo.CurrentCulture)}");
            }

            if (channels != Format.Channels)
            {
                throw new InvalidOperationException($"Channel Count does not match between audio format and audio file. Expected: {Format.Channels.ToString(CultureInfo.CurrentCulture)}. Actual: {channels.ToString(CultureInfo.CurrentCulture)}");
            }
        }

        private static AudioFormat MakeAudioFormat(int sampleRate, int bitsPerSample, int channels)
        {
            var channelString = channels == 1 ? "mono" : "stereo";
            var sampsStr = (sampleRate / 1000).ToString(CultureInfo.CurrentCulture);
            var formatName = $"float-{sampsStr}khz-{bitsPerSample.ToString(CultureInfo.CurrentCulture)}bit-{channelString}-pcm";
            var audioFormat = new AudioFormat(
                formatName,
                MediaType.Audio.PCMA,
                sampleRate,
                bitsPerSample,
                channels);
            return audioFormat;
        }
    }
}
