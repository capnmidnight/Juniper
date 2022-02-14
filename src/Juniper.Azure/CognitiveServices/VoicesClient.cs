using Juniper.IO;

using System;
using System.Threading.Tasks;

namespace Juniper.Speech.Azure.CognitiveServices
{
    public class VoicesClient
    {
        private readonly string azureSubscriptionKey;
        private readonly IJsonDecoder<Voice[]> voiceListDecoder;

        private string authToken;
        private Voice[] voices;

        protected string AzureRegion { get; }
        protected CachingStrategy Cache { get; }

        public VoicesClient(string azureRegion, string azureSubscriptionKey, IJsonDecoder<Voice[]> voiceListDecoder, CachingStrategy cache)
        {
            if (string.IsNullOrEmpty(azureRegion))
            {
                throw new ArgumentException("Must provide a service region", nameof(azureRegion));
            }

            if (string.IsNullOrEmpty(azureSubscriptionKey))
            {
                throw new ArgumentException("Must provide a subscription key", nameof(azureSubscriptionKey));
            }

            if (voiceListDecoder is null
                || voiceListDecoder.InputContentType != MediaType.Application_Json)
            {
                throw new ArgumentException("Must provide a JSON deserializer for the voice list data", nameof(voiceListDecoder));
            }

            Cache = cache ?? new CachingStrategy();
            AzureRegion = azureRegion;
            this.azureSubscriptionKey = azureSubscriptionKey;
            this.voiceListDecoder = voiceListDecoder;
        }

        public VoicesClient(string azureRegion, string azureSubscriptionKey, IJsonDecoder<Voice[]> voiceListDecoder)
            : this(azureRegion, azureSubscriptionKey, voiceListDecoder, null)
        { }

        public bool IsAvailable { get; protected set; } = true;

        public void ClearError()
        {
            IsAvailable = true;
        }

        protected async Task<string> GetAuthTokenAsync()
        {
            try
            {
                if (string.IsNullOrEmpty(authToken))
                {
                    var plainText = new StringFactory();
                    var authRequest = new AuthTokenRequest(AzureRegion, azureSubscriptionKey);
                    authToken = await authRequest.DecodeAsync(plainText)
                        .ConfigureAwait(false);
                }

                return authToken;
            }
            catch
            {
                IsAvailable = false;
                throw;
            }
        }

        public async Task<Voice[]> GetVoicesAsync()
        {
            try
            {
                if (voices is null)
                {
                    var voiceListRequest = new VoiceListRequest(AzureRegion);
                    if (!Cache.IsCached(voiceListRequest))
                    {
                        voiceListRequest.AuthToken = await GetAuthTokenAsync()
                            .ConfigureAwait(false);
                    }

                    voices = await Cache.LoadAsync(voiceListDecoder, voiceListRequest)
                        .ConfigureAwait(false);
                }

                return voices;
            }
            catch
            {
                IsAvailable = false;
                throw;
            }
        }
    }
}