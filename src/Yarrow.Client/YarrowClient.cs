using System;
using System.IO;
using System.Threading.Tasks;

using Juniper.Google.Maps;
using Juniper.Google.Maps.StreetView;
using Juniper.Image;

namespace Yarrow.Client
{
    public class YarrowClient
    {
        private YarrowRequestConfiguration api;
        private YarrowMetadataRequest metadataRequest;
        private YarrowImageRequest imageRequest;

        public YarrowClient()
        {
            var myPictures = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            var cacheDirName = Path.Combine(myPictures, "Yarrow");
            var cacheDir = new DirectoryInfo(cacheDirName);
            api = new YarrowRequestConfiguration(new Uri("http://localhost:8000"), cacheDir);
            metadataRequest = new YarrowMetadataRequest(api);
            imageRequest = new YarrowImageRequest(api);
        }

        public Task<MetadataResponse> GetMetadata(PlaceName placeName)
        {
            metadataRequest.Place = placeName;
            return metadataRequest.Get();
        }

        public async Task<FileInfo> GetImage(PanoID pano)
        {
            imageRequest.Pano = pano;
            await imageRequest.GetJPEG();
            return imageRequest.CacheFile;
        }
    }
}