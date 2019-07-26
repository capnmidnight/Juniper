using System;
using System.Collections;

using Juniper.Image;
using Juniper.Data;

using UnityEngine;
using System.Threading.Tasks;
using Juniper.Unity.Coroutines;

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
        public static async Task<RawImage> StreamPNG(string imagePath, bool flipImage = true)
        {
            using (var imageFile = await StreamingAssets.GetStream(
                Application.temporaryCachePath,
                StreamingAssets.FormatPath(Application.streamingAssetsPath, Application.dataPath, imagePath),
                "image/png"))
            {
                var decoder = new Image.PNG.Factory();
                return await decoder.DecodeAsync(imageFile.Content, flipImage);
            }
        }

        public static async Task<RawImage> StreamJPEG(string imagePath, bool flipImage = true)
        {
            using (var imageFile = await StreamingAssets.GetStream(
                Application.temporaryCachePath,
                StreamingAssets.FormatPath(Application.streamingAssetsPath, Application.dataPath, imagePath),
                "image/jpeg"))
            {
                var decoder = new Image.JPEG.Factory();
                return await decoder.DecodeAsync(imageFile.Content, flipImage);
            }
        }

        public static IEnumerator ConstructTexture2D(Task<RawImage> imageTask, TextureFormat format, Action<Texture> resolve, Action<Exception> reject)
        {
            yield return new WaitForTask(imageTask);

            try
            {
                resolve(ConstructTexture2D(imageTask.Result, format));
            }
            catch (Exception exp)
            {
                reject(exp);
                throw;
            }
        }

        public static IEnumerator ConstructCubemap(Task<RawImage[]> imageTask, TextureFormat format, Action<Texture> resolve, Action<Exception> reject)
        {
            yield return new WaitForTask(imageTask);

            try
            {
                resolve(ConstructCubemap(imageTask.Result, format));
            }
            catch (Exception exp)
            {
                reject(exp);
                throw;
            }
        }

        public static Texture2D ConstructTexture2D(RawImage image, TextureFormat format)
        {
            var texture = new Texture2D(image.dimensions.width, image.dimensions.height, format, false);
            texture.LoadRawTextureData(image.data);
            texture.Apply(false, true);
            return texture;
        }

        private static void SetCubemapFace(this Cubemap texture, RawImage img, CubemapFace face)
        {
            texture.SetPixels(img.data.ToColors(img.components, true), face);
        }

        public static Cubemap ConstructCubemap(RawImage[] images, TextureFormat format)
        {
            var first = images[0];
            var texture = new Cubemap(first.dimensions.width, format, false);
            texture.SetCubemapFace(images[0], CubemapFace.PositiveZ);
            texture.SetCubemapFace(images[1], CubemapFace.PositiveX);
            texture.SetCubemapFace(images[2], CubemapFace.NegativeX);
            texture.SetCubemapFace(images[3], CubemapFace.NegativeZ);
            texture.SetCubemapFace(images[4], CubemapFace.PositiveY);
            texture.SetCubemapFace(images[5], CubemapFace.NegativeY);
            return texture;
        }

#endif
    }
}