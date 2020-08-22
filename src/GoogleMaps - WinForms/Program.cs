using System;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using Juniper.Imaging;
using Juniper.IO;
using Juniper.World.GIS;
using Juniper.World.GIS.Google;
using Juniper.World.GIS.Google.Geocoding;
using Juniper.World.GIS.Google.StreetView;

namespace Juniper
{
    internal static class Program
    {
        private static ImageViewer form;
        private static GoogleMapsClient<MetadataResponse> gmaps;
        private static IImageCodec<Image> imageDecoder;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.ThreadException += Application_ThreadException;

            var metadataDecoder = new JsonFactory<MetadataResponse>();
            var geocodingDecoder = new JsonFactory<GeocodingResponse>();
            var userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            var assetsRoot = Path.Combine(userProfile, "Box", "VR Initiatives", "Engineering", "Assets");
            var keyFileName = Path.Combine(assetsRoot, "DevKeys", "google-streetview.txt");
            var gmapsCacheDirName = Path.Combine(assetsRoot, "GoogleMaps");
            var gmapsCacheDir = new DirectoryInfo(gmapsCacheDirName);
            var cache = new CachingStrategy
            {
                new FileCacheLayer(gmapsCacheDir)
            };

            var lines = File.ReadAllLines(keyFileName);
            var apiKey = lines[0];
            var signingKey = lines[1];
            gmaps = new GoogleMapsClient<MetadataResponse>(
                apiKey, signingKey,
                metadataDecoder, geocodingDecoder,
                cache);

            imageDecoder = new GDICodec(MediaType.Image.Jpeg);

            form = new ImageViewer();
            form.LocationSubmitted += Form_LocationSubmitted;
            form.LatLngSubmitted += Form_LatLngSubmitted;
            form.PanoSubmitted += Form_PanoSubmitted;
            using (form)
            {
                Application.Run(form);
            }
        }

        private static async Task GetImageDataAsync(MetadataResponse metadata)
        {
            if (metadata.Status == System.Net.HttpStatusCode.OK)
            {
                var geo = await gmaps.ReverseGeocodeAsync(metadata.Location)
                    .ConfigureAwait(false);
                try
                {
                    using var stream = await gmaps.GetImageAsync(metadata.Pano_ID, 20, 0, 0)
                        .ConfigureAwait(false);
                    var image = imageDecoder.Deserialize(stream);
                    form.SetImage(metadata, geo, image);
                }
                catch (Exception exp)
                {
                    form.SetError(exp);
                }
            }
            else
            {
                form.SetError();
            }
        }

        private static void Form_LocationSubmitted(object sender, StringEventArgs e)
        {
            _ = LocationSubmittedAsync(e.Value)
                .ContinueWith((task) => form.SetError(task.Exception), CancellationToken.None, TaskContinuationOptions.OnlyOnFaulted, TaskScheduler.Default)
                .ConfigureAwait(false);
        }

        private static async Task LocationSubmittedAsync(string location)
        {
            var metadata = await gmaps.SearchMetadataAsync(location)
                            .ConfigureAwait(false);
            await GetImageDataAsync(metadata)
                .ConfigureAwait(false);
        }

        private static void Form_LatLngSubmitted(object sender, LatLngPointEventArgs e)
        {
            _ = LatLngSubmittedAsync(e.Value)
                .ContinueWith((task) => form.SetError(task.Exception), CancellationToken.None, TaskContinuationOptions.OnlyOnFaulted, TaskScheduler.Default)
                .ConfigureAwait(false);
        }

        private static async Task LatLngSubmittedAsync(LatLngPoint point)
        {
            var metadata = await gmaps.GetMetadataAsync(point)
                            .ConfigureAwait(false);
            await GetImageDataAsync(metadata)
                .ConfigureAwait(false);
        }

        private static void Form_PanoSubmitted(object sender, StringEventArgs e)
        {
            _ = PanoSubmittedAsync(e.Value)
                .ContinueWith((task) => form.SetError(task.Exception), CancellationToken.None, TaskContinuationOptions.OnlyOnFaulted, TaskScheduler.Default)
                .ConfigureAwait(false);
        }

        private static async Task PanoSubmittedAsync(string pano)
        {
            var metadata = await gmaps.GetMetadataAsync(pano)
                            .ConfigureAwait(false);
            await GetImageDataAsync(metadata)
                .ConfigureAwait(false);
        }

        private static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            form.SetError(e.Exception);
        }
    }
}