using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Juniper.Audio;
using Juniper.Azure.CognitiveServices;
using Juniper.IO;

namespace Juniper.Azure
{
    class Program
    {
        static async Task Main(string[] args)
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
                .AddLayer(new ZipFileCacheLayer(zipFileName))
                .AddLayer(new FileCacheLayer(cacheDir));

            var voiceListDecoder = new JsonFactory<Voice[]>();
            var outputFormat = AudioFormat.Audio24KHz48KbitrateMonoMP3;
            var audioDecoder = new NAudioAudioDataDecoder();
            var ttsClient = new TextToSpeechClient(
                region,
                subscriptionKey,
                resourceName,
                voiceListDecoder,
                outputFormat,
                audioDecoder,
                cache);

            var voices = await ttsClient.GetVoices();
            var voice = voices.FirstOrDefault(v => v.Locale == "en-US" && v.Gender == "Female");
            await DecodeAudio(text, audioDecoder, ttsClient, voice);
            //await PlayAudio(text, audioDecoder, ttsClient, voice);
        }

        private static async Task DecodeAudio(string text, NAudioAudioDataDecoder audioDecoder, TextToSpeechClient ttsClient, Voice voice)
        {
            var audio = await ttsClient.GetDecodedAudio(text, voice.ShortName);
            Console.WriteLine($"content type: {audio.format.ContentType.Value}");
            Console.WriteLine($"min: {audio.data.Min()}, max: {audio.data.Max()}");
            Console.WriteLine($"channels: {audio.format.channels}, samples: {audio.data.Length}, sample rate: {audio.format.sampleRate}");
            await audioDecoder.Play(audio);
        }

        private static async Task PlayAudio(string text, NAudioAudioDataDecoder audioDecoder, TextToSpeechClient ttsClient, Voice voice)
        {
            var audioStream = await ttsClient.GetAudioDataStream(text, voice.ShortName);
            var waveStream = audioDecoder.MakeDecodingStream(audioStream);
            Console.WriteLine($"stream type: {waveStream.GetType().Name}");
            Console.WriteLine($"channels: {waveStream.WaveFormat.Channels}, sample rate: {waveStream.WaveFormat.SampleRate}");
            await audioDecoder.Play(waveStream);
        }
    }
}
