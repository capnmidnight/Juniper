using System;
using System.IO;
using System.Threading.Tasks;

using Juniper.Google.Maps;
using Juniper.Google.Maps.StreetView;
using Juniper.Image;
using Juniper.Progress;
using Juniper.World.GIS;

namespace Yarrow.Client
{
    public class YarrowClient
    {
        private readonly YarrowRequestConfiguration api;
        private readonly YarrowMetadataRequest metadataRequest;
        private readonly YarrowImageRequest imageRequest;

        public YarrowClient()
        {
            var myPictures = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            var cacheDirName = Path.Combine(myPictures, "Yarrow");
            var cacheDir = new DirectoryInfo(cacheDirName);
            api = new YarrowRequestConfiguration(new Uri("http://localhost:8000"), cacheDir);
            metadataRequest = new YarrowMetadataRequest(api);
            imageRequest = new YarrowImageRequest(api);
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

        public Task<ImageData> GetImage(PanoID pano, IProgress prog = null)
        {
            imageRequest.Pano = pano;
            return imageRequest.GetJPEG(prog);
        }
    }
}