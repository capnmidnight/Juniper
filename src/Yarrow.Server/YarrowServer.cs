using System;
using System.IO;

using Juniper.Google.Maps;
using Juniper.HTTP;
using Juniper.Image;

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
            var keyFile = Path.Combine(cacheDirName, "keys.txt");
            var gmaps = GoogleMapsController.CreateController(keyFile, new Size(640, 640), cacheDir);
            server = HttpServerUtil.Start(
                args, info, warning, error,
                new YarrowController(gmaps));
        }

        public void Stop()
        {
            server.Stop();
        }
    }
}