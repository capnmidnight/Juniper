#if !NETSTANDARD
using System;
using System.Globalization;

namespace Juniper.Imaging
{
    public static class ImageDataExt
    {
        public static System.Drawing.Imaging.PixelFormat ToGDIPixelFormat(this int componentCount)
        {
            if (componentCount == 4)
            {
                return System.Drawing.Imaging.PixelFormat.Format32bppArgb;
            }
            else if (componentCount == 3)
            {
                return System.Drawing.Imaging.PixelFormat.Format24bppRgb;
            }
            else
            {
                throw new NotSupportedException($"Component count {componentCount.ToString(CultureInfo.InvariantCulture)}");
            }
        }

        public static MediaType.Image ToMediaType(this System.Drawing.Imaging.ImageFormat format)
        {
            if (format == System.Drawing.Imaging.ImageFormat.Bmp)
            {
                return MediaType.Image.Bmp;
            }
            else if (format == System.Drawing.Imaging.ImageFormat.Emf)
            {
                return MediaType.Image.Emf;
            }
            else if (format == System.Drawing.Imaging.ImageFormat.Gif)
            {
                return MediaType.Image.Gif;
            }
            else if (format == System.Drawing.Imaging.ImageFormat.Icon)
            {
                return MediaType.Image.X_Icon;
            }
            else if (format == System.Drawing.Imaging.ImageFormat.Jpeg)
            {
                return MediaType.Image.Jpeg;
            }
            else if (format == System.Drawing.Imaging.ImageFormat.Png)
            {
                return MediaType.Image.Png;
            }
            else if (format == System.Drawing.Imaging.ImageFormat.Tiff)
            {
                return MediaType.Image.Tiff;
            }
            else if (format == System.Drawing.Imaging.ImageFormat.Wmf)
            {
                return MediaType.Image.Wmf;
            }
            else
            {
                throw new NotSupportedException($"Format {format}");
            }
        }

        public static System.Drawing.Imaging.ImageFormat ToGDIImageFormat(this MediaType.Image format)
        {
            if (format == MediaType.Image.Bmp)
            {
                return System.Drawing.Imaging.ImageFormat.Bmp;
            }
            else if (format == MediaType.Image.Emf)
            {
                return System.Drawing.Imaging.ImageFormat.Emf;
            }
            else if (format == MediaType.Image.Gif)
            {
                return System.Drawing.Imaging.ImageFormat.Gif;
            }
            else if (format == MediaType.Image.X_Icon)
            {
                return System.Drawing.Imaging.ImageFormat.Icon;
            }
            else if (format == MediaType.Image.Jpeg)
            {
                return System.Drawing.Imaging.ImageFormat.Jpeg;
            }
            else if (format == MediaType.Image.Png)
            {
                return System.Drawing.Imaging.ImageFormat.Png;
            }
            else if (format == MediaType.Image.Tiff)
            {
                return System.Drawing.Imaging.ImageFormat.Tiff;
            }
            else if (format == MediaType.Image.Wmf)
            {
                return System.Drawing.Imaging.ImageFormat.Wmf;
            }
            else
            {
                throw new NotSupportedException($"Format {format}");
            }
        }

        public static int GetComponents(this System.Drawing.Image img)
        {
            if (img is null)
            {
                throw new ArgumentNullException(nameof(img));
            }

            if (img.PixelFormat == System.Drawing.Imaging.PixelFormat.Format32bppArgb)
            {
                return 4;
            }
            else if (img.PixelFormat == System.Drawing.Imaging.PixelFormat.Format24bppRgb)
            {
                return 3;
            }
            else
            {
                throw new NotSupportedException($"Pixel format {img.PixelFormat.ToString()}");
            }
        }
    }
}
#endif