using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Juniper.Imaging;

namespace Juniper.World.GIS.Google.MapTiles
{
    public class TileRequest : AbstractGoogleMapsRequest<MediaType.Image>
    {
        private readonly List<Marker> markers = new List<Marker>();
        private int scale;
        private string language;
        private string region;
        private MapImageType maptype;

        private int zoom;
        private string address;
        private LatLngPoint center;
        private Size size;

        public TileRequest(string apiKey, string signingKey, Size size)
            : base("staticmap", apiKey, signingKey, Juniper.MediaType.Image.Png)
        {
            Size = size;
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

        public string Address
        {
            get { return address; }
            set
            {
                address = value;
                center = default;
                SetQuery(nameof(center), address);
            }
        }

        public LatLngPoint Center
        {
            get { return center; }
            set
            {
                address = default;
                center = value;
                SetQuery(nameof(center), center.ToString());
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

        public LinePath Path { get; set; }

        protected override Uri BaseURI
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

                if (Path != null)
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
                  .Append(point.center);
                delim = "|";
            }

            AddQuery(nameof(markers), sb);
        }
    }
}