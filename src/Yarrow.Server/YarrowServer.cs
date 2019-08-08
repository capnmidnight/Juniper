using System;
using System.Drawing;
using System.IO;

using Juniper.Google.Maps;
using Juniper.HTTP;
using Juniper.Imaging.Windows;

namespace Yarrow.Server
{
    public class YarrowServer
    {
        private readonly HttpServer server;

        public YarrowServer(string[] args, Action<string> info, Action<string> warning, Action<string> error, string apiKey, string signingKey, DirectoryInfo cacheDir)
        {
            var gmaps = new GoogleMapsRequestConfiguration(apiKey, signingKey, cacheDir);
            server = HttpServerUtil.Create(
                args, info, warning, error,
                new YarrowMetadataController(gmaps),
                new YarrowGeocodingController(gmaps),
                new YarrowImageController<Image>(gmaps, new GDIImageDecoder(System.Drawing.Imaging.ImageFormat.Jpeg)));
        }

        public void Start()
        {
            server.Start();
        }

        public void Stop()
        {
            server.Stop();
        }
    }
}