using Juniper.IO;
using Juniper.Sound;

using System;
using System.IO;
using System.Threading.Tasks;

namespace Juniper.Speech.Azure.CognitiveServices
{
    public class TextToSpeechStreamClient : VoicesClient
    {
        private readonly string azureResourceName;

        public TextToSpeechStreamClient(string azureRegion, string azureSubscriptionKey, string azureResourceName, IJsonDecoder<Voice[]> voiceListDecoder, CachingStrategy cache)
            : base(azureRegion, azureSubscriptionKey, voiceListDecoder, cache)
        {
            if (string.IsNullOrEmpty(azureResourceName))
            {
                throw new ArgumentException("Must provide a resource name that is tied to the subscription", nameof(azureResourceName));
            }

            this.azureResourceName = azureResourceName;
        }

        public TextToSpeechStreamClient(string azureRegion, string azureSubscriptionKey, string azureResourceName, IJsonDecoder<Voice[]> voiceListDecoder)
            : this(azureRegion, azureSubscriptionKey, azureResourceName, voiceListDecoder, null)
        { }

        public async Task<Stream> GetAudioDataStreamAsync(AudioFormat outputFormat, string text, string voiceName, float rateChange = 0, float pitchChange = 0)
        {
            if (string.IsNullOrEmpty(text))
            {
                throw new ArgumentException("Must provide some text to generate speech.", nameof(text));
            }

            if (string.IsNullOrEmpty(voiceName))
            {
                throw new ArgumentException("Must provide a voice to generate speech.", nameof(voiceName));
            }

            try
            {
                var ttsRequest = new TextToSpeechRequest(AzureRegion, azureResourceName, outputFormat)
                {
                    Text = text,
                    VoiceName = voiceName,
                    RateChange = rateChange,
                    PitchChange = pitchChange
                };

                if (!Cache.IsCached(ttsRequest))
                {
                    ttsRequest.AuthToken = await GetAuthTokenAsync()
                        .ConfigureAwait(false);
                }

                return await Cache.OpenAsync(ttsRequest)
                    .ConfigureAwait(false);
            }
            catch
            {
                IsAvailable = false;
                throw;
            }
        }

        public Task<Stream> GetAudioDataStreamAsync(AudioFormat outputFormat, string text, Voice voice, float rateChange = 0, float pitchChange = 0)
        {
            return GetAudioDataStreamAsync(outputFormat, text, voice?.ShortName, rateChange, pitchChange);
        }
    }
}