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
    {
        private readonly YarrowRequestConfiguration yarrow;
        private readonly YarrowMetadataRequest yarrowMetadataRequest;
        private readonly YarrowImageRequest<T> yarrowImageRequest;
        private readonly YarrowGeocodingRequest yarrowReverseGeocodeRequest;

        private readonly GoogleMapsRequestConfiguration gmaps;
        private readonly MetadataRequest gmapsMetadataRequest;
        private readonly ImageRequest<T> gmapsImageRequest;
        private readonly ReverseGeocodingRequest gmapsReverseGeocodeRequest;

        private bool useGoogleMaps = false;
        private Exception lastError;

        public YarrowClient(Uri yarrowServerUri, IImageDecoder<T> decoder, DirectoryInfo yarrowCacheDir)
        {
            yarrow = new YarrowRequestConfiguration(yarrowServerUri, yarrowCacheDir);
            yarrowMetadataRequest = new YarrowMetadataRequest(yarrow);
            yarrowImageRequest = new YarrowImageRequest<T>(yarrow, decoder);
            yarrowReverseGeocodeRequest = new YarrowGeocodingRequest(yarrow);
        }

        public YarrowClient(Uri yarrowServerUri, IImageDecoder<T> decoder, DirectoryInfo yarrowCacheDir, string gmapsApiKey, string gmapsSigningKey, DirectoryInfo gmapsCacheDir)
            : this(yarrowServerUri, decoder, yarrowCacheDir)
        {
            gmaps = new GoogleMapsRequestConfiguration(gmapsApiKey, gmapsSigningKey, gmapsCacheDir);
            gmapsMetadataRequest = new MetadataRequest(gmaps);
            gmapsImageRequest = new ImageRequest<T>(gmaps, decoder, new Size(640, 640));
            gmapsReverseGeocodeRequest = new ReverseGeocodingRequest(gmaps);

            useGoogleMaps = true;
        }

        public string Status
        {
            get
            {
                var errorMsg = lastError?.Message ?? "NONE";
                return $"Using google maps directly: {useGoogleMaps}. Last error: {errorMsg}";
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
                if (useGoogleMaps)
                {
                    return await getter(gmapsRequest, prog);
                }
                else
                {
                    try
                    {
                        return await getter(yarrowRequest, prog);
                    }
#pragma warning disable CA1031 // Do not catch general exception types
                    catch (Exception exp)
                    {
                        lastError = exp;
                        useGoogleMaps = true;
                        return await Cascade(yarrowRequest, gmapsRequest, getter, prog);
                    }
#pragma warning restore CA1031 // Do not catch general exception types
                }
            });
        }

        public Task<MetadataResponse> GetMetadata(PanoID pano, IProgress prog = null)
        {
            yarrowMetadataRequest.Pano = gmapsMetadataRequest.Pano = pano;
            return Cascade<YarrowMetadataRequest, MetadataRequest, IDeserializer<MetadataResponse>, MetadataResponse>
                (yarrowMetadataRequest, gmapsMetadataRequest, (req, p) => req.Get(p), prog);
        }

        public Task<MetadataResponse> GetMetadata(PlaceName placeName, IProgress prog = null)
        {
            yarrowMetadataRequest.Place = gmapsMetadataRequest.Place = placeName;
            return Cascade<YarrowMetadataRequest, MetadataRequest, IDeserializer<MetadataResponse>, MetadataResponse>
                (yarrowMetadataRequest, gmapsMetadataRequest, (req, p) => req.Get(p), prog);
        }

        public Task<MetadataResponse> GetMetadata(LatLngPoint latLng, IProgress prog = null)
        {
            yarrowMetadataRequest.Location = gmapsMetadataRequest.Location = latLng;
            return Cascade<YarrowMetadataRequest, MetadataRequest, IDeserializer<MetadataResponse>, MetadataResponse>
                (yarrowMetadataRequest, gmapsMetadataRequest, (req, p) => req.Get(p), prog);
        }

        public Task<T> GetImage(PanoID pano, IProgress prog = null)
        {
            yarrowImageRequest.Pano = gmapsImageRequest.Pano = pano;
            return Cascade<YarrowImageRequest<T>, ImageRequest<T>, IImageDecoder<T>, T>
                (yarrowImageRequest, gmapsImageRequest, (req, p) => req.GetImage(p), prog);
        }

        public Task<GeocodingResponse> ReverseGeocode(LatLngPoint latLng, IProgress prog = null)
        {
            yarrowReverseGeocodeRequest.Location = gmapsReverseGeocodeRequest.Location = latLng;
            return Cascade<YarrowGeocodingRequest, ReverseGeocodingRequest, IDeserializer<GeocodingResponse>, GeocodingResponse>
                (yarrowReverseGeocodeRequest, gmapsReverseGeocodeRequest, (req, p) => req.Get(p), prog);
        }
    }
}