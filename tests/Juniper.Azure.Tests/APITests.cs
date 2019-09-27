using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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

        [TestMethod]
        public async Task GetAuthToken()
        {
            var tokenRequest = new AuthTokenRequest(region, subscriptionKey);
            var token = await tokenRequest.PostForDecoded(plainText);
            Assert.IsNotNull(token);
            Assert.AreNotEqual(0, token.Length);
        }

        [TestMethod]
        public async Task GetVoiceList()
        {
            var tokenRequest = new AuthTokenRequest(region, subscriptionKey);
            var token = await tokenRequest.PostForDecoded(plainText);

            var voicesRequest = new VoiceListRequest(region, token, cacheDir);
            var voices = await voicesRequest.GetDecoded(voiceList);

            Assert.IsNotNull(voices);
            Assert.AreNotEqual(0, voices.Length);
        }

        [TestMethod]
        public async Task GetAudioFile()
        {
            var tokenRequest = new AuthTokenRequest(region, subscriptionKey);
            var token = await tokenRequest.PostForDecoded(plainText);

            var voicesRequest = new VoiceListRequest(region, token, cacheDir);
            var voices = await voicesRequest.GetDecoded(voiceList);

            var voice = (from v in voices
                         where v.Locale == "en-US"
                            && v.Gender == "Male"
                         select v)
                        .First();

            var audioRequest = new SpeechRequest(region, token, "dls-dev-speech-recognition", cacheDir)
            {
                Text = "Hello, world",
                VoiceName = voice.Name,
                OutputFormat = OutputFormat.Audio16KHz128KbitrateMonoMP3
            };

            using (var audio = await audioRequest.PostForStream(MediaType.Audio.Mpeg))
            {
                var mem = new MemoryStream();
                audio.CopyTo(mem);
                var buff = mem.ToArray();
                Assert.AreNotEqual(0, buff.Length);
            }
        }
    }
}
