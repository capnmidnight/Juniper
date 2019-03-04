using Juniper.Data;

using System;
using System.Collections;

namespace Juniper.Imaging
{
    /// <summary>
    /// Load PNG images from disk or the 'net, and decode them in a background thread, so the
    /// rendering thread doesn't take a hit from the long process.
    /// </summary>
    public static class PNGImageLoader
    {
        /// <summary>
        /// Decodes a raw file buffer of PNG data into raw image buffer, with width and height saved.
        /// </summary>
        /// <param name="pngBytes">Png bytes.</param>
        /// <param name="ImageComplete">Image complete.</param>
        private static void DecodePNGTexture(byte[] pngBytes, OnRawImageComplete ImageComplete)
        {
            OneShotThread.Run(() =>
            {
#if NETFX_CORE
                using (var mem = new InMemoryRandomAccessStream())
                {
                    using (var writer = new DataWriter(mem))
                    {
                        writer.WriteBytes(pngBytes);
                        await writer.FlushAsync();
                        await writer.StoreAsync();
                        writer.DetachStream();
                    }

                    var decoder = await BitmapDecoder.CreateAsync(mem);
                    var frame = await decoder.GetFrameAsync(0);
                    var pixelData = await frame.GetPixelDataAsync(
                        BitmapPixelFormat.Bgra8,
                        BitmapAlphaMode.Straight,
                        new BitmapTransform(),
                        ExifOrientationMode.IgnoreExifOrientation,
                        ColorManagementMode.DoNotColorManage);

                    ImageComplete(new RawImage
                    {
                        width = (int)frame.PixelWidth,
                        height = (int)frame.PixelHeight,
                        data = pixelData.DetachPixelData()
                    });
                }
#else
                using (var mem = new System.IO.MemoryStream(pngBytes))
                {
                    var png = new Hjg.Pngcs.PngReader(mem);
                    png.SetUnpackedMode(true);
                    var rows = png.ReadRowsByte();
                    var scans = rows.ScanlinesB;
                    var data = new byte[rows.Nrows * rows.elementsPerRow];
                    for (var i = 0; i < rows.Nrows; ++i)
                    {
                        var row = rows.ScanlinesB[rows.Nrows - i - 1];
                        Array.Copy(row, 0, data, i * rows.elementsPerRow, row.Length);
                    }

                    ImageComplete(new RawImage
                    {
                        width = rows.elementsPerRow / rows.channels,
                        height = rows.Nrows,
                        data = data
                    });
                }
#endif
            });
        }

#if UNITY_5_3_OR_NEWER
        /// <summary>
        /// The texture format to use for decoding PNGs on the current system.
        /// </summary>
#if UNITY_WSA
        private const UnityEngine.TextureFormat DecodedPNGFormat = UnityEngine.TextureFormat.BGRA32;
#else
        private const UnityEngine.TextureFormat DecodedPNGFormat = UnityEngine.TextureFormat.RGBA32;
#endif

        /// <summary>
        /// Gets a texture from disk or the internet, running in a background thread, with a
        /// coroutine waiting on the thread to finish.
        /// </summary>
        /// <returns>The texture from streaming assets PNGC oroutine.</returns>
        /// <param name="imagePath">Image path.</param>
        /// <param name="resolve">Resolve.</param>
        /// <param name="reject">Reject.</param>
        public static IEnumerator StreamTexture(string imagePath, Action<UnityEngine.Texture> resolve, Action<Exception> reject)
        {
            RawImage? image = null;

            StreamingAssets.GetBytes(
                StreamingAssets.FormatPath(UnityEngine.Application.dataPath, imagePath),
                "image/png",
                bytes => DecodePNGTexture(bytes, img => image = img),
                reject);

            yield return new UnityEngine.WaitUntil(() => image != null);

            try
            {
                var texture = new UnityEngine.Texture2D(image.Value.width, image.Value.height, DecodedPNGFormat, false);
                texture.LoadRawTextureData(image.Value.data);
                texture.Apply(false, true);
                resolve(texture);
            }
            catch (Exception exp)
            {
                reject(exp);
            }
        }

#endif
    }
}