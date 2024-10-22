//using System;
//using System.Globalization;
//using System.IO;
//using System.Linq;

//using Xabe.FFmpeg;
//using Juniper.Processes;

//namespace Juniper.Sound
//{

//    public class FFMpegAudioDataDecoder : IAudioDecoder
//    {
//        private static void InitFFMpeg()
//        {
//            if (FFmpeg.ExecutablesPath is null)
//            {
//                //Set directory where app should look for FFmpeg executables.
//                var path = ShellCommand.FindCommandPath("ffmpeg");
//                if (path is null)
//                {
//                    path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "FFmpeg");
//                    var dir = new DirectoryInfo(path);
//                    if (dir.Exists)
//                    {
//                        path = dir.GetFiles()
//                            .Where(f => Path.GetFileNameWithoutExtension(f.Name).ToLower() == "ffmpeg")
//                            .Select(f => f.FullName)
//                            .FirstOrDefault();
//                    }
//                }

//                FFmpeg.SetExecutablesPath(Path.GetDirectoryName(path));
//                if (!File.Exists(path))
//                {
//                    throw new Exception("No FFMpeg");
//                }
//            }
//        }

//        public static readonly MediaType.Audio[] SupportedFormats =
//        {
//            MediaType.Audio_Wave,
//            MediaType.Audio_Mpeg,
//            MediaType.Audio_OggVorbis,
//            MediaType.Audio_Raw
//        };

//        public FFMpegAudioDataDecoder()
//        { }

//        public bool SupportsFormat(AudioFormat format)
//        {
//            return Array.IndexOf(SupportedFormats, format?.ContentType) > -1;
//        }

//        private AudioFormat format;

//        public AudioFormat Format
//        {
//            get { return format; }

//            set
//            {
//                if (!SupportsFormat(value))
//                {
//                    throw new NotSupportedException($"Don't know how to decode audio format {value?.ContentType}");
//                }

//                format = value;
//            }
//        }

//        public MediaType.Audio InputContentType
//        {
//            get
//            {
//                if (Format is null)
//                {
//                    return null;
//                }
//                else
//                {
//                    return Format.ContentType;
//                }
//            }
//        }

//#pragma warning disable CA1822 // Mark members as static
//        public MediaType.Audio OutputContentType => MediaType.Audio_PCMA;
//#pragma warning restore CA1822 // Mark members as static


//        public FFMpegProcessStream MakeDecodingStream(Stream stream)
//        {
//            if (stream is null)
//            {
//                throw new ArgumentNullException(nameof(stream));
//            }

//            //if (!stream.CanSeek)
//            //{
//            //    var mem = new MemoryStream();
//            //    stream.CopyTo(mem);
//            //    stream = mem;
//            //}

//            var proc = new FFMpegProcessStream();
//            if (!proc.Start(stream))
//            {
//                proc.Dispose();
//                throw new ProcessStartException("Couldn't start the FFmpeg process");
//            }
//            return proc;
//        }

//        public AudioData Deserialize(Stream stream)
//        {
//            var waveStream = MakeDecodingStream(stream);
//            var format = waveStream.WaveFormat;
//            var sampleRate = format.SampleRate;
//            var bitsPerSample = format.BitsPerSample;
//            var bytesPerSample = (int)Units.Bits.Bytes(bitsPerSample);
//            var channels = format.Channels;
//            var samples = waveStream.Length / bytesPerSample;

//            ValidateFormat(sampleRate, bitsPerSample, channels);

//            var audioFormat = MakeAudioFormat(sampleRate, bitsPerSample, channels);
//            var pcmStream = new PcmBytesToFloatsStream(waveStream, bytesPerSample);
//            return new AudioData(audioFormat, pcmStream, samples, stream);
//        }

//        public Stream ToWave(Stream stream)
//        {
//            var mem = new MemoryStream();
//            var waveStream = MakeDecodingStream(stream);
//            WaveFileWriter.WriteWavFileToStream(mem, waveStream);
//            mem.Flush();
//            mem.Position = 0;
//            return mem;
//        }

//        private void ValidateFormat(int sampleRate, int bitsPerSample, int channels)
//        {
//            if (sampleRate != Format.SampleRate)
//            {
//                throw new InvalidOperationException($"Sample Rate does not match between audio format and audio file. Expected: {Format.SampleRate.ToString(CultureInfo.CurrentCulture)}. Actual: {sampleRate.ToString(CultureInfo.CurrentCulture)}");
//            }

//            if (bitsPerSample != Format.BitsPerSample)
//            {
//                throw new InvalidOperationException($"Sample Size does not match between audio format and audio file. Expected: {Format.BitsPerSample.ToString(CultureInfo.CurrentCulture)}. Actual: {bitsPerSample.ToString(CultureInfo.CurrentCulture)}");
//            }

//            if (channels != Format.Channels)
//            {
//                throw new InvalidOperationException($"Channel Count does not match between audio format and audio file. Expected: {Format.Channels.ToString(CultureInfo.CurrentCulture)}. Actual: {channels.ToString(CultureInfo.CurrentCulture)}");
//            }
//        }

//        private static AudioFormat MakeAudioFormat(int sampleRate, int bitsPerSample, int channels)
//        {
//            var channelString = channels == 1 ? "mono" : "stereo";
//            var sampsStr = (sampleRate / 1000).ToString(CultureInfo.CurrentCulture);
//            var formatName = $"float-{sampsStr}khz-{bitsPerSample.ToString(CultureInfo.CurrentCulture)}bit-{channelString}-pcm";
//            var audioFormat = new AudioFormat(
//                formatName,
//                MediaType.Audio_PCMA,
//                sampleRate,
//                bitsPerSample,
//                channels);
//            return audioFormat;
//        }
//    }
//}
