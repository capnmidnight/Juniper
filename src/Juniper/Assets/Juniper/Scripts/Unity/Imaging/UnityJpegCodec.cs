using Juniper.Serialization;

using UnityEngine;

namespace Juniper.Imaging
{
    public class UnityJpegCodec : AbstractUnityTextureCodec
    {
        private readonly int encodingQuality;

        public UnityJpegCodec(int encodingQuality = 100)
        {
            this.encodingQuality = encodingQuality;
        }

        public override ImageInfo GetImageInfo(byte[] data, DataSource source = DataSource.None)
        {
            return ImageInfo.ReadJPEG(data, source);
        }
        public override HTTP.MediaType.Image Format { get { return HTTP.MediaType.Image.Jpeg; } }
        protected override byte[] Encode(Texture2D value)
        {
            return value.EncodeToJPG(encodingQuality);
        }
    }
}
