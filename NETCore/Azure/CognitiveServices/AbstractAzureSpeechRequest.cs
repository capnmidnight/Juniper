using Juniper.Progress;

namespace Juniper.Speech.Azure.CognitiveServices;

public abstract class AbstractAzureSpeechRequest<MediaTypeT> : AbstractAzureRequest<MediaTypeT>
    where MediaTypeT : MediaType
{
    protected AbstractAzureSpeechRequest(HttpClient http, HttpMethod method, string region, string path, MediaTypeT contentType)
        : base(http, method, region, "tts.speech", path, contentType)
    { }

    public string? AuthToken { get; set; }

    protected override void ModifyRequest(HttpRequestMessage request, IProgress? prog = null)
    {
        if (string.IsNullOrEmpty(AuthToken))
        {
            throw new InvalidOperationException("An AuthToken is required to be able to submit this request.");
        }

        base.ModifyRequest(request, prog);
        request.Header("Authorization", $"Bearer {AuthToken}");
    }
}
