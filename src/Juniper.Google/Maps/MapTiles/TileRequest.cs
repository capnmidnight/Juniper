using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Juniper.Imaging;
using Juniper.World.GIS;

namespace Juniper.Google.Maps.MapTiles
{
    public partial class TileRequest : AbstractGoogleMapsRequest<ImageData>
    {
        public static TileRequest Create(GoogleMapsRequestConfiguration api, LocationTypes locationType, object value, int zoom, Size size, ImageFormat format = ImageFormat.PNG)
        {
            switch (locationType)
            {
                case LocationTypes.LatLngPoint: return new TileRequest(api, (LatLngPoint)value, zoom, size, format);
                case LocationTypes.PlaceName: return new TileRequest(api, (PlaceName)value, zoom, size, format);
                default: return default;
            }
        }

        public static TileRequest Create(GoogleMapsRequestConfiguration api, LocationTypes locationType, object value, int zoom, int width, int height, ImageFormat format = ImageFormat.PNG)
        {
            return Create(api, locationType, value, zoom, new Size(width, height), format);
        }

        private readonly List<Marker> markers = new List<Marker>();
        private LinePath path;
        private ImageFormat format;
        private int scale;
        private string language;
        private string region;
        private MapImageType maptype;

        private TileRequest(GoogleMapsRequestConfiguration api, string center, int zoom, Size size, ImageFormat format)
            : base(api, new ImageFactory(format), "staticmap", "tiles", true)
        {
            SetQuery(nameof(center), center);
            SetQuery(nameof(zoom), zoom);
            SetQuery(nameof(size), size);
            Format = format;
        }

        public TileRequest(GoogleMapsRequestConfiguration api, PlaceName address, int zoom, Size size, ImageFormat format)
            : this(api, (string)address, zoom, size, format) { }

        public TileRequest(GoogleMapsRequestConfiguration api, PlaceName address, int zoom, int width, int height, ImageFormat format)
            : this(api, (string)address, zoom, new Size(width, height), format) { }

        public TileRequest(GoogleMapsRequestConfiguration api, LatLngPoint center, int zoom, Size size, ImageFormat format)
            : this(api, center.ToString(), zoom, size, format) { }

        public TileRequest(GoogleMapsRequestConfiguration api, LatLngPoint center, int zoom, int width, int height, ImageFormat format)
            : this(api, center.ToString(), zoom, new Size(width, height), format) { }

        public bool FlipImage { get; set; }

        public ImageFormat Format
        {
            get { return format; }
            set
            {
                var mapping = FORMAT_MAPPINGS.Get(value, default);
                var description = FORMAT_DESCRIPTIONS.Get(mapping, default);
                if (mapping == TileImageFormat.Unsupported
                    || description == default)
                {
                    throw new ArgumentException($"{value} is not supported yet.");
                }

                format = value;
                deserializer = new ImageFactory(format);
                SetContentType(ImageData.GetContentType(format), ImageData.GetContentType(format));

                if (mapping != TileImageFormat.PNG8)
                {
                    SetQuery(nameof(format), description.gmapsFieldValue);
                }
            }
        }

        public int Scale
        {
            get { return scale; }
            set { scale = SetQuery(nameof(scale), value); }
        }

        public string Language
        {
            get { return language; }
            set { language = SetQuery(nameof(language), value); }
        }

        public string Region
        {
            get { return region; }
            set { region = SetQuery(nameof(region), value); }
        }

        public MapImageType MapType
        {
            get { return maptype; }
            set { maptype = SetQuery(nameof(maptype), value); }
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