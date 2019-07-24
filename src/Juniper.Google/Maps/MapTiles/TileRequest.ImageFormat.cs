using System.Collections.Generic;

namespace Juniper.Google.Maps.MapTiles
{
    public partial class TileRequest
    {
        private static readonly Dictionary<TileImageFormat, ImageFormat> FORMAT_DESCRIPTIONS = new Dictionary<TileImageFormat, ImageFormat>
        {
            { TileImageFormat.PNG8, new ImageFormat( "image/png", "png", "png8", Image.Decoder.SupportedFormats.PNG) },
            { TileImageFormat.PNG32, new ImageFormat( "image/png", "png", "png32", Image.Decoder.SupportedFormats.PNG) },
            { TileImageFormat.GIF, new ImageFormat( "image/gif", "gif", "gif", Image.Decoder.SupportedFormats.Unsupported) },
            { TileImageFormat.JPEG, new ImageFormat( "image/jpeg", "jpeg", "jpg", Image.Decoder.SupportedFormats.JPEG) },
            { TileImageFormat.JPEGBaseline, new ImageFormat( "image/jpeg", "jpeg", "jpg-baseline", Image.Decoder.SupportedFormats.JPEG) }
        };

        private static readonly Dictionary<Image.Decoder.SupportedFormats, TileImageFormat> FORMAT_MAPPINGS = new Dictionary<Image.Decoder.SupportedFormats, TileImageFormat> {
            { Image.Decoder.SupportedFormats.JPEG, TileImageFormat.JPEG },
            { Image.Decoder.SupportedFormats.PNG, TileImageFormat.PNG8 }
        };

        private struct ImageFormat
        {
            public readonly string contentType;
            public readonly string fileExtension;
            public readonly string gmapsFieldValue;
            public readonly Image.Decoder.SupportedFormats format;

            internal ImageFormat(string contentType, string fileExtension, string gmapsFieldValue, Image.Decoder.SupportedFormats format)
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