using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Juniper.Audio.NAudio;
using Juniper.Azure.CognitiveServices;
using Juniper.HTTP;
using Juniper.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Juniper.Azure.Tests
{
    [TestClass]
    public class APITests
    {
        private readonly IDeserializer<string> plainText = new StringFactory();
        private readonly IDeserializer<Voice[]> voiceListDecoder = new JsonFactory<Voice[]>();
        private string subscriptionKey;
        private string region;
        private string resourceName;
        private CachingStrategy cache;

        [TestInitialize]
        public void Setup()
        {
            var userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            var keyFile = Path.Combine(userProfile, "Projects", "DevKeys", "azure-speech.txt");
            var lines = File.ReadAllLines(keyFile);
            subscriptionKey = lines[0];
            region = lines[1];
            resourceName = lines[2];
            var cacheDirName = Path.Combine(userProfile, "Projects");
            var cacheDir = new DirectoryInfo(cacheDirName);
            var fileCache = new FileCacheLayer(cacheDir);
            cache = new CachingStrategy();
            cache.AddLayer(fileCache);
        }

        private async Task<string> GetToken()
        {
            var tokenRequest = new AuthTokenRequest(region, subscriptionKey);
            var token = await tokenRequest.Decode(new StringFactory());
            return token;
        }

        private async Task<Voice[]> GetVoices(string token)
        {
            var voiceListRequest = new VoiceListRequest(region)
            {
                AuthToken = token
            };
            var voices = await cache
                .GetStreamSource(voiceListRequest)
                .Decode(voiceListDecoder);
            return voices;
        }

        private async Task<TextToSpeechRequest> MakeSpeechRequest()
        {
            var token = await GetToken();
            var voices = await GetVoices(token);

            var voice = (from v in voices
                         where v.ShortName == "en-US-JessaNeural"
                         select v)
                        .First();

            var format = OutputFormat.Audio16KHz128KbitrateMonoMP3;
            var audioDecoder = new NAudioAudioDataDecoder(format.ContentType);
            var audioRequest = new TextToSpeechRequest(region, resourceName, format)
            {
                Text = "Hello, world",
                VoiceName = voice.ShortName,
                Style = SpeechStyle.Cheerful,
                RateChange = 0.75f,
                PitchChange = -0.1f
            };
            return audioRequest;
        }

        [TestMethod]
        public async Task GetAuthToken()
        {
            var token = await GetToken();
            Assert.IsNotNull(token);
            Assert.AreNotEqual(0, token.Length);
        }

        [TestMethod]
        public async Task GetVoiceList()
        {
            var token = await GetToken();
            var voices = await GetVoices(token);

            Assert.IsNotNull(voices);
            Assert.AreNotEqual(0, voices.Length);
        }

        [TestMethod]
        public async Task GetAudioFile()
        {
            var audioRequest = await MakeSpeechRequest();

            using (var audioStream = await cache
                .GetStreamSource(audioRequest)
                .GetStream())
            {
                var mem = new MemoryStream();
                audioStream.CopyTo(mem);
                var buff = mem.ToArray();
                Assert.AreNotEqual(0, buff.Length);
            }
        }

        [TestMethod]
        public async Task DecodeAudio()
        {
            var audioRequest = await MakeSpeechRequest();
            var audioDecoder = new NAudioAudioDataDecoder(audioRequest.ContentType);
            var audio = await cache
                .GetStreamSource(audioRequest)
                .Decode(audioDecoder);
            Assert.AreEqual(MediaType.Audio.Mpeg, audio.contentType);
            Assert.AreEqual(audio.samplesPerChannel * audio.numChannels, audio.data.Length);
            Assert.AreEqual(16000, audio.frequency);
        }
    }
}
