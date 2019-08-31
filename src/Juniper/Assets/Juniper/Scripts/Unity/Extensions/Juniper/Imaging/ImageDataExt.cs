using Juniper.HTTP;

using UnityEngine;

namespace Juniper.Imaging
{
    public static class ImageDataExt
    {
        public static Texture2D CreateTexture(this ImageData image)
        {
            var texture = CreateTextureInternal(image);
            CopyToTextureInternal(image, texture);
            return texture;
        }

        public static void CopyToTexture(this ImageData image, ref Texture2D texture)
        {
            if (texture == null
                || texture.width != image.info.dimensions.width
                || texture.height != image.info.dimensions.height
                || texture.GetComponents() != image.info.components)
            {
                if (texture != null)
                {
                    Object.Destroy(texture);
                }

                texture = CreateTextureInternal(image);
            }

            CopyToTextureInternal(image, texture);
        }

        private static Texture2D CreateTextureInternal(ImageData image)
        {
            return new Texture2D(
                image.info.dimensions.width,
                image.info.dimensions.height,
                image.info.components == 4
                    ? TextureFormat.RGBA32
                    : TextureFormat.RGB24,
                false);
        }

        private static void CopyToTextureInternal(ImageData image, Texture2D texture)
        {
            if (image.contentType == MediaType.Image.Png
                || image.contentType == MediaType.Image.Jpeg
                || image.contentType == MediaType.Image.EXR
                || image.contentType == MediaType.Image.X_Tga)
            {
                texture.LoadImage(image.data);
            }
            else if (image.contentType == MediaType.Image.Raw)
            {
                texture.LoadRawTextureData(image.data);
            }
        }
    }
}
