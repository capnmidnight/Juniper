using Juniper.Serialization;

using UnityEngine;

namespace Juniper.Imaging
{
    public class UnityPngCodec : AbstractUnityTextureCodec
    {
        public override ImageInfo GetImageInfo(byte[] data, DataSource source = DataSource.None)
        {
            return ImageInfo.ReadPNG(data, source);
        }

        public override HTTP.MediaType.Image Format { get { return HTTP.MediaType.Image.Png; } }

        protected override byte[] Encode(Texture2D value)
        {
            return value.EncodeToPNG();
        }
    }
}
