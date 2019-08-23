using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Juniper.Imaging;
using Juniper.Imaging.HjgPngcs;
using Juniper.World.GIS;

namespace Juniper.Google.Maps.MapTiles
{
    public class TileRequest : AbstractGoogleMapsRequest<IImageDecoder<ImageData>, ImageData>
    {
        private readonly List<Marker> markers = new List<Marker>();
        private LinePath path;
        private int scale;
        private string language;
        private string region;
        private MapImageType maptype;

        private int zoom;
        private PlaceName address;
        private LatLngPoint center;
        private Size size;

        private TileRequest(GoogleMapsRequestConfiguration api, IImageDecoder<ImageData> decoder, Size size)
            : base(api, decoder, "staticmap", "tiles", true)
        {
            Size = size;
            SetContentType(decoder.Format);
        }

        public TileRequest(GoogleMapsRequestConfiguration api, Size size)
            : this(api, new HjgPngcsImageDataCodec(), size)
        {
        }

        public Size Size
        {
            get { return size; }
            set
            {
                size = value;
                SetQuery(nameof(size), size);
            }
        }

        public int Zoom
        {
            get { return zoom; }
            set
            {
                zoom = value;
                SetQuery(nameof(zoom), zoom);
            }
        }

        public PlaceName Address
        {
            get { return address; }
            set
            {
                address = value;
                center = default;
                SetQuery(nameof(center), (string)address);
            }
        }

        public LatLngPoint Center
        {
            get { return center; }
            set
            {
                address = default;
                center = value;
                SetQuery(nameof(center), (string)center);
            }
        }

        public int Scale
        {
            get { return scale; }
            set
            {
                scale = value;
                SetQuery(nameof(scale), scale);
            }
        }

        public string Language
        {
            get { return language; }
            set
            {
                language = value;
                SetQuery(nameof(language), language);
            }
        }

        public string Region
        {
            get { return region; }
            set
            {
                region = value;
                SetQuery(nameof(region), region);
            }
        }

        public MapImageType MapType
        {
            get { return maptype; }
            set
            {
                maptype = value;
                SetQuery(nameof(maptype), maptype);
            }
        }

        public void AddMarker(Marker marker)
        {
            if (marker != default)
            {
                markers.Add(marker);
            }
        }

        public LinePath Path
        {
            get { return path; }
            set { path = value; }
        }

        public override Uri BaseURI
        {
            get
            {
                if (markers.Count > 0)
                {
                    RemoveQuery(nameof(markers));
                    var groupedByStyle = markers.GroupBy(m => m.style);
                    foreach (var group in groupedByStyle)
                    {
                        AddMarkerGroup(group.Key, group);
                    }
                }

                if (path != null)
                {
                    SetQuery(nameof(path), path);
                }

                return base.BaseURI;
            }
        }

        private void AddMarkerGroup(MarkerStyle style, IEnumerable<Marker> group)
        {
            var sb = new StringBuilder();
            var delim = "";
            if (style != default)
            {
                sb.Append(style);
                delim = "|";
            }

            foreach (var point in group)
            {
                sb.Append($"{delim}{point.center}");
                delim = "|";
            }

            AddQuery(nameof(markers), sb);
        }
    }
}