using Juniper.Caching;
using Juniper.IO;

namespace Juniper.Speech.Azure.CognitiveServices;

public class VoicesClient
{
    protected HttpClient Http { get; }
    private readonly string azureSubscriptionKey;
    private readonly IJsonDecoder<Voice[]> voiceListDecoder;

    private string? authToken;
    private Voice[]? voices;

    protected string AzureRegion { get; }
    protected CachingStrategy Cache { get; }

    public VoicesClient(HttpClient http, string azureRegion, string azureSubscriptionKey, IJsonDecoder<Voice[]> voiceListDecoder, CachingStrategy? cache = null)
    {
        if (string.IsNullOrEmpty(azureRegion))
        {
            throw new ArgumentException("Must provide a service region", nameof(azureRegion));
        }

        if (string.IsNullOrEmpty(azureSubscriptionKey))
        {
            throw new ArgumentException("Must provide a subscription key", nameof(azureSubscriptionKey));
        }

        if (voiceListDecoder?.InputContentType != MediaType.Application_Json)
        {
            throw new ArgumentException("Must provide a JSON deserializer for the voice list data", nameof(voiceListDecoder));
        }

        Cache = cache ?? new CachingStrategy();
        this.Http = http;
        AzureRegion = azureRegion;
        this.azureSubscriptionKey = azureSubscriptionKey;
        this.voiceListDecoder = voiceListDecoder;
    }

    public bool IsAvailable { get; protected set; } = true;

    public void ClearError()
    {
        IsAvailable = true;
    }

    protected async Task<string?> GetAuthTokenAsync()
    {
        try
        {
            if (string.IsNullOrEmpty(authToken))
            {
                var plainText = new StringFactory();
                var authRequest = new AuthTokenRequest(Http, AzureRegion, azureSubscriptionKey);
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

    public async Task<Voice[]?> GetVoicesAsync()
    {
        try
        {
            if (voices is null)
            {
                var voiceListRequest = new VoiceListRequest(Http, AzureRegion);
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