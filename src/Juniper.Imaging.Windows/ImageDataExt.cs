using System;
using System.IO;
using System.Runtime.InteropServices;
using Juniper.Serialization;

namespace Juniper.Imaging.Windows
{
    public static class ImageDataExt
    {
        public static System.Drawing.Image ToGDI(this ImageData value)
        {
            var outImage = new System.Drawing.Bitmap(value.dimensions.width, value.dimensions.height);
            if (value.contentType == HTTP.MediaType.Image.Raw)
            {
                var pixelFormat = value.components == 4
                    ? System.Drawing.Imaging.PixelFormat.Format32bppArgb
                    : System.Drawing.Imaging.PixelFormat.Format24bppRgb;

                var imageData = outImage.LockBits(
                    new System.Drawing.Rectangle(0, 0, value.dimensions.width, value.dimensions.height),
                    System.Drawing.Imaging.ImageLockMode.WriteOnly,
                    pixelFormat);

                Marshal.Copy(value.data, 0, imageData.Scan0, value.data.Length);

                outImage.UnlockBits(imageData);
            }
            else
            {
                using (var mem = new MemoryStream(value.data))
                using (var inImage = System.Drawing.Image.FromStream(mem))
                using (var g = System.Drawing.Graphics.FromImage(outImage))
                {
                    g.DrawImageUnscaled(inImage, 0, 0);
                    g.Flush();
                }
            }

            return outImage;
        }

        public static ImageData ToJuniper(this System.Drawing.Image image, DataSource source = DataSource.None)
        {
            int components;
            if (image.PixelFormat == System.Drawing.Imaging.PixelFormat.Format32bppArgb)
            {
                components = 4;
            }
            else if (image.PixelFormat == System.Drawing.Imaging.PixelFormat.Format24bppRgb)
            {
                components = 3;
            }
            else
            {
                throw new NotSupportedException($"Pixel format {image.PixelFormat}");
            }

            HTTP.MediaType.Image imageFormat;
            if (image.RawFormat == System.Drawing.Imaging.ImageFormat.Png)
            {
                imageFormat = HTTP.MediaType.Image.Png;
            }
            else if (image.RawFormat == System.Drawing.Imaging.ImageFormat.Jpeg)
            {
                imageFormat = HTTP.MediaType.Image.Jpeg;
            }
            else
            {
                throw new NotSupportedException($"Format {image.RawFormat}");
            }

            using (var mem = new MemoryStream())
            {
                image.Save(mem, image.RawFormat);

                return new ImageData(
                    source,
                    image.Width, image.Height,
                    components,
                    imageFormat,
                    mem.ToArray());
            }
        }
    }
}
