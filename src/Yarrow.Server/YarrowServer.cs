using System;
using System.IO;
using Juniper.Google.Maps;
using Juniper.HTTP;

namespace Yarrow.Server
{
    public class YarrowServer
    {
        private readonly HttpServer server;

        public YarrowServer(string[] args, Action<string> info, Action<string> warning, Action<string> error)
        {
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
                var gmaps = new GoogleMapsRequestConfiguration(apiKey, signingKey, cacheDir);
                server = HttpServerUtil.Start(
                    args, info, warning, error,
                    new YarrowMetadataController(gmaps),
                    new YarrowGeocodingController(gmaps),
                    new YarrowImageController(gmaps));
            }
        }

        public void Stop()
        {
            server.Stop();
        }
    }
}