using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Juniper.Audio.NAudio;
using Juniper.Azure.CognitiveServices;
using Juniper.Caching;
using Juniper.HTTP;
using Juniper.HTTP.REST;
using Juniper.Serialization;

namespace Juniper.Azure
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var text = "Hello, world";
            if(args.Length > 1)
            {
                text = args[1];
            }

            var userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            var cacheDirName = Path.Combine(userProfile, "Projects");
            var cacheDir = new DirectoryInfo(cacheDirName);
            var fileCache = new FileCacheLayer(cacheDir);
            var cache = new CachingStrategy().AddLayer(fileCache);
            var keyFile = Path.Combine(userProfile, "Projects", "DevKeys", "azure-speech.txt");
            var lines = File.ReadAllLines(keyFile);
            var subscriptionKey = lines[0];
            var region = lines[1];
            var resourceName = "dls-dev-speech-recognition";
            var audioDecoder = new NAudioAudioDataDecoder(MediaType.Audio.Mpeg);
            var voiceListDecoder = new JsonFactory<Voice[]>();
            var ttsClient = new TextToSpeechClient(
                region,
                subscriptionKey,
                resourceName,
                voiceListDecoder,
                OutputFormat.Audio16KHz128KbitrateMonoMP3,
                audioDecoder,
                cache);

            var voices = await ttsClient.GetVoices();
            var voice = voices.FirstOrDefault(v => v.Locale == "en-US" && v.Gender == "Female");
            var audio = await ttsClient.Speak(text, voice.ShortName);

            var min = audio.data.Min();
            var max = audio.data.Max();

            Console.WriteLine($"{min} -> {max}");
        }
    }
}
