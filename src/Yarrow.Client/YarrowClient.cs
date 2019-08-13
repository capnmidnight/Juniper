using System;
using System.IO;
using System.Threading.Tasks;

using Juniper.Google.Maps;
using Juniper.Google.Maps.Geocoding;
using Juniper.Google.Maps.StreetView;
using Juniper.HTTP.REST;
using Juniper.Imaging;
using Juniper.Progress;
using Juniper.Serialization;
using Juniper.World.GIS;

namespace Yarrow.Client
{
    public class YarrowClient<T>
        where T : class
    {
        private readonly IImageDecoder<T> decoder;
        private readonly YarrowRequestConfiguration yarrow;

        private readonly GoogleMapsRequestConfiguration gmaps;

        private bool useGoogleMaps = false;
        private Exception lastError;

        public YarrowClient(Uri yarrowServerUri, IImageDecoder<T> decoder, DirectoryInfo yarrowCacheDir)
        {
            this.decoder = decoder;
            yarrow = new YarrowRequestConfiguration(yarrowServerUri, yarrowCacheDir);
        }

        public YarrowClient(Uri yarrowServerUri, IImageDecoder<T> decoder, DirectoryInfo yarrowCacheDir, string gmapsApiKey, string gmapsSigningKey, DirectoryInfo gmapsCacheDir)
            : this(yarrowServerUri, decoder, yarrowCacheDir)
        {
            gmaps = new GoogleMapsRequestConfiguration(gmapsApiKey, gmapsSigningKey, gmapsCacheDir);
        }

        public string Status
        {
            get
            {
                var errorMsg = lastError?.Message ?? "NONE";
                return $"Using google maps directly: {useGoogleMaps.ToString()}. Last error: {errorMsg}";
            }
        }

        public void ClearError()
        {
            lastError = null;
        }

        public string ServiceURL
        {
            get
            {
                if (useGoogleMaps)
                {
                    return gmaps.baseServiceURI.ToString();
                }
                else
                {
                    return yarrow.baseServiceURI.ToString();
                }
            }
        }

        private Task<ResultT> Cascade<YarrowRequestT, GmapsRequestT, DecoderT, ResultT>(
            YarrowRequestT yarrowRequest,
            GmapsRequestT gmapsRequest,
            Func<AbstractRequest<DecoderT, ResultT>, IProgress, Task<ResultT>> getter,
            IProgress prog)
            where DecoderT : IDeserializer<ResultT>
            where YarrowRequestT : AbstractRequest<DecoderT, ResultT>
            where GmapsRequestT : AbstractRequest<DecoderT, ResultT>
        {
            return Task.Run(async () =>
            {
                var request = useGoogleMaps
                    ? (AbstractRequest<DecoderT, ResultT>)gmapsRequest
                    : yarrowRequest;

                try
                {
                    return await getter(request, prog);
                }
#pragma warning disable CA1031 // Do not catch general exception types
                catch (Exception exp)
                {
                    lastError = exp;
                    if (useGoogleMaps)
                    {
                        throw;
                    }
                    else
                    {
                        useGoogleMaps = true;
                        return await Cascade(yarrowRequest, gmapsRequest, getter, prog);
                    }
                }
#pragma warning restore CA1031 // Do not catch general exception types
            });
        }

        private static readonly Func<AbstractRequest<IDeserializer<MetadataResponse>, MetadataResponse>, IProgress, Task<MetadataResponse>> getMetadata =
            new Func<AbstractRequest<IDeserializer<MetadataResponse>, MetadataResponse>, IProgress, Task<MetadataResponse>>((req, p) => req.Get(p));

        public Task<MetadataResponse> GetMetadata(PanoID pano, IProgress prog = null)
        {
            var yarrowMetadataRequest = new YarrowMetadataRequest(yarrow);
            var gmapsMetadataRequest = new MetadataRequest(gmaps);
            yarrowMetadataRequest.Pano = gmapsMetadataRequest.Pano = pano;
            return Cascade(yarrowMetadataRequest, gmapsMetadataRequest, getMetadata, prog);
        }

        public Task<MetadataResponse> GetMetadata(PlaceName placeName, IProgress prog = null)
        {
            var yarrowMetadataRequest = new YarrowMetadataRequest(yarrow);
            var gmapsMetadataRequest = new MetadataRequest(gmaps);
            yarrowMetadataRequest.Place = gmapsMetadataRequest.Place = placeName;
            return Cascade(yarrowMetadataRequest, gmapsMetadataRequest, getMetadata, prog);
        }

        public Task<MetadataResponse> GetMetadata(LatLngPoint latLng, IProgress prog = null)
        {
            var yarrowMetadataRequest = new YarrowMetadataRequest(yarrow);
            var gmapsMetadataRequest = new MetadataRequest(gmaps);
            yarrowMetadataRequest.Location = gmapsMetadataRequest.Location = latLng;
            return Cascade(yarrowMetadataRequest, gmapsMetadataRequest, getMetadata, prog);
        }

        private static readonly Func<AbstractRequest<IImageDecoder<T>, T>, IProgress, Task<T>> getImage =
            new Func<AbstractRequest<IImageDecoder<T>, T>, IProgress, Task<T>>(AbstractRequestExt.GetImage);

        public Task<T> GetImage(PanoID pano, int fov, int heading, int pitch, IProgress prog = null)
        {
            var yarrowImageRequest = new YarrowImageRequest<T>(yarrow, decoder);
            var gmapsImageRequest = new ImageRequest<T>(gmaps, decoder, new Size(640, 640));
            yarrowImageRequest.Pano = gmapsImageRequest.Pano = pano;
            yarrowImageRequest.FOV = gmapsImageRequest.FOV = fov;
            yarrowImageRequest.Heading = gmapsImageRequest.Heading = heading;
            yarrowImageRequest.Pitch = gmapsImageRequest.Pitch = pitch;
            return Cascade(yarrowImageRequest, gmapsImageRequest, getImage, prog);
        }

        private static readonly Func<AbstractRequest<IDeserializer<GeocodingResponse>, GeocodingResponse>, IProgress, Task<GeocodingResponse>> getGeocode =
            new Func<AbstractRequest<IDeserializer<GeocodingResponse>, GeocodingResponse>, IProgress, Task<GeocodingResponse>>((req, p) => req.Get(p));

        public Task<GeocodingResponse> ReverseGeocode(LatLngPoint latLng, IProgress prog = null)
        {
            var yarrowReverseGeocodeRequest = new YarrowGeocodingRequest(yarrow);
            var gmapsReverseGeocodeRequest = new ReverseGeocodingRequest(gmaps);
            yarrowReverseGeocodeRequest.Location = gmapsReverseGeocodeRequest.Location = latLng;
            return Cascade(yarrowReverseGeocodeRequest, gmapsReverseGeocodeRequest, getGeocode, prog);
        }
    }
}