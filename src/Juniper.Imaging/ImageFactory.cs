using System;
using System.IO;

using Juniper.Progress;
using Juniper.Serialization;

namespace Juniper.Imaging
{
    public class ImageFactory : AbstractImageDataDecoder
    {
        private IImageDecoder<ImageData> decoder;
        private ImageFormat format;

        public ImageFactory(ImageFormat format)
        {
            Format = format;
        }

        public ImageFormat Format
        {
            get { return format; }
            set
            {
                if (value == ImageFormat.PNG)
                {
                    format = value;
                    decoder = new PNG.PngDecoder();
                }
                else if (value == ImageFormat.JPEG)
                {
                    format = value;
                    decoder = new JPEG.JpegDecoder();
                }
                else
                {
                    throw new ArgumentException($"Image format `{format}` has not been implemented yet.");
                }
            }
        }
        public override ImageData Read(byte[] data, DataSource source = DataSource.None)
        {
            return decoder.Read(data, source);
        }

        public override ImageData Deserialize(Stream imageStream)
        {
            return decoder.Deserialize(imageStream);
        }

        public override void Serialize(Stream outputStream, ImageData image, IProgress prog = null)
        {
            decoder.Serialize(outputStream, image, prog);
        }
    }
}