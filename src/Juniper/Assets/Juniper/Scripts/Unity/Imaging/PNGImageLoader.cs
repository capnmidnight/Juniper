using System;
using System.Collections;

using Juniper.Image;
using Juniper.Data;

using UnityEngine;

namespace Juniper.Imaging
{
    /// <summary>
    /// Load PNG images from disk or the 'net, and decode them in a background thread, so the
    /// rendering thread doesn't take a hit from the long process.
    /// </summary>
    public static class PNGImageLoader
    {
        

#if UNITY_5_3_OR_NEWER
        /// <summary>
        /// The texture format to use for decoding PNGs on the current system.
        /// </summary>
#if UNITY_WSA
        private const TextureFormat DecodedPNGFormat = TextureFormat.BGRA32;
#else
        private const TextureFormat DecodedPNGFormat = TextureFormat.RGBA32;
#endif

        /// <summary>
        /// Gets a texture from disk or the Internet, running in a background thread, with a
        /// coroutine waiting on the thread to finish.
        /// </summary>
        /// <returns>The texture from streaming assets PNGC coroutine.</returns>
        /// <param name="imagePath">Image path.</param>
        /// <param name="resolve">  Resolve.</param>
        /// <param name="reject">   Reject.</param>
        public static IEnumerator StreamTexture(string imagePath, Action<Texture> resolve, Action<Exception> reject)
        {
            var resultTask = StreamingAssets.GetStream(
                Application.temporaryCachePath,
                StreamingAssets.FormatPath(Application.streamingAssetsPath, Application.dataPath, imagePath),
                "image/png");

            yield return new WaitUntil(() => resultTask.IsCompleted || resultTask.IsFaulted || resultTask.IsCanceled);

            using (resultTask.Result.Content)
            {
                var imageTask = Decoder.DecodePNGAsync(resultTask.Result.Content);

                yield return new WaitUntil(() => imageTask.IsCompleted || imageTask.IsFaulted || imageTask.IsCanceled);

                try
                {
                    var texture = new Texture2D(imageTask.Result.width, imageTask.Result.height, DecodedPNGFormat, false);
                    texture.LoadRawTextureData(imageTask.Result.data);
                    texture.Apply(false, true);
                    resolve(texture);
                }
                catch (Exception exp)
                {
                    reject(exp);
                }
            }
        }

#endif
    }
}
