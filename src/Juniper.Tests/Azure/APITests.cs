using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Juniper.IO;
using Juniper.Sound;
using Juniper.Speech.Azure;
using Juniper.Speech.Azure.CognitiveServices;

using NUnit.Framework;

namespace Juniper.Azure.Tests
{
    [TestFixture]
    public class APITests
    {
        private readonly IJsonDecoder<Voice[]> voiceListDecoder = new JsonFactory<Voice[]>();
        private string subscriptionKey;
        private string region;
        private string resourceName;
        private CachingStrategy cache;

        [SetUp]
        public void Setup()
        {
            var userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            var assetsRoot = Path.Combine(userProfile, "Box", "VR Initiatives", "Engineering", "Assets");
            var keyFile = Path.Combine(assetsRoot, "DevKeys", "azure-speech.txt");
            var lines = File.ReadAllLines(keyFile);
            subscriptionKey = lines[0];
            region = lines[1];
            resourceName = lines[2];
            var cacheDirName = Path.Combine(assetsRoot, "Azure");
            var cacheDir = new DirectoryInfo(cacheDirName);
            cache = new CachingStrategy
            {
                new FileCacheLayer(cacheDir)
            };
        }

        private Task<string> GetTokenAsync()
        {
            var tokenRequest = new AuthTokenRequest(region, subscriptionKey);
            return tokenRequest
                .DecodeAsync(new StringFactory());
        }

        private async Task<Voice[]> GetVoicesAsync()
        {
            var voiceListRequest = new VoiceListRequest(region);
            if (!cache.IsCached(voiceListRequest))
            {
                voiceListRequest.AuthToken = await GetTokenAsync().ConfigureAwait(false);
            }

            return await cache
                .LoadAsync(voiceListDecoder, voiceListRequest)
                .ConfigureAwait(false);
        }

        private async Task<TextToSpeechRequest> MakeSpeechRequestAsync()
        {
            var voices = await GetVoicesAsync().ConfigureAwait(false);

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
                audioRequest.AuthToken = await GetTokenAsync().ConfigureAwait(false);
            }

            return audioRequest;
        }

        [Test]
        public async Task GetAuthTokenAsync()
        {
            var token = await GetTokenAsync().ConfigureAwait(false);
            Assert.IsNotNull(token);
            Assert.AreNotEqual(0, token.Length);
        }

        [Test]
        public async Task GetVoiceListAsync()
        {
            var voices = await GetVoicesAsync().ConfigureAwait(false);

            Assert.IsNotNull(voices);
            Assert.AreNotEqual(0, voices.Length);
        }

        [Test]
        public async Task GetVoiceListClientAsync()
        {
            var voicesClient = new VoicesClient(region, subscriptionKey, new JsonFactory<Voice[]>());
            var voices = await voicesClient
                .GetVoicesAsync()
                .ConfigureAwait(false);

            Assert.IsNotNull(voices);
            Assert.AreNotEqual(0, voices.Length);
        }

        [Test]
        public async Task GetAudioFileAsync()
        {
            var audioRequest = await MakeSpeechRequestAsync().ConfigureAwait(false);

            using var audioStream = await cache
                .OpenAsync(audioRequest)
                .ConfigureAwait(false);
            using var mem = new MemoryStream();
            await audioStream.CopyToAsync(mem)
                .ConfigureAwait(false);
            var buff = mem.ToArray();
            Assert.AreNotEqual(0, buff.Length);
        }

        [Test]
        public async Task DecodeAudioAsync()
        {
            var audioRequest = await MakeSpeechRequestAsync().ConfigureAwait(false);
            var audioDecoder = new NAudioAudioDataDecoder
            {
                Format = audioRequest.OutputFormat
            };
            using var audio = await cache
                .LoadAsync(audioDecoder, audioRequest)
                .ConfigureAwait(false);
            Assert.AreEqual(MediaType.Audio.PCMA, audio.Format.ContentType);
            Assert.AreEqual(audioRequest.OutputFormat.SampleRate, audio.Format.SampleRate);
        }
    }
}
