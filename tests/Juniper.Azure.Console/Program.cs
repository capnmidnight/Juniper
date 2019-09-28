using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Juniper.Audio.NAudio;
using Juniper.Azure;
using Juniper.Azure.CognitiveServices;
using Juniper.HTTP;
using Juniper.Serialization;

namespace ConsoleApp1
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var text = "The quick, brown fox jump over the lazy dog.";
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

            var voicesRequest = new VoiceListRequest(region, cacheDir);
            var voice = await GetVoice(voicesRequest, region, subscriptionKey);

            var mpeg = MediaType.Audio.Mpeg;
            var decoder = new NAudioAudioDataDecoder(mpeg);
            var audioRequest = new SpeechRequest(region, "dls-dev-speech-recognition", OutputFormat.Audio16KHz128KbitrateMonoMP3, cacheDir)
            {
                Text = text,
                Voice = voice,
                Style = SpeechStyle.Cheerful,
                RateChange = 0.75F,
                PitchChange = -0.1F
            };
            if(!audioRequest.GetCacheFileName(decoder.ContentType).Exists)
            {
                audioRequest.AuthToken = voicesRequest.AuthToken
                    ?? await GetToken(region, subscriptionKey);
            }

            var audio = await audioRequest.PostForDecoded(decoder);

            var min = audio.data.Min();
            var max = audio.data.Max();

            System.Console.WriteLine($"{min} -> {max}");
        }

        private static async Task<Voice> GetVoice(VoiceListRequest voicesRequest, string region, string subscriptionKey)
        {
            var voiceList = new JsonFactory<Voice[]>();
            if (!voicesRequest.GetCacheFileName(voiceList.ContentType).Exists)
            {
                voicesRequest.AuthToken = await GetToken(region, subscriptionKey);
            }
            var voices = await voicesRequest.GetDecoded(voiceList);
            var voice = (from v in voices
                         where v.ShortName == "en-US-JessaNeural"
                         select v).First();
            return voice;
        }

        private static async Task<string> GetToken(string region, string subscriptionKey)
        {
            var plainText = new StreamStringDecoder();
            var tokenRequest = new AuthTokenRequest(region, subscriptionKey);
            var token = await tokenRequest.PostForDecoded(plainText);
            return token;
        }
    }
}
