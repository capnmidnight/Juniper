using System;

using Juniper.HTTP.REST;
using Juniper.Imaging;
using Juniper.World;
using Juniper.World.GIS;

namespace Juniper.Google.Maps.StreetView
{
    public class CubeMapRequest<T> : AbstractMultiRequest<T, ImageRequest<T>>
    {
        private CubeMapRequest(GoogleMapsRequestConfiguration api, Func<ImageRequest<T>> factory)
            : base(api, 6, factory)
        {
            subRequests[0].Heading = Heading.North;
            subRequests[0].Pitch = Pitch.Level;

            subRequests[1].Heading = Heading.East;
            subRequests[1].Pitch = Pitch.Level;

            subRequests[2].Heading = Heading.West;
            subRequests[2].Pitch = Pitch.Level;

            subRequests[3].Heading = Heading.South;
            subRequests[3].Pitch = Pitch.Level;

            subRequests[4].Heading = Heading.North;
            subRequests[4].Pitch = Pitch.Up;

            subRequests[5].Heading = Heading.North;
            subRequests[5].Pitch = Pitch.Down;
        }

        public CubeMapRequest(GoogleMapsRequestConfiguration api, IImageDecoder<T> decoder, Size size)
            : this(api, () => new ImageRequest<T>(api, decoder, size)) { }

        public CubeMapRequest(GoogleMapsRequestConfiguration api, IImageDecoder<T> decoder, Size size, PanoID pano)
            : this(api, () => new ImageRequest<T>(api, decoder, size, pano)) { }

        public CubeMapRequest(GoogleMapsRequestConfiguration api, IImageDecoder<T> decoder, Size size, PlaceName placeName)
            : this(api, () => new ImageRequest<T>(api, decoder, size, placeName)) { }

        public CubeMapRequest(GoogleMapsRequestConfiguration api, IImageDecoder<T> decoder, Size size, LatLngPoint location)
            : this(api, () => new ImageRequest<T>(api, decoder, size, location)) { }

        public Size Size
        {
            get { return subRequests[0].Size; }
            set
            {
                foreach (var request in subRequests)
                {
                    request.Size = value;
                }
            }
        }

        public void SetSize(int width, int height)
        {
            Size = new Size(width, height);
        }

        public PanoID Pano
        {
            get { return subRequests[0].Pano; }
            set
            {
                foreach (var request in subRequests)
                {
                    request.Pano = value;
                }
            }
        }

        public PlaceName Place
        {
            get { return subRequests[0].Place; }
            set
            {
                foreach (var request in subRequests)
                {
                    request.Place = value;
                }
            }
        }

        public LatLngPoint Location
        {
            get { return subRequests[0].Location; }
            set
            {
                foreach (var request in subRequests)
                {
                    request.Location = value;
                }
            }
        }

        public int Radius
        {
            get { return subRequests[0].Radius; }
            set
            {
                foreach (var request in subRequests)
                {
                    request.Radius = value;
                }
            }
        }

        public bool OutdoorOnly
        {
            get { return subRequests[0].OutdoorOnly; }
            set
            {
                foreach (var request in subRequests)
                {
                    request.OutdoorOnly = value;
                }
            }
        }
    }
}