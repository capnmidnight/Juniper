using System;
using System.Threading.Tasks;

using Juniper.Audio;
using Juniper.HTTP;
using Juniper.HTTP.REST;
using Juniper.Serialization;

namespace Juniper.Azure.CognitiveServices
{
    public class TextToSpeechClient
    {
        private readonly string azureSubscriptionKey;
        private readonly string azureRegion;
        private readonly VoiceListRequest voiceListRequest;
        private readonly TextToSpeechRequest ttsRequest;
        private readonly IDeserializer<Voice[]> voiceListDecoder;
        private readonly IDeserializer<AudioData> audioDecoder;
        private readonly CachingStrategy cachingStrategy;

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
                || voiceListDecoder.ContentType != MediaType.Application.Json)
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

            if (audioDecoder.ContentType != outputFormat.ContentType)
            {
                throw new ArgumentException($"Must provide a decoder that matches the output format type. Given {audioDecoder.ContentType.Value}. Expected {outputFormat.ContentType.Value}", nameof(audioDecoder));
            }

            this.azureRegion = azureRegion;
            this.azureSubscriptionKey = azureSubscriptionKey;
            this.voiceListDecoder = voiceListDecoder;
            this.audioDecoder = audioDecoder;
            this.cachingStrategy = cachingStrategy;

            voiceListRequest = new VoiceListRequest(azureRegion);
            ttsRequest = new TextToSpeechRequest(azureRegion, azureResourceName, outputFormat);
        }

        public TextToSpeechClient(string azureRegion, string azureSubscriptionKey, string azureResourceName, IDeserializer<Voice[]> voiceListDecoder, OutputFormat outputFormat, IDeserializer<AudioData> audioDecoder)
            : this(azureRegion, azureSubscriptionKey, azureResourceName, voiceListDecoder, outputFormat, audioDecoder, null)
        { }

        public void ClearError()
        {
            IsAvailable = true;
        }

        public async Task<Voice[]> GetVoices()
        {
            try
            {
                if (voices is null)
                {
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

        private Task<string> GetAuthToken()
        {
            try
            {
                var plainText = new StreamStringDecoder();
                var authRequest = new AuthTokenRequest(azureRegion, azureSubscriptionKey);
                return authRequest.GetDecoded(plainText);
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

                ttsRequest.Text = text;
                ttsRequest.VoiceName = voiceName;
                ttsRequest.RateChange = rateChange;
                ttsRequest.PitchChange = pitchChange;

                if(!cachingStrategy.IsCached(ttsRequest)
                    && string.IsNullOrEmpty(ttsRequest.AuthToken))
                {
                    if (voiceListRequest != null
                        && !string.IsNullOrEmpty(voiceListRequest.AuthToken))
                    {
                        ttsRequest.AuthToken = voiceListRequest.AuthToken;
                    }
                    else
                    {
                        ttsRequest.AuthToken = await GetAuthToken();
                    }
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