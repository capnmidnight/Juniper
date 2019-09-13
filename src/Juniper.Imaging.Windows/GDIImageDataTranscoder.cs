using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;

using Juniper.Progress;

namespace Juniper.Imaging.Windows
{
    public class GDIImageDataTranscoder : IImageTranscoder<Image, ImageData>
    {
        public ImageData TranslateTo(Image image, IProgress prog)
        {
            using (var mem = new MemoryStream())
            {
                prog.Report(0);
                image.Save(mem, image.RawFormat);

                var img = new ImageData(
                    image.Width,
                    image.Height,
                    image.GetComponents(),
                    image.RawFormat.ToMediaType(),
                    mem.ToArray());
                prog.Report(1);
                return img;
            }
        }

        public Image TranslateFrom(ImageData image, IProgress prog)
        {
            var outImage = new Bitmap(image.info.dimensions.width, image.info.dimensions.height);
            if (image.contentType == HTTP.MediaType.Image.Raw)
            {
                var imageData = outImage.LockBits(
                    new Rectangle(0, 0, image.info.dimensions.width, image.info.dimensions.height),
                    System.Drawing.Imaging.ImageLockMode.WriteOnly,
                    image.info.components.ToGDIPixelFormat());

                Marshal.Copy(image.data, 0, imageData.Scan0, image.data.Length);

                outImage.UnlockBits(imageData);
            }
            else
            {
                using (var mem = new MemoryStream(image.data))
                using (var inImage = Image.FromStream(mem))
                using (var g = Graphics.FromImage(outImage))
                {
                    g.DrawImageUnscaled(inImage, 0, 0);
                    g.Flush();
                }
            }

            return outImage;
        }
    }
}