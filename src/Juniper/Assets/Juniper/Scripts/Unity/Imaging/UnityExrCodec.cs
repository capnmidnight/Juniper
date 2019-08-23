using System;

using Juniper.Serialization;

using UnityEngine;

namespace Juniper.Imaging
{
    public class UnityExrCodec : AbstractUnityTextureCodec
    {
        Texture2D.EXRFlags exrFlags;

        public UnityExrCodec(Texture2D.EXRFlags exrFlags = Texture2D.EXRFlags.None)
        {
            this.exrFlags = exrFlags;
        }

        public override ImageInfo GetImageInfo(byte[] data, DataSource source = DataSource.None)
        {
            throw new NotSupportedException("Don't know ho to read the raw image information from an EXR file.");
        }

        public override HTTP.MediaType.Image Format { get { return HTTP.MediaType.Image.EXR; } }

        protected override byte[] Encode(Texture2D value)
        {
            return value.EncodeToEXR(exrFlags);
        }
    }
}
