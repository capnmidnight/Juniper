using System;
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
            if (image is null)
            {
                throw new ArgumentNullException(nameof(image));
            }

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
            if (image is null)
            {
                throw new ArgumentNullException(nameof(image));
            }

            var raw = image.GetData();
            var outImage = new Bitmap(image.Info.Dimensions.Width, image.Info.Dimensions.Height);
            var imageData = outImage.LockBits(
                 new Rectangle(0, 0, image.Info.Dimensions.Width, image.Info.Dimensions.Height),
                 System.Drawing.Imaging.ImageLockMode.WriteOnly,
                 image.Info.Components.ToGDIPixelFormat());

            Marshal.Copy(raw, 0, imageData.Scan0, raw.Length);

            outImage.UnlockBits(imageData);

            return outImage;
        }
    }
}