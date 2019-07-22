using System.Collections.Generic;

namespace Juniper.Google.Maps.MapTiles
{
    public partial class TileSearch
    {
        private static readonly Dictionary<TileImageFormat, ImageFormat> FORMAT_DESCRIPTIONS = new Dictionary<TileImageFormat, ImageFormat>
        {
            { TileImageFormat.PNG8, new ImageFormat( "image/png", "png", "png8") },
            { TileImageFormat.PNG32, new ImageFormat( "image/png", "png", "png32") },
            { TileImageFormat.GIF, new ImageFormat( "image/gif", "gif", "gif") },
            { TileImageFormat.JPEG, new ImageFormat( "image/jpeg", "jpeg", "jpg") },
            { TileImageFormat.JPEGBaseline, new ImageFormat( "image/jpeg", "jpeg", "jpg-baseline") }

        };

        private struct ImageFormat
        {
            public readonly string contentType;
            public readonly string fileExtension;
            public readonly string gmapsFieldValue;

            public ImageFormat(string v1, string v2, string v3)
            {
                contentType = v1;
                fileExtension = v2;
                gmapsFieldValue = v3;
            }

            public override bool Equals(object obj)
            {
                return obj != null
                    && obj is ImageFormat f
                    && contentType == f.contentType
                    && fileExtension == f.fileExtension
                    && gmapsFieldValue == f.gmapsFieldValue;
            }

            public static bool operator==(ImageFormat left, ImageFormat right)
            {
                return left.Equals(right);
            }

            public static bool operator!=(ImageFormat left, ImageFormat right)
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