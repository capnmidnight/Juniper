#if !NETSTANDARD
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;

using Juniper.Progress;

namespace Juniper.Imaging
{

    public class GDIImageDataTranscoder : IImageTranscoder<Image, ImageData>
    {
        public ImageData Translate(Image image, IProgress prog)
        {
            using (var mem = new MemoryStream())
            {
                prog.Report(0);
                image.Save(mem, image.RawFormat);

                var img = new ImageData(
                    image.Width,
                    image.Height,
                    image.GetComponents(),
                    mem.ToArray());
                prog.Report(1);
                return img;
            }
        }

        public Image Translate(ImageData image, IProgress prog)
        {
            var outImage = new Bitmap(image.info.dimensions.width, image.info.dimensions.height);
            var imageData = outImage.LockBits(
                 new Rectangle(0, 0, image.info.dimensions.width, image.info.dimensions.height),
                 System.Drawing.Imaging.ImageLockMode.WriteOnly,
                 image.info.components.ToGDIPixelFormat());

            Marshal.Copy(image.data, 0, imageData.Scan0, image.data.Length);

            outImage.UnlockBits(imageData);

            return outImage;
        }
    }
}
#endif