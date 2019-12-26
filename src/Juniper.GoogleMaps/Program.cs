using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

using Juniper.Imaging;
using Juniper.IO;
using Juniper.World.GIS;
using Juniper.World.GIS.Google;
using Juniper.World.GIS.Google.Geocoding;
using Juniper.World.GIS.Google.StreetView;

namespace Juniper.GoogleMaps
{
    internal static class Program
    {
        private static readonly string MY_PICTURES = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);

        private static ImageViewer form;
        private static GoogleMapsClient gmaps;
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
            var gmapsCacheDirName = Path.Combine(MY_PICTURES, "GoogleMaps");
            var gmapsCacheDir = new DirectoryInfo(gmapsCacheDirName);
            var cache = new CachingStrategy()
                .AppendLayer(new FileCacheLayer(gmapsCacheDir));

            var userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            var keyFileName = Path.Combine(userProfile, "Projects", "DevKeys", "google-streetview.txt");
            var lines = File.ReadAllLines(keyFileName);
            var apiKey = lines[0];
            var signingKey = lines[1];
            gmaps = new GoogleMapsClient(
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

        private static async Task GetImageData(MetadataResponse metadata)
        {
            if (metadata.status == System.Net.HttpStatusCode.OK)
            {
                var geo = await gmaps.ReverseGeocodeAsync(metadata.location)
                    .ConfigureAwait(false);
                try
                {
                    using (var stream = await gmaps.GetImageAsync(metadata.pano_id, 20, 0, 0)
                        .ConfigureAwait(false))
                    {
                        var image = imageDecoder.Deserialize(stream);
                        form.SetImage(metadata, geo, image);
                    }
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

        private static async void Form_LocationSubmitted(object sender, string location)
        {
            var metadata = await gmaps.SearchMetadataAsync(location)
                .ConfigureAwait(false);
            await GetImageData(metadata)
                .ConfigureAwait(false);
        }

        private static async void Form_LatLngSubmitted(object sender, string latlng)
        {
            var metadata = await gmaps.GetMetadataAsync(LatLngPoint.ParseDecimal(latlng))
                .ConfigureAwait(false);
            await GetImageData(metadata)
                .ConfigureAwait(false);
        }

        private static async void Form_PanoSubmitted(object sender, string pano)
        {
            var metadata = await gmaps.GetMetadataAsync(pano)
                .ConfigureAwait(false);
            await GetImageData(metadata)
                .ConfigureAwait(false);
        }

        private static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            form.SetError(e.Exception);
        }
    }
}