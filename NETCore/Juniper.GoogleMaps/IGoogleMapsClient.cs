using Juniper.Progress;
using Juniper.World.GIS.Google.Geocoding;
using Juniper.World.GIS.Google.StreetView;

using System.Threading.Tasks;

namespace Juniper.World.GIS.Google
{
    public interface IGoogleMapsClient
        : IGoogleMapsStreamingClient
    {
        Task<MetadataResponse> FindClosestMetadataAsync(LatLngPoint point, int searchRadius);
        Task<MetadataResponse> GetMetadataAsync(LatLngPoint latLng, int searchRadius = 50, IProgress prog = null);
        Task<MetadataResponse> GetMetadataAsync(string pano, int searchRadius = 50, IProgress prog = null);
        Task<MetadataResponse> SearchMetadataAsync(string placeName, int searchRadius = 50, IProgress prog = null);
        Task<MetadataResponse> SearchMetadataAsync(string searchLocation, string searchPano, LatLngPoint searchPoint, int searchRadius, IProgress prog = null);
        Task<GeocodingResponse> ReverseGeocodeAsync(LatLngPoint latLng, IProgress prog = null);
    }
}