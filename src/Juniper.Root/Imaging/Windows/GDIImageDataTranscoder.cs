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
            using var mem = new MemoryStream();
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

        public Image Translate(ImageData image, IProgress prog)
        {
            var outImage = new Bitmap(image.Info.Dimensions.Width, image.Info.Dimensions.Height);
            var imageData = outImage.LockBits(
                 new Rectangle(0, 0, image.Info.Dimensions.Width, image.Info.Dimensions.Height),
                 System.Drawing.Imaging.ImageLockMode.WriteOnly,
                 image.Info.Components.ToGDIPixelFormat());

            Marshal.Copy(image.Data, 0, imageData.Scan0, image.Data.Length);

            outImage.UnlockBits(imageData);

            return outImage;
        }
    }
}
#endif