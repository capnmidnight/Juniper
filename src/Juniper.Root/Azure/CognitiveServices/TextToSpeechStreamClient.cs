using System;
using System.IO;
using System.Threading.Tasks;
using Juniper.IO;
using Juniper.Sound;

namespace Juniper.Azure.CognitiveServices
{
    public class TextToSpeechStreamClient : VoicesClient
    {
        private readonly string azureResourceName;

        public TextToSpeechStreamClient(string azureRegion, string azureSubscriptionKey, string azureResourceName, IJsonDecoder<Voice[]> voiceListDecoder, AudioFormat outputFormat, CachingStrategy cache)
            : base(azureRegion, azureSubscriptionKey, voiceListDecoder, cache)
        {
            if (string.IsNullOrEmpty(azureResourceName))
            {
                throw new ArgumentException("Must provide a resource name that is tied to the subscription", nameof(azureResourceName));
            }

            this.azureResourceName = azureResourceName;

            OutputFormat = outputFormat
                ?? throw new ArgumentException("Must provide an audio output format", nameof(outputFormat));
        }

        public virtual AudioFormat OutputFormat { get; set; }

        public TextToSpeechStreamClient(string azureRegion, string azureSubscriptionKey, string azureResourceName, IJsonDecoder<Voice[]> voiceListDecoder, AudioFormat outputFormat)
            : this(azureRegion, azureSubscriptionKey, azureResourceName, voiceListDecoder, outputFormat, null)
        { }

        public async Task<Stream> GetAudioDataStreamAsync(string text, string voiceName, float rateChange, float pitchChange)
        {
            try
            {
                var ttsRequest = new TextToSpeechRequest(azureRegion, azureResourceName, OutputFormat)
                {
                    Text = text,
                    VoiceName = voiceName,
                    RateChange = rateChange,
                    PitchChange = pitchChange
                };

                if (!cache.IsCached(ttsRequest))
                {
                    ttsRequest.AuthToken = await GetAuthTokenAsync()
                        .ConfigureAwait(false);
                }

                return await cache.OpenAsync(ttsRequest)
                    .ConfigureAwait(false);
            }
            catch
            {
                IsAvailable = false;
                throw;
            }
        }

        public Task<Stream> GetAudioDataStreamAsync(string text, Voice voice, float rateChange, float pitchChange)
        {
            return GetAudioDataStreamAsync(text, voice.ShortName, rateChange, pitchChange);
        }

        public Task<Stream> GetAudioDataStreamAsync(string text, string voiceName, float rateChange)
        {
            return GetAudioDataStreamAsync(text, voiceName, rateChange, 0);
        }

        public Task<Stream> GetAudioDataStreamAsync(string text, Voice voice, float rateChange)
        {
            return GetAudioDataStreamAsync(text, voice, rateChange, 0);
        }

        public Task<Stream> GetAudioDataStreamAsync(string text, string voiceName)
        {
            return GetAudioDataStreamAsync(text, voiceName, 0, 0);
        }

        public Task<Stream> GetAudioDataStreamAsync(string text, Voice voice)
        {
            return GetAudioDataStreamAsync(text, voice, 0, 0);
        }
    }
}