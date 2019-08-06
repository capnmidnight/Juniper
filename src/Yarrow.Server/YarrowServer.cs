using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Juniper.Google.Maps;
using Juniper.HTTP;
using Juniper.Image;
using Juniper.Json;
using Juniper.Serialization;

namespace Yarrow.Server
{
    public class YarrowServer
    {
        private HttpServer server;
        private GoogleMapsController gmaps;
        private JsonFactory json;

        public YarrowServer(string[] args, Action<string> info, Action<string> warning, Action<string> error)
        {
            json = new JsonFactory();
            var myPictures = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            var cacheDirName = Path.Combine(myPictures, "GoogleMaps");
            var cacheDir = new DirectoryInfo(cacheDirName);
            var keyFile = Path.Combine(cacheDirName, "keys.txt");
            gmaps = GoogleMapsController.CreateController(keyFile, new Size(640, 640), cacheDir);

            server = HttpServerUtil.Start(args, info, warning, error, this);
        }

        public bool IsDone { get { return server.Done; } }

        [Route("/api/metadata\\?location=([^/]+)")]
        public void GetMetadata(HttpListenerContext context, string[] args)
        {
            Task.WaitAll(Task.Run(async () =>
            {
                var response = context.Response;
                var metadata = await gmaps.GetMetadata((PlaceName)args[1]);
                var data = json.Serialize(metadata);
                response.ContentType = "application/json";
                response.ContentLength64 = data.Length;
                response.StatusCode = (int)HttpStatusCode.OK;
                response.OutputStream.Write(data, 0, data.Length);
            }));
        }
    }
}