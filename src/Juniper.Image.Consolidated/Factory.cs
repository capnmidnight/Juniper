using System;
using System.IO;

namespace Juniper.Image.Consolidated
{
    public class Factory : IFactory
    {
        private readonly IFactory factory;

        public Factory(ImageFormat format)
        {
            if (format == ImageFormat.PNG)
            {
                factory = new PNG.Factory();
            }
            else if (format == ImageFormat.JPEG)
            {
                factory = new JPEG.Factory();
            }
            else
            {
                throw new ArgumentException($"Image format `{format}` has not been implemented yet.");
            }
        }

        public RawImage Decode(Stream imageStream, bool flipImage)
        {
            return factory.Decode(imageStream, flipImage);
        }

        public void Encode(RawImage image, Stream outputStream, bool flipImage)
        {
            factory.Encode(image, outputStream, flipImage);
        }
    }
}