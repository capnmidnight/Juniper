using System;
using System.Drawing;
using System.IO;

using Juniper.Google.Maps;
using Juniper.HTTP;
using Juniper.Imaging;
using Juniper.Imaging.Windows;

namespace Yarrow.Server
{
    public class YarrowServer
    {
        private readonly HttpServer server;

        public YarrowServer(int httpPort, int httpsPort, Action<string> info, Action<string> warning, Action<string> error, string apiKey, string signingKey, DirectoryInfo cacheDir)
        {
            var gmaps = new GoogleMapsRequestConfiguration(apiKey, signingKey, cacheDir);
            server = new HttpServer(
                httpPort, httpsPort,
                info, warning, error);

            var decoder = new GDIImageDecoder(ImageFormat.JPEG);
            server.AddRoutesFrom(new YarrowImageController<Image>(gmaps, decoder));
            server.AddRoutesFrom(new YarrowMetadataController(gmaps));
            server.AddRoutesFrom(new YarrowGeocodingController(gmaps));
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