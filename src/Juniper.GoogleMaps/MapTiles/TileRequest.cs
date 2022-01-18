using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Juniper.World.GIS.Google.MapTiles
{
    public class TileRequest : AbstractGoogleMapsRequest<MediaType.Image>
    {
        private readonly List<Marker> markers = new();
        private int scale;
        private string language;
        private string region;
        private MapImageType maptype;

        private int zoom;
        private string address;
        private LatLngPoint center;
        private Size size;

        public TileRequest(string apiKey, string signingKey, Size size)
            : base("staticmap", Juniper.MediaType.Image.Png, apiKey, signingKey)
        {
            Size = size;
        }

        public Size Size
        {
            get => size;
            set
            {
                size = value;
                SetQuery(nameof(size), size);
            }
        }

        public int Zoom
        {
            get => zoom;
            set
            {
                zoom = value;
                SetQuery(nameof(zoom), zoom);
            }
        }

        public string Address
        {
            get => address;
            set
            {
                address = value;
                center = default;
                SetQuery(nameof(center), address);
            }
        }

        public LatLngPoint Center
        {
            get => center;
            set
            {
                address = default;
                center = value;
                SetQuery(nameof(center), center?.ToString(CultureInfo.InvariantCulture));
            }
        }

        public int Scale
        {
            get => scale;
            set
            {
                scale = value;
                SetQuery(nameof(scale), scale);
            }
        }

        public string Language
        {
            get => language;
            set
            {
                language = value;
                SetQuery(nameof(language), language);
            }
        }

        public string Region
        {
            get => region;
            set
            {
                region = value;
                SetQuery(nameof(region), region);
            }
        }

        public MapImageType MapType
        {
            get => maptype;
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

        public LinePathCollection Path { get; /* set; */ }

        protected override Uri BaseURI
        {
            get
            {
                if (markers.Count > 0)
                {
                    RemoveQuery(nameof(markers));
                    var groupedByStyle = markers.GroupBy(m => m.Style);
                    foreach (var group in groupedByStyle)
                    {
                        AddMarkerGroup(group.Key, group);
                    }
                }

                if (Path is not null)
                {
                    SetQuery(nameof(Path), Path);
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
                sb.Append(delim)
                  .Append(point.Center);
                delim = "|";
            }

            AddQuery(nameof(markers), sb);
        }
    }
}