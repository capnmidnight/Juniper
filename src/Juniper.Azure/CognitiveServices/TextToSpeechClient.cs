using System;
using System.Threading.Tasks;

using Juniper.Audio;
using Juniper.Caching;
using Juniper.HTTP;
using Juniper.Serialization;

namespace Juniper.Azure.CognitiveServices
{
    public class TextToSpeechClient
    {
        private readonly string azureSubscriptionKey;
        private readonly string azureRegion;
        private readonly string azureResourceName;
        private readonly OutputFormat outputFormat;
        private readonly IDeserializer<Voice[]> voiceListDecoder;
        private readonly IDeserializer<AudioData> audioDecoder;
        private readonly CachingStrategy cachingStrategy;

        private string authToken;

        private Voice[] voices;

        public bool IsAvailable { get; private set; } = true;

        public TextToSpeechClient(string azureRegion, string azureSubscriptionKey, string azureResourceName, IDeserializer<Voice[]> voiceListDecoder, OutputFormat outputFormat, IDeserializer<AudioData> audioDecoder, CachingStrategy cachingStrategy)
        {
            if (string.IsNullOrEmpty(azureRegion))
            {
                throw new ArgumentException("Must provide an service region", nameof(azureRegion));
            }

            if (string.IsNullOrEmpty(azureSubscriptionKey))
            {
                throw new ArgumentException("Must provide a subscription key", nameof(azureSubscriptionKey));
            }

            if (string.IsNullOrEmpty(azureResourceName))
            {
                throw new ArgumentException("Must provide a resource name that is tied to the subscription", nameof(azureResourceName));
            }

            if (voiceListDecoder == null
                || voiceListDecoder.ReadContentType != MediaType.Application.Json)
            {
                throw new ArgumentException("Most provide a JSON deserializer for the voice list data", nameof(voiceListDecoder));
            }

            if (outputFormat == null)
            {
                throw new ArgumentException("Must provide an audio output format", nameof(outputFormat));
            }

            if (audioDecoder == null)
            {
                throw new ArgumentException("Must provide an audio decoder", nameof(audioDecoder));
            }

            if (audioDecoder.ReadContentType != outputFormat.ContentType)
            {
                throw new ArgumentException($"Must provide a decoder that matches the output format type. Given {audioDecoder.ReadContentType.Value}. Expected {outputFormat.ContentType.Value}", nameof(audioDecoder));
            }

            this.azureRegion = azureRegion;
            this.azureSubscriptionKey = azureSubscriptionKey;
            this.azureResourceName = azureResourceName;
            this.voiceListDecoder = voiceListDecoder;
            this.outputFormat = outputFormat;
            this.audioDecoder = audioDecoder;
            this.cachingStrategy = cachingStrategy;
        }

        public TextToSpeechClient(string azureRegion, string azureSubscriptionKey, string azureResourceName, IDeserializer<Voice[]> voiceListDecoder, OutputFormat outputFormat, IDeserializer<AudioData> audioDecoder)
            : this(azureRegion, azureSubscriptionKey, azureResourceName, voiceListDecoder, outputFormat, audioDecoder, null)
        { }

        public void ClearError()
        {
            IsAvailable = true;
        }

        private async Task<string> GetAuthToken()
        {
            try
            {
                if (string.IsNullOrEmpty(authToken))
                {
                    var plainText = new StreamStringDecoder();
                    var authRequest = new AuthTokenRequest(azureRegion, azureSubscriptionKey);
                    authToken = await authRequest.GetDecoded(plainText);
                }
                return authToken;
            }
            catch
            {
                IsAvailable = false;
                throw;
            }
        }

        public async Task<Voice[]> GetVoices()
        {
            try
            {
                if (voices is null)
                {
                    var voiceListRequest = new VoiceListRequest(azureRegion);
                    if (!cachingStrategy.IsCached(voiceListRequest))
                    {
                        voiceListRequest.AuthToken = await GetAuthToken();
                    }
                    voices = await cachingStrategy.GetDecoded(voiceListRequest, voiceListDecoder);
                }

                return voices;
            }
            catch
            {
                IsAvailable = false;
                throw;
            }
        }

        public async Task<AudioData> Speak(string text, string voiceName, float rateChange, float pitchChange)
        {
            try
            {
                if (voiceName == null)
                {
                    var voices = await GetVoices();
                    foreach (var voice in voices)
                    {
                        if (voice.Locale == "en-US" && voice.Gender == "Female")
                        {
                            voiceName = voice.ShortName;
                            break;
                        }
                    }
                }

                if (voiceName == null)
                {
                    throw new InvalidOperationException("Could not find a default voice for text to speech commands");
                }

                var ttsRequest = new TextToSpeechRequest(azureRegion, azureResourceName, outputFormat)
                {
                    Text = text,
                    VoiceName = voiceName,
                    RateChange = rateChange,
                    PitchChange = pitchChange
                };

                if (!cachingStrategy.IsCached(ttsRequest))
                {
                    ttsRequest.AuthToken = await GetAuthToken();
                }

                return await cachingStrategy.GetDecoded(ttsRequest, audioDecoder);
            }
            catch
            {
                IsAvailable = false;
                throw;
            }
        }

        public Task<AudioData> Speak(string text, string voiceName, float rateChange)
        {
            return Speak(text, voiceName, rateChange, 0);
        }

        public Task<AudioData> Speak(string text, string voiceName)
        {
            return Speak(text, voiceName, 0, 0);
        }

        public Task<AudioData> Speak(string text)
        {
            return Speak(text, null, 0, 0);
        }
    }
}