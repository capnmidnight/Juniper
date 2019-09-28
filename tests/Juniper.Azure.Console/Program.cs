using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Juniper.Audio.NAudio;
using Juniper.Azure.CognitiveServices;
using Juniper.HTTP;
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
            var keyFile = Path.Combine(userProfile, "Projects", "DevKeys", "azure-speech.txt");
            var lines = File.ReadAllLines(keyFile);
            var subscriptionKey = lines[0];
            var region = lines[1];
            var resourceName = "dls-dev-speech-recognition";

            var ttsClient = new TextToSpeechClient(
                region,
                subscriptionKey,
                resourceName,
                new JsonFactory<Voice[]>(),
                OutputFormat.Audio16KHz128KbitrateMonoMP3,
                new NAudioAudioDataDecoder(MediaType.Audio.Mpeg),
                cacheDir);

            var voices = await ttsClient.GetVoices();
            var voice = voices.FirstOrDefault(v => v.Locale == "en-US" && v.Gender == "Female");
            var audio = await ttsClient.Speak(text, voice.ShortName);

            var min = audio.data.Min();
            var max = audio.data.Max();

            Console.WriteLine($"{min} -> {max}");
        }
    }
}
