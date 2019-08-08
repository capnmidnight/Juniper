
using System.Threading.Tasks;

using Juniper.Data;
using Juniper.Imaging;
using Juniper.Imaging.JPEG;
using Juniper.Imaging.PNG;

using UnityEngine;

namespace Juniper.Imaging
{
    /// <summary>
    /// Load PNG images from disk or the 'net, and decode them in a background thread, so the
    /// rendering thread doesn't take a hit from the long process.
    /// </summary>
    public static class ImageLoader
    {
#if UNITY_5_3_OR_NEWER
        /// <summary>
        /// The texture format to use for decoding PNGs on the current system.
        /// </summary>
#if UNITY_WSA
        private const TextureFormat DecodedImageFormat = TextureFormat.BGRA32;
#else
        private const TextureFormat DecodedImageFormat = TextureFormat.RGBA32;
#endif

        /// <summary>
        /// Gets a texture from disk or the Internet, running in a background thread, with a
        /// coroutine waiting on the thread to finish.
        /// </summary>
        /// <returns>The texture from streaming assets PNGC coroutine.</returns>
        /// <param name="imagePath">Image path.</param>
        /// <param name="resolve">  Resolve.</param>
        /// <param name="reject">   Reject.</param>
        public static async Task<ImageData> StreamPNG(string imagePath)
        {
            using (var imageFile = await StreamingAssets.GetStream(
                Application.temporaryCachePath,
                StreamingAssets.FormatPath(Application.streamingAssetsPath, Application.dataPath, imagePath),
                "image/png"))
            {
                var decoder = new PngFactory();
                return decoder.Deserialize(imageFile.Content);
            }
        }

        public static async Task<ImageData> StreamJPEG(string imagePath)
        {
            using (var imageFile = await StreamingAssets.GetStream(
                Application.temporaryCachePath,
                StreamingAssets.FormatPath(Application.streamingAssetsPath, Application.dataPath, imagePath),
                "image/jpeg"))
            {
                var decoder = new JpegFactory();
                return decoder.Deserialize(imageFile.Content);
            }
        }

        public static Texture2D ConstructTexture2D(ImageData image, TextureFormat format)
        {
            var texture = new Texture2D(image.dimensions.width, image.dimensions.height, format, false);
            texture.LoadRawTextureData(image.data);
            texture.Compress(false);
            texture.Apply(false, true);

            return texture;
        }

#endif
    }
}