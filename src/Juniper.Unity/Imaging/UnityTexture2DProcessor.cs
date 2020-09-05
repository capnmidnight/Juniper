using System;

using Juniper.Progress;

using UnityEngine;

namespace Juniper.Imaging
{
    public class UnityTexture2DProcessor : IImageProcessor<Texture2D>
    {
        public int GetWidth(Texture2D img)
        {
            if (img is null)
            {
                throw new ArgumentNullException(nameof(img));
            }

            return img.width;
        }

        public int GetHeight(Texture2D img)
        {
            if (img is null)
            {
                throw new ArgumentNullException(nameof(img));
            }

            return img.height;
        }

        public int GetComponents(Texture2D img)
        {
            if (img is null)
            {
                throw new ArgumentNullException(nameof(img));
            }

            if (img.format == TextureFormat.ARGB32
                || img.format == TextureFormat.RGBA32
                || img.format == TextureFormat.BGRA32)
            {
                return 4;
            }
            else if (img.format == TextureFormat.RGB24)
            {
                return 3;
            }
            else
            {
                throw new NotSupportedException($"Don't know how to handle pixel format {img.format}.");
            }
        }

        public Texture2D Concatenate(Texture2D[,] images, IProgress prog)
        {
            if (images is null)
            {
                throw new ArgumentNullException(nameof(images));
            }

            this.ValidateImages(images, prog,
                out var rows, out var columns, out _,
                out var tileWidth, out var tileHeight);

            var totalLen = rows * tileHeight * columns * tileWidth;

            var outputImage = new Texture2D(columns * tileWidth, rows * tileHeight);
            for (var r = 0; r < rows; ++r)
            {
                for (var c = 0; c < columns; ++c)
                {
                    var img = images[r, c];
                    if (img != null)
                    {
                        for (var y = 0; y < tileHeight; ++y)
                        {
                            var pixels = img.GetPixels(0, tileHeight - y - 1, tileWidth, 1);
                            outputImage.SetPixels(c * tileWidth, r * tileHeight + y, tileWidth, 1, pixels);
                            prog?.Report(tileWidth * (r * tileHeight * columns + c * tileHeight + y), totalLen);
                        }
                    }
                }
            }

            return outputImage;
        }
    }
}
