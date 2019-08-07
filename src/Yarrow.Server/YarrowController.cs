using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

using Juniper.Google.Maps;
using Juniper.Google.Maps.StreetView;
using Juniper.HTTP;
using Juniper.Image;
using Juniper.World.GIS;

namespace Yarrow.Server
{
    public class YarrowController
    {
        private readonly GoogleMapsRequestConfiguration gmaps;
        private readonly Size imageSize;

        public YarrowController()
        {
            imageSize = new Size(640, 640);
            var myPictures = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            var cacheDirName = Path.Combine(myPictures, "GoogleMaps");
            var cacheDir = new DirectoryInfo(cacheDirName);
            var keyFileName = Path.Combine(cacheDirName, "keys.txt");
            var keyFile = new FileInfo(keyFileName);
            using (var fileStream = keyFile.OpenRead())
            using (var reader = new StreamReader(fileStream))
            {
                var apiKey = reader.ReadLine();
                var signingKey = reader.ReadLine();
                gmaps = new GoogleMapsRequestConfiguration(apiKey, signingKey, cacheDir);
            }
        }

        [Route("/api/metadata\\?location=([^/]+)")]
        public void GetMetadataFromPlaceName(HttpListenerContext context, string location)
        {
            var metadataRequest = new MetadataRequest(gmaps, (PlaceName)location);
            Task.WaitAll(metadataRequest.Proxy(context.Response));
        }

        [Route("/api/metadata\\?latlng=([^/]+)")]
        public void GetMetadataFromLatLng(HttpListenerContext context, string location)
        {
            var metadataRequest = new MetadataRequest(gmaps, LatLngPoint.ParseDecimal(location));
            Task.WaitAll(metadataRequest.Proxy(context.Response));
        }

        [Route("/api/image\\?pano=([^/]+)")]
        public void GetImage(HttpListenerContext context, string pano)
        {
            var imageRequest = new CrossCubeMapRequest(gmaps, (PanoID)pano, imageSize);
            Task.WaitAll(imageRequest.ProxyJPEG(context.Response));
        }
    }
}