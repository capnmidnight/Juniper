using System.Collections.Generic;
using Juniper.Image;

namespace Juniper.Google.Maps.MapTiles
{
    public partial class TileRequest
    {
        private static readonly Dictionary<TileImageFormat, ImageFormat> FORMAT_DESCRIPTIONS = new Dictionary<TileImageFormat, ImageFormat>
        {
            { TileImageFormat.PNG8, new ImageFormat( "image/png", "png", "png8", Image.ImageFormat.PNG) },
            { TileImageFormat.PNG32, new ImageFormat( "image/png", "png", "png32", Image.ImageFormat.PNG) },
            { TileImageFormat.GIF, new ImageFormat( "image/gif", "gif", "gif", Image.ImageFormat.Unsupported) },
            { TileImageFormat.JPEG, new ImageFormat( "image/jpeg", "jpeg", "jpg", Image.ImageFormat.JPEG) },
            { TileImageFormat.JPEGBaseline, new ImageFormat( "image/jpeg", "jpeg", "jpg-baseline", Image.ImageFormat.JPEG) }
        };

        private static readonly Dictionary<Image.ImageFormat, TileImageFormat> FORMAT_MAPPINGS = new Dictionary<Image.ImageFormat, TileImageFormat> {
            { Image.ImageFormat.JPEG, TileImageFormat.JPEG },
            { Image.ImageFormat.PNG, TileImageFormat.PNG8 }
        };

        private struct ImageFormat
        {
            public readonly string contentType;
            public readonly string fileExtension;
            public readonly string gmapsFieldValue;
            public readonly Image.ImageFormat format;

            internal ImageFormat(string contentType, string fileExtension, string gmapsFieldValue, Image.ImageFormat format)
            {
                this.contentType = contentType;
                this.fileExtension = fileExtension;
                this.gmapsFieldValue = gmapsFieldValue;
                this.format = format;
            }

            public override bool Equals(object obj)
            {
                return obj != null
                    && obj is ImageFormat f
                    && contentType == f.contentType
                    && fileExtension == f.fileExtension
                    && gmapsFieldValue == f.gmapsFieldValue;
            }

            public static bool operator ==(ImageFormat left, ImageFormat right)
            {
                return left.Equals(right);
            }

            public static bool operator !=(ImageFormat left, ImageFormat right)
            {
                return !(left == right);
            }

            public override int GetHashCode()
            {
                return contentType.GetHashCode()
                    ^ fileExtension.GetHashCode()
                    ^ gmapsFieldValue.GetHashCode();
            }
        }
    }
}