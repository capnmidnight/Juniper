using System;
using System.Threading.Tasks;

using Juniper.IO;

namespace Juniper.Azure.CognitiveServices
{
    public class VoicesClient
    {
        private readonly string azureSubscriptionKey;
        private readonly IJsonDecoder<Voice[]> voiceListDecoder;

        private string authToken;
        private Voice[] voices;

        protected readonly string azureRegion;
        protected readonly CachingStrategy cache;

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

            if (voiceListDecoder == null
                || voiceListDecoder.ContentType != MediaType.Application.Json)
            {
                throw new ArgumentException("Must provide a JSON deserializer for the voice list data", nameof(voiceListDecoder));
            }

            this.azureRegion = azureRegion;
            this.azureSubscriptionKey = azureSubscriptionKey;
            this.voiceListDecoder = voiceListDecoder;
            this.cache = cache ?? new CachingStrategy();
        }

        public VoicesClient(string azureRegion, string azureSubscriptionKey, IJsonDecoder<Voice[]> voiceListDecoder)
            : this(azureRegion, azureSubscriptionKey, voiceListDecoder, null)
        { }

        public bool IsAvailable { get; protected set; } = true;

        public void ClearError()
        {
            IsAvailable = true;
        }

        protected async Task<string> GetAuthToken()
        {
            try
            {
                if (string.IsNullOrEmpty(authToken))
                {
                    var plainText = new StringFactory();
                    var authRequest = new AuthTokenRequest(azureRegion, azureSubscriptionKey);
                    authToken = await authRequest.Decode(plainText);
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
                    if (!cache.IsCached(voiceListRequest))
                    {
                        voiceListRequest.AuthToken = await GetAuthToken();
                    }
                    voices = await cache.Load(voiceListDecoder, voiceListRequest);
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