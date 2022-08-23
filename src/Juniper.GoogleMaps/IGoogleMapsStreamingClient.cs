using Juniper.Progress;

using System.IO;
using System.Threading.Tasks;

namespace Juniper.World.GIS.Google
{
    public interface IGoogleMapsStreamingClient
    {
        string Status { get; }

        void ClearError();

        Task<Stream> GetImageStreamAsync(string pano, int fovDegrees, int headingDegrees, int pitchDegrees, IProgress prog = null);
        Task<Stream> GetMetadataStreamAsync(LatLngPoint latLng, int searchRadius = 50, IProgress prog = null);
        Task<Stream> GetMetadataStreamAsync(string pano, int searchRadius = 50, IProgress prog = null);
        Task<Stream> ReverseGeocodeStreamAsync(LatLngPoint latLng, IProgress prog = null);
        Task<Stream> SearchMetadataStreamAsync(string placeName, int searchRadius = 50, IProgress prog = null);
        Task<Stream> SearchMetadataStreamAsync(string searchLocation, string searchPano, LatLngPoint searchPoint, int searchRadius, IProgress prog = null);
    }
}