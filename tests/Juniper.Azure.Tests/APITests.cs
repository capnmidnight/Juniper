using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Juniper.Audio.NAudio;
using Juniper.Azure.CognitiveServices;
using Juniper.HTTP;
using Juniper.Serialization;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Juniper.Azure.Tests
{
    [TestClass]
    public class APITests
    {
        private readonly IDeserializer<string> plainText = new StreamStringDecoder();
        private readonly IDeserializer<Voice[]> voiceList = new JsonFactory<Voice[]>();
        private string subscriptionKey;
        private string region;
        private DirectoryInfo cacheDir;

        [TestInitialize]
        public void Setup()
        {
            var userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            var keyFile = Path.Combine(userProfile, "Projects", "DevKeys", "azure-speech.txt");
            var lines = File.ReadAllLines(keyFile);
            subscriptionKey = lines[0];
            region = lines[1];
            var cacheDirName = Path.Combine(userProfile, "Projects");
            cacheDir = new DirectoryInfo(cacheDirName);
        }

        private async Task<string> GetToken()
        {
            var tokenRequest = new AuthTokenRequest(region, subscriptionKey);
            var token = await tokenRequest.PostForDecoded(plainText);
            return token;
        }

        private async Task<Voice[]> GetVoices(string token)
        {
            var voicesRequest = new VoiceListRequest(region, token, cacheDir);
            var voices = await voicesRequest.GetDecoded(voiceList);
            return voices;
        }

        private async Task<SpeechRequest> MakeSpeechRequest()
        {
            var token = await GetToken();
            var voices = await GetVoices(token);

            var voice = (from v in voices
                         where v.ShortName == "en-US-JessaNeural"
                         select v)
                        .First();

            var audioRequest = new SpeechRequest(region, token, "dls-dev-speech-recognition", OutputFormat.Audio16KHz128KbitrateMonoMP3, cacheDir)
            {
                Text = "Hello, world",
                Voice = voice,
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

            using (var audio = await audioRequest.PostForStream(MediaType.Audio.Mpeg))
            {
                var mem = new MemoryStream();
                audio.CopyTo(mem);
                var buff = mem.ToArray();
                Assert.AreNotEqual(0, buff.Length);
            }
        }

        [TestMethod]
        public async Task DecodeAudio()
        {
            var mpeg = MediaType.Audio.Mpeg;
            var decoder = new NAudioAudioDataDecoder(mpeg);
            var audioRequest = await MakeSpeechRequest();
            var audio = await audioRequest.PostForDecoded(decoder);
            Assert.AreEqual(mpeg, audio.contentType);
            Assert.AreEqual(audio.samplesPerChannel * audio.numChannels, audio.data.Length);
            Assert.AreEqual(16000, audio.frequency);
        }
    }
}
