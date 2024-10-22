using System.Globalization;

namespace Juniper.World.GIS.Google.StreetView;

public abstract class AbstractStreetViewRequest<MediaTypeT> : AbstractGoogleMapsRequest<MediaTypeT>
    where MediaTypeT : MediaType
{
    private string? pano;
    private string? placeName;
    private LatLngPoint? location;
    private int radius;

    protected AbstractStreetViewRequest(HttpClient http, string path, string apiKey, string signingKey, MediaTypeT contentType)
        : base(http, path, contentType, apiKey, signingKey)
    { }

    public string? Pano
    {
        get => pano;
        set
        {
            placeName = default;
            location = default;
            pano = value;
            RemoveQuery(nameof(location));
            SetQuery(nameof(pano), value);
        }
    }

    public string? Place
    {
        get => placeName;
        set
        {
            placeName = value;
            location = default;
            pano = default;
            RemoveQuery(nameof(pano));
            SetQuery(nameof(location), value);
        }
    }

    public LatLngPoint? Location
    {
        get => location;
        set
        {
            placeName = default;
            location = value;
            pano = default;
            RemoveQuery(nameof(pano));
            SetQuery(nameof(location), value?.ToString(CultureInfo.InvariantCulture));
        }
    }

    public int Radius
    {
        get => radius;
        set
        {
            radius = value;
            SetQuery(nameof(radius), radius);
        }
    }
}