using System;
using System.Linq;
using Juniper.HTTP.REST;
using Juniper.Image;
using Juniper.World;
using Juniper.World.GIS;

namespace Juniper.Google.Maps.StreetView
{
    public class CubeMapRequest : AbstractMultiRequest<RawImage, ImageRequest>
    {
        private CubeMapRequest(Func<ImageRequest> factory)
            : base(6, factory)
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

        public CubeMapRequest(PanoID pano, Size size)
            : this(() => new ImageRequest(pano, size)) { }

        public CubeMapRequest(PanoID pano, int width, int height)
            : this(() => new ImageRequest(pano, new Size(width, height))) { }

        public CubeMapRequest(PlaceName placeName, Size size)
            : this(() => new ImageRequest(placeName, size)) { }

        public CubeMapRequest(PlaceName placeName, int width, int height)
            : this(() => new ImageRequest(placeName, new Size(width, height))) { }

        public CubeMapRequest(LatLngPoint location, Size size)
            : this(() => new ImageRequest(location, size)) { }

        public CubeMapRequest(LatLngPoint location, int width, int height)
            : this(() => new ImageRequest(location, new Size(width, height))) { }

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

        public bool FlipImages
        {
            get
            {
                return subRequests.Any(request => request.FlipImage);
            }
            set
            {
                foreach (var request in subRequests)
                {
                    request.FlipImage = value;
                }
            }
        }
    }
}