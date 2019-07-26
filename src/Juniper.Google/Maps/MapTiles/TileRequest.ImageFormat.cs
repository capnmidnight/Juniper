using System.Collections.Generic;

using Juniper.Image;

namespace Juniper.Google.Maps.MapTiles
{
    public partial class TileRequest
    {
        private static readonly Dictionary<TileImageFormat, ImageFormatDescription> FORMAT_DESCRIPTIONS = new Dictionary<TileImageFormat, ImageFormatDescription>
        {
            { TileImageFormat.PNG8, new ImageFormatDescription( "png8", ImageFormat.PNG) },
            { TileImageFormat.PNG32, new ImageFormatDescription( "png32", ImageFormat.PNG) },
            { TileImageFormat.GIF, new ImageFormatDescription( "gif", ImageFormat.Unsupported) },
            { TileImageFormat.JPEG, new ImageFormatDescription( "jpg", ImageFormat.JPEG) },
            { TileImageFormat.JPEGBaseline, new ImageFormatDescription( "jpg-baseline", ImageFormat.JPEG) }
        };

        private static readonly Dictionary<ImageFormat, TileImageFormat> FORMAT_MAPPINGS = new Dictionary<ImageFormat, TileImageFormat> {
            { ImageFormat.JPEG, TileImageFormat.JPEG },
            { ImageFormat.PNG, TileImageFormat.PNG8 }
        };

        private struct ImageFormatDescription
        {
            public readonly string gmapsFieldValue;
            public readonly ImageFormat format;

            internal ImageFormatDescription(string gmapsFieldValue, ImageFormat format)
            {
                this.gmapsFieldValue = gmapsFieldValue;
                this.format = format;
            }

            public override bool Equals(object obj)
            {
                return obj != null
                    && obj is ImageFormatDescription f
                    && gmapsFieldValue == f.gmapsFieldValue;
            }

            public static bool operator ==(ImageFormatDescription left, ImageFormatDescription right)
            {
                return left.Equals(right);
            }

            public static bool operator !=(ImageFormatDescription left, ImageFormatDescription right)
            {
                return !(left == right);
            }

            public override int GetHashCode()
            {
                return gmapsFieldValue.GetHashCode()
                    ^ format.GetHashCode();
            }
        }
    }
}