using System;
using System.IO;
using System.Threading.Tasks;

using Juniper.IO;
using Juniper.Sound;
using Juniper.Speech.Azure.CognitiveServices;

using NAudio.Wave;

namespace Juniper
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            var text = "The quick brown fox jumps over the lazy dog.";
            if (args?.Length > 1)
            {
                text = args[1];
            }

            // credentials
            var userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            var keyFile = Path.Combine(userProfile, "Projects", "DevKeys", "azure-speech.txt");
            var lines = File.ReadAllLines(keyFile);
            var subscriptionKey = lines[0];
            var region = lines[1];
            var resourceName = lines[2];

            // caching
            var cacheDirName = Path.Combine(userProfile, "Projects");
            var cacheDir = new DirectoryInfo(cacheDirName);
            var cache = new CachingStrategy
            {
                new FileCacheLayer(cacheDir)
            };

            var voiceListDecoder = new JsonFactory<Voice[]>();
            var outputFormats = new[]
            {
                AudioFormat.Audio16KHz32KbitrateMonoMP3,
                AudioFormat.Audio16KHz64KbitrateMonoMP3,
                AudioFormat.Audio16KHz128KbitrateMonoMP3,
                AudioFormat.Audio24KHz48KbitrateMonoMP3,
                AudioFormat.Audio24KHz94KbitrateMonoMP3,
                AudioFormat.Audio24KHz160KbitrateMonoMP3
            };

            var audioDecoder = new NAudioAudioDataDecoder();

            foreach (var outputFormat in outputFormats)
            {
                System.Console.Write(outputFormat.Name);
                System.Console.Write(":> ");
                var ttsClient = new TextToSpeechClient(
                    region,
                    subscriptionKey,
                    resourceName,
                    voiceListDecoder,
                    outputFormat,
                    audioDecoder,
                    cache);

                var voices = await ttsClient
                    .GetVoicesAsync()
                    .ConfigureAwait(false);
                var voice = Array.Find(voices, v => v.Locale == "en-US" && v.Gender == "Female");

                try
                {
                    //await DecodeAudio(text, audioDecoder, ttsClient, voice);
                    await PlayAudioAsync(text, audioDecoder, ttsClient, voice).ConfigureAwait(false);
                    System.Console.WriteLine("Success!");
                }
                catch (Exception exp)
                {
                    System.Console.WriteLine(exp.Message);
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Code Quality", "IDE0051:Remove unused private members", Justification = "Keeping around as an example")]
        private static async Task DecodeAudioAsync(string text, TextToSpeechClient ttsClient, Voice voice)
        {
            var audio = await ttsClient
                .GetDecodedAudioAsync(text, voice.ShortName)
                .ConfigureAwait(false);
            await PlayAsync(audio)
                .ConfigureAwait(false);
        }

        private static async Task PlayAudioAsync(string text, NAudioAudioDataDecoder audioDecoder, TextToSpeechClient ttsClient, Voice voice)
        {
            using var audioStream = await ttsClient
                .GetAudioDataStreamAsync(text, voice.ShortName)
                .ConfigureAwait(false);
            using var waveStream = audioDecoder.MakeDecodingStream(audioStream);
            var sr = waveStream.WaveFormat.SampleRate;
            var bps = waveStream.WaveFormat.BitsPerSample;
            System.Console.Write($"{bps} * {sr} = {bps * sr}");
            //await Task.Yield();
            await PlayAsync(waveStream).ConfigureAwait(false);
        }

        public static Task PlayAsync(AudioData audio)
        {
            if (audio is null)
            {
                throw new ArgumentNullException(nameof(audio));
            }

            var format = new WaveFormat(audio.Format.SampleRate, audio.Format.BitsPerSample, audio.Format.Channels);
            using var sourceStream = new FloatsToPcmBytesStream(audio.DataStream, audio.Format.BitsPerSample / 8);
            using var waveStream = new RawSourceWaveStream(sourceStream, format);
            return PlayAsync(waveStream);
        }

        public static async Task PlayAsync(WaveStream waveStream)
        {
            using var waveOut = new WaveOut();
            waveOut.Init(waveStream);
            waveOut.Play();
            while (waveOut.PlaybackState == PlaybackState.Playing)
            {
                await Task.Yield();
            }
        }
    }
}
