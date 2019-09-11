using System;

namespace Juniper.Imaging.Windows
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
                throw new NotSupportedException($"Component count {componentCount.ToString()}");
            }
        }

        public static HTTP.MediaType.Image ToMediaType(this System.Drawing.Imaging.ImageFormat format)
        {
            if (format == System.Drawing.Imaging.ImageFormat.Bmp)
            {
                return HTTP.MediaType.Image.Bmp;
            }
            else if (format == System.Drawing.Imaging.ImageFormat.Emf)
            {
                return HTTP.MediaType.Image.Emf;
            }
            else if (format == System.Drawing.Imaging.ImageFormat.Gif)
            {
                return HTTP.MediaType.Image.Gif;
            }
            else if (format == System.Drawing.Imaging.ImageFormat.Icon)
            {
                return HTTP.MediaType.Image.X_Icon;
            }
            else if (format == System.Drawing.Imaging.ImageFormat.Jpeg)
            {
                return HTTP.MediaType.Image.Jpeg;
            }
            else if (format == System.Drawing.Imaging.ImageFormat.Png)
            {
                return HTTP.MediaType.Image.Png;
            }
            else if (format == System.Drawing.Imaging.ImageFormat.Tiff)
            {
                return HTTP.MediaType.Image.Tiff;
            }
            else if (format == System.Drawing.Imaging.ImageFormat.Wmf)
            {
                return HTTP.MediaType.Image.Wmf;
            }
            else
            {
                throw new NotSupportedException($"Format {format}");
            }
        }

        public static System.Drawing.Imaging.ImageFormat ToGDIImageFormat(this HTTP.MediaType.Image format)
        {
            if (format == HTTP.MediaType.Image.Bmp)
            {
                return System.Drawing.Imaging.ImageFormat.Bmp;
            }
            else if (format == HTTP.MediaType.Image.Emf)
            {
                return System.Drawing.Imaging.ImageFormat.Emf;
            }
            else if (format == HTTP.MediaType.Image.Gif)
            {
                return System.Drawing.Imaging.ImageFormat.Gif;
            }
            else if (format == HTTP.MediaType.Image.X_Icon)
            {
                return System.Drawing.Imaging.ImageFormat.Icon;
            }
            else if (format == HTTP.MediaType.Image.Jpeg)
            {
                return System.Drawing.Imaging.ImageFormat.Jpeg;
            }
            else if (format == HTTP.MediaType.Image.Png)
            {
                return System.Drawing.Imaging.ImageFormat.Png;
            }
            else if (format == HTTP.MediaType.Image.Tiff)
            {
                return System.Drawing.Imaging.ImageFormat.Tiff;
            }
            else if (format == HTTP.MediaType.Image.Wmf)
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
                throw new NotSupportedException($"Pixel format {img.PixelFormat.GetStringValue()}");
            }
        }
    }
}
