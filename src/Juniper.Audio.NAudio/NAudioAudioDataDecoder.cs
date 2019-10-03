using System;
using System.IO;
using System.Threading.Tasks;
using Juniper.HTTP;
using Juniper.IO;
using Juniper.Progress;

using NAudio.Vorbis;
using NAudio.Wave;

using NLayer.NAudioSupport;

namespace Juniper.Audio.NAudio
{
    public class NAudioAudioDataDecoder : IAudioDecoder
    {
        public static float[] BytesToFloat(int bitsPerSample, byte[] bytes)
        {
            var scalar = (float)Math.Pow(2, 1 - bitsPerSample);
            var bytesPerSample = bitsPerSample / 8;
            var numSamples = bytes.Length / bytesPerSample;
            var floats = new float[numSamples];
            for (var s = 0; s < floats.Length; ++s)
            {
                short accum = 0;
                for (var b = bytesPerSample - 1; b >= 0; --b)
                {
                    accum <<= 8;
                    var c = bytes[s * bytesPerSample + b];
                    accum |= c;
                }

                var v = accum * scalar;
                if(bytesPerSample == 1)
                {
                    v = 0.5f * (v - 1);
                }
                floats[s] = v;
            }

            return floats;
        }

        public static byte[] FloatsToBytes(int bitsPerSample, float[] floats)
        {
            var scalar = (float)Math.Pow(2, bitsPerSample - 1);
            var bytesPerSample = bitsPerSample / 8;
            var numSamples = floats.Length * bytesPerSample;
            var bytes = new byte[numSamples];
            for (var s = 0; s < floats.Length; ++s)
            {
                var v = floats[s] * scalar;
                if(bytesPerSample == 1)
                {
                    v = 2 * v + 1;
                }
                short accum = (short)v;
                for (var b = 0; b < bytesPerSample; ++b)
                {
                    var c = (byte)(accum & 0xff);
                    bytes[s * bytesPerSample + b] = c;
                    accum >>= 8;
                }
            }

            return bytes;
        }

        public static readonly MediaType.Audio[] SupportedFormats =
        {
            MediaType.Audio.X_Wav,
            MediaType.Audio.Mpeg,
            MediaType.Audio.Vorbis,
            MediaType.Audio.PCMA,
        };

        public NAudioAudioDataDecoder()
        { }

        public bool SupportsFormat(AudioFormat format)
        {
            return Array.IndexOf(SupportedFormats, format.ContentType) > -1;
        }

        private AudioFormat format;

        public AudioFormat Format
        {
            get
            {
                return format;
            }
            set
            {
                if (!SupportsFormat(value))
                {
                    throw new NotSupportedException($"Don't know how to decode audio format {value.ContentType}");
                }

                format = value;
            }
        }

        public WaveStream MakeDecodingStream(Stream stream)
        {
            if (stream != null && !stream.CanSeek)
            {
                stream = new ErsatzSeekableStream(stream);
            }

            if (Format.ContentType == MediaType.Audio.X_Wav)
            {
                return new WaveFileReader(stream);
            }
            else if (Format.ContentType == MediaType.Audio.Mpeg)
            {
                return new Mp3FileReader(stream, CreateMp3FrameDecompressor);
            }
            else if (Format.ContentType == MediaType.Audio.Vorbis)
            {
                return new VorbisWaveReader(stream);
            }
            else if (Format.ContentType == MediaType.Audio.PCMA)
            {
                return new RawSourceWaveStream(stream, new WaveFormat(Format.sampleRate, Format.bitsPerSample, Format.channels));
            }
            else
            {
                throw new NotSupportedException($"Don't know how to decode audio format {Format.ContentType}");
            }
        }

        private IMp3FrameDecompressor CreateMp3FrameDecompressor(WaveFormat format)
        {
            //if (Environment.OSVersion.Platform == PlatformID.Unix
            //    || Environment.OSVersion.Platform == PlatformID.MacOSX)
            //{
                // This works on Android, but only seems to work for some Mp3 files.
                return new Mp3FrameDecompressor(format);
            //}
            //else
            //{
            //    // This seems to work for all Mp3 files, but only works on Windows.
            //    return Mp3FileReader.CreateAcmFrameDecompressor(format);
            //}
        }

        public AudioData Deserialize(Stream stream, IProgress prog)
        {
            prog.Report(0);
            AudioData audioData = null;
            if (stream != null)
            {
                using (stream)
                using (var waveStream = MakeDecodingStream(stream))
                {
                    var format = waveStream.WaveFormat;

                    if (format.SampleRate != Format.sampleRate)
                    {
                        throw new InvalidOperationException($"Sample Rate does not match between audio format and audio file. Expected: {Format.sampleRate}. Actual: {format.SampleRate}");
                    }

                    if (format.BitsPerSample != Format.bitsPerSample)
                    {
                        throw new InvalidOperationException($"Sample Size does not match between audio format and audio file. Expected: {Format.bitsPerSample}. Actual: {format.BitsPerSample}");
                    }


                    if (format.Channels != Format.channels)
                    {
                        throw new InvalidOperationException($"Channel Count does not match between audio format and audio file. Expected: {Format.channels}. Actual: {format.Channels}");
                    }

                    var mem = new MemoryStream();
                    waveStream.CopyTo(mem);
                    mem.Flush();
                    var buf = mem.ToArray();

                    var data = BytesToFloat(format.BitsPerSample, buf);

                    var channelString = Format.channels == 1 ? "mono" : "stereo";
                    audioData = new AudioData(
                        new AudioFormat(
                            $"float-{Format.sampleRate / 1000}khz-{Format.bitsPerSample}bit-{channelString}-pcm",
                            MediaType.Audio.PCMA,
                            Format.sampleRate,
                            Format.bitsPerSample,
                            Format.channels),
                        data);
                }
            }
            prog.Report(1);
            return audioData;
        }

        public Task Play(AudioData audio)
        {
            var format = new WaveFormat(audio.format.sampleRate, audio.format.bitsPerSample, audio.format.channels);
            var data = FloatsToBytes(audio.format.bitsPerSample, audio.data);

            var waveStream = new RawSourceWaveStream(data, 0, data.Length, format);
            return Play(waveStream);
        }

        public async Task Play(WaveStream waveStream)
        {
            var waveOut = new WaveOut();
            waveOut.Init(waveStream);
            waveOut.Play();
            while (waveOut.PlaybackState == PlaybackState.Playing)
            {
                await Task.Yield();
            }
        }
    }
}
