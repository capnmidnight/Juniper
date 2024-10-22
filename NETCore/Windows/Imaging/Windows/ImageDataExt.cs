using System.Globalization;

namespace Juniper.Imaging;

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
            return MediaType.Image_Bmp;
        }
        else if (format == System.Drawing.Imaging.ImageFormat.Emf)
        {
            return MediaType.Image_Emf;
        }
        else if (format == System.Drawing.Imaging.ImageFormat.Gif)
        {
            return MediaType.Image_Gif;
        }
        else if (format == System.Drawing.Imaging.ImageFormat.Icon)
        {
            return MediaType.Image_X_Icon;
        }
        else if (format == System.Drawing.Imaging.ImageFormat.Jpeg)
        {
            return MediaType.Image_Jpeg;
        }
        else if (format == System.Drawing.Imaging.ImageFormat.Png)
        {
            return MediaType.Image_Png;
        }
        else if (format == System.Drawing.Imaging.ImageFormat.Tiff)
        {
            return MediaType.Image_Tiff;
        }
        else if (format == System.Drawing.Imaging.ImageFormat.Wmf)
        {
            return MediaType.Image_Wmf;
        }
        else
        {
            throw new NotSupportedException($"Format {format}");
        }
    }

    public static System.Drawing.Imaging.ImageFormat ToGDIImageFormat(this MediaType.Image format)
    {
        if (format == MediaType.Image_Bmp)
        {
            return System.Drawing.Imaging.ImageFormat.Bmp;
        }
        else if (format == MediaType.Image_Emf)
        {
            return System.Drawing.Imaging.ImageFormat.Emf;
        }
        else if (format == MediaType.Image_Gif)
        {
            return System.Drawing.Imaging.ImageFormat.Gif;
        }
        else if (format == MediaType.Image_X_Icon)
        {
            return System.Drawing.Imaging.ImageFormat.Icon;
        }
        else if (format == MediaType.Image_Jpeg)
        {
            return System.Drawing.Imaging.ImageFormat.Jpeg;
        }
        else if (format == MediaType.Image_Png)
        {
            return System.Drawing.Imaging.ImageFormat.Png;
        }
        else if (format == MediaType.Image_Tiff)
        {
            return System.Drawing.Imaging.ImageFormat.Tiff;
        }
        else if (format == MediaType.Image_Wmf)
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
            throw new NotSupportedException($"Pixel format {img.PixelFormat}");
        }
    }
}