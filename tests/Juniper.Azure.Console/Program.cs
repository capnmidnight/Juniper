using System;
using System.IO;
using System.Threading.Tasks;

using Juniper.Audio;
using Juniper.Azure.CognitiveServices;
using Juniper.IO;

using NAudio.Wave;

namespace Juniper.Azure
{
    internal static class Program
    {
        private static async Task Main(string[] args)
        {
            var text = "The quick brown fox jumps over the lazy dog.";
            if (args.Length > 1)
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
            var zipFileName = Path.Combine(cacheDirName, "cognitiveservices.zip");
            var cache = new CachingStrategy()
                //.AddLayer(new ZipFileCacheLayer(zipFileName))
                .AppendLayer(new FileCacheLayer(cacheDir));

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
                Console.Write(outputFormat.Name);
                Console.Write(":> ");
                var ttsClient = new TextToSpeechClient(
                    region,
                    subscriptionKey,
                    resourceName,
                    voiceListDecoder,
                    outputFormat,
                    audioDecoder,
                    cache);

                var voices = await ttsClient
                    .GetVoices()
                    .ConfigureAwait(false);
                var voice = Array.Find(voices, v => v.Locale == "en-US" && v.Gender == "Female");

                try
                {
                    //await DecodeAudio(text, audioDecoder, ttsClient, voice);
                    await PlayAudio(text, audioDecoder, ttsClient, voice).ConfigureAwait(false);
                    Console.WriteLine("Success!");
                }
                catch (Exception exp)
                {
                    Console.WriteLine(exp.Message);
                }
            }
        }

        //private static async Task DecodeAudio(string text, NAudioAudioDataDecoder audioDecoder, TextToSpeechClient ttsClient, Voice voice)
        //{
        //    var audio = await ttsClient
        //        .GetDecodedAudio(text, voice.ShortName)
        //        .ConfigureAwait(false);
        //    await Play(audio).ConfigureAwait(false);
        //}

        private static async Task PlayAudio(string text, NAudioAudioDataDecoder audioDecoder, TextToSpeechClient ttsClient, Voice voice)
        {
            var audioStream = await ttsClient
                .GetAudioDataStream(text, voice.ShortName)
                .ConfigureAwait(false);
            var waveStream = audioDecoder.MakeDecodingStream(audioStream);
            var sr = waveStream.WaveFormat.SampleRate;
            var bps = waveStream.WaveFormat.BitsPerSample;
            Console.Write($"{bps} * {sr} = {bps * sr}");
            //await Task.Yield();
            await Play(waveStream).ConfigureAwait(false);
        }

        public static Task Play(AudioData audio)
        {
            var format = new WaveFormat(audio.format.sampleRate, audio.format.bitsPerSample, audio.format.channels);
            var sourceStream = new FloatsToPcmBytesStream(audio.dataStream, audio.format.bitsPerSample / 8);
            var waveStream = new RawSourceWaveStream(sourceStream, format);
            return Play(waveStream);
        }

        public static async Task Play(WaveStream waveStream)
        {
            using (waveStream)
            using (var waveOut = new WaveOut())
            {
                waveOut.Init(waveStream);
                waveOut.Play();
                while (waveOut.PlaybackState == PlaybackState.Playing)
                {
                    await Task.Yield();
                }
            }
        }
    }
}
