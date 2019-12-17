using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Juniper.Audio;
using Juniper.Azure.CognitiveServices;
using Juniper.IO;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Juniper.Azure.Tests
{
    [TestClass]
    public class APITests
    {
        private readonly IJsonDecoder<Voice[]> voiceListDecoder = new JsonFactory<Voice[]>();
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
            cache = new CachingStrategy(cacheDir);
        }

        private async Task<string> GetToken()
        {
            var tokenRequest = new AuthTokenRequest(region, subscriptionKey);
            return await tokenRequest
                .Decode(new StringFactory())
                .ConfigureAwait(false);
        }

        private async Task<Voice[]> GetVoices()
        {
            var voiceListRequest = new VoiceListRequest(region);
            if (!cache.IsCached(voiceListRequest))
            {
                voiceListRequest.AuthToken = await GetToken().ConfigureAwait(false);
            }
            return await cache
                .Load(voiceListDecoder, voiceListRequest)
                .ConfigureAwait(false);
        }

        private async Task<TextToSpeechRequest> MakeSpeechRequest()
        {
            var voices = await GetVoices().ConfigureAwait(false);

            var voice = (from v in voices
                         where v.ShortName == "en-US-JessaNeural"
                         select v)
                        .First();

            var format = AudioFormat.Audio16KHz128KbitrateMonoMP3;
            var audioRequest = new TextToSpeechRequest(region, resourceName, format)
            {
                Text = "Hello, world",
                VoiceName = voice.ShortName,
                Style = SpeechStyle.Cheerful,
                RateChange = 0.75f,
                PitchChange = -0.1f
            };
            if (!cache.IsCached(audioRequest))
            {
                audioRequest.AuthToken = await GetToken().ConfigureAwait(false);
            }
            return audioRequest;
        }

        [TestMethod]
        public async Task GetAuthToken()
        {
            var token = await GetToken().ConfigureAwait(false);
            Assert.IsNotNull(token);
            Assert.AreNotEqual(0, token.Length);
        }

        [TestMethod]
        public async Task GetVoiceList()
        {
            var voices = await GetVoices().ConfigureAwait(false);

            Assert.IsNotNull(voices);
            Assert.AreNotEqual(0, voices.Length);
        }

        [TestMethod]
        public async Task GetVoiceListClient()
        {
            var voicesClient = new VoicesClient(region, subscriptionKey, new JsonFactory<Voice[]>());
            var voices = await voicesClient
                .GetVoices()
                .ConfigureAwait(false);

            Assert.IsNotNull(voices);
            Assert.AreNotEqual(0, voices.Length);
        }

        [TestMethod]
        public async Task GetAudioFile()
        {
            var audioRequest = await MakeSpeechRequest().ConfigureAwait(false);

            using (var audioStream = await cache
                .Open(audioRequest)
                .ConfigureAwait(false))
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
            var audioRequest = await MakeSpeechRequest().ConfigureAwait(false);
            var audioDecoder = new NAudioAudioDataDecoder
            {
                Format = audioRequest.OutputFormat
            };
            var audio = await cache
                .Load(audioDecoder, audioRequest)
                .ConfigureAwait(false);
            Assert.AreEqual(MediaType.Audio.PCMA, audio.format.ContentType);
            Assert.AreEqual(audioRequest.OutputFormat.sampleRate, audio.format.sampleRate);
        }
    }
}
