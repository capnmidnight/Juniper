namespace Juniper.World.GIS.Google.TimeZone;

public class TimeZoneRequest : AbstractGoogleMapsRequest<MediaType.Application>
{
    private LatLngPoint? location;
    private DateTime? timestamp;

    public TimeZoneRequest(HttpClient http, string apiKey)
        : base(http, "timezone/json", MediaType.Application_Json, apiKey, null)
    { }

    public LatLngPoint? Location
    {
        get => location;
        set
        {
            location = value;
            SetQuery(nameof(location), location);
        }
    }

    public DateTime? Timestamp
    {
        get => timestamp;
        set
        {
            timestamp = value;
            DateTimeOffset? offset = timestamp is not null 
                ? new DateTimeOffset(timestamp.Value)
                : null;
            SetQuery(nameof(timestamp), offset?.ToUnixTimeSeconds());
        }
    }
}