using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;

using Juniper.Progress;
using Juniper.Serialization;

namespace Juniper.Imaging.Windows
{
    public class GDIImageDataCodec : AbstractImageDataDecoder
    {
        private readonly GDICodec subCodec;

        public override HTTP.MediaType.Image Format { get { return subCodec.Format; } }

        public GDIImageDataCodec(HTTP.MediaType.Image format)
        {
            subCodec = new GDICodec(format);
        }

        public override ImageInfo GetImageInfo(byte[] data, DataSource source = DataSource.None)
        {
            return subCodec.GetImageInfo(data, source);
        }

        public override void Serialize(Stream stream, ImageData image, IProgress prog = null)
        {
            using (var outImage = new Bitmap(image.info.dimensions.width, image.info.dimensions.height))
            {
                if (image.info.contentType == HTTP.MediaType.Image.Raw)
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

                subCodec.Serialize(stream, outImage, prog);
            }
        }

        public override ImageData Deserialize(Stream stream)
        {
            var source = stream.DetermineSource();
            using (var image = subCodec.Deserialize(stream))
            {
                var components = image.PixelFormat.ToComponentCount();
                var imageFormat = image.RawFormat.ToMediaType();

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
}