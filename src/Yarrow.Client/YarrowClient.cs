using System;
using System.IO;
using System.Threading.Tasks;

using Juniper.Google.Maps;
using Juniper.Google.Maps.Geocoding;
using Juniper.Google.Maps.StreetView;
using Juniper.Imaging;
using Juniper.Progress;
using Juniper.World.GIS;

namespace Yarrow.Client
{
    public class YarrowClient<T>
    {
        private readonly YarrowRequestConfiguration api;
        private readonly YarrowMetadataRequest metadataRequest;
        private readonly YarrowImageRequest<T> imageRequest;
        private readonly YarrowGeocodingRequest reverseGeocodeRequest;

        public YarrowClient(IImageDecoder<T> decoder)
        {
            var myPictures = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            var cacheDirName = Path.Combine(myPictures, "Yarrow");
            var cacheDir = new DirectoryInfo(cacheDirName);
            api = new YarrowRequestConfiguration(new Uri("http://localhost:8000"), cacheDir);
            metadataRequest = new YarrowMetadataRequest(api);
            imageRequest = new YarrowImageRequest<T>(api, decoder);
            reverseGeocodeRequest = new YarrowGeocodingRequest(api);
        }

        public Task<MetadataResponse> GetMetadata(PanoID pano, IProgress prog = null)
        {
            metadataRequest.Pano = pano;
            return metadataRequest.Get(prog);
        }

        public Task<MetadataResponse> GetMetadata(PlaceName placeName, IProgress prog = null)
        {
            metadataRequest.Location = placeName;
            return metadataRequest.Get(prog);
        }

        public Task<MetadataResponse> GetMetadata(LatLngPoint latLng, IProgress prog = null)
        {
            metadataRequest.LatLng = latLng;
            return metadataRequest.Get(prog);
        }

        public Task<T> GetImage(PanoID pano, IProgress prog = null)
        {
            imageRequest.Pano = pano;
            return imageRequest.GetJPEG(prog);
        }

        public Task<GeocodingResponse> ReverseGeocode(LatLngPoint latLng, IProgress prog = null)
        {
            reverseGeocodeRequest.LatLng = latLng;
            return reverseGeocodeRequest.Get(prog);
        }
    }
}