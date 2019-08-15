using UnityEngine;

namespace Juniper.Imaging
{
    public static class ImageDataExt
    {
        public static Texture ToTexture(this ImageData image)
        {
            var texture = new Texture2D(image.dimensions.width, image.dimensions.height, TextureFormat.RGB24, false);
            if (image.format == ImageFormat.None)
            {
                texture.LoadRawTextureData(image.data);
            }
            else if (image.format != ImageFormat.Unsupported)
            {
                texture.LoadImage(image.data);
            }
            texture.Compress(true);
            texture.Apply(false, true);
            return texture;
        }
    }
}