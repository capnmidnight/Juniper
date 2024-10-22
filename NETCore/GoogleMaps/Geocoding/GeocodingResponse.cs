using Juniper.Google;
using System.Net;
using System.Runtime.Serialization;

namespace Juniper.World.GIS.Google.Geocoding;

[Serializable]
public class GeocodingResponse : ISerializable
{
    private static readonly string STATUS_FIELD = nameof(Status).ToLowerInvariant();
    private static readonly string RESULTS_FIELD = nameof(Results).ToLowerInvariant();
    private static readonly string ERROR_MESSAGE_FIELD = nameof(Error_Message).ToLowerInvariant();

    public HttpStatusCode? Status { get; }

    public GeocodingResult[]? Results { get; }

    public string? Error_Message { get; }

    protected GeocodingResponse(SerializationInfo info, StreamingContext context)
    {
        if (info is null)
        {
            throw new ArgumentNullException(nameof(info));
        }

        Status = info.GetString(STATUS_FIELD)?.MapToStatusCode();
        if (Status == HttpStatusCode.OK)
        {
            Results = info.GetValue<GeocodingResult[]>(RESULTS_FIELD);
        }
        else if (Status != HttpStatusCode.NoContent)
        {
            Error_Message = info.GetString(ERROR_MESSAGE_FIELD);
        }
    }

    public void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        if (info is null)
        {
            throw new ArgumentNullException(nameof(info));
        }

        info.AddValue(STATUS_FIELD, Status?.ToGoogleString());

        if (Status == HttpStatusCode.OK)
        {
            info.AddValue(RESULTS_FIELD, Results);
        }
        else if (Status != HttpStatusCode.NoContent)
        {
            info.AddValue(ERROR_MESSAGE_FIELD, Error_Message);
        }
    }
}