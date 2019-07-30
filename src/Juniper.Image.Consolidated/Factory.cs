using System;
using System.IO;
using Juniper.Serialization;

namespace Juniper.Image.Consolidated
{
    public class Factory : IFactory<RawImage>
    {
        private IFactory<RawImage> factory;
        private ImageFormat format;

        public Factory(ImageFormat format)
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
                    factory = new PNG.Factory();
                }
                else if (value == ImageFormat.JPEG)
                {
                    format = value;
                    factory = new JPEG.Factory();
                }
                else
                {
                    throw new ArgumentException($"Image format `{format}` has not been implemented yet.");
                }
            }
        }

        public RawImage Deserialize(Stream imageStream)
        {
            return factory.Deserialize(imageStream);
        }

        public void Serialize(Stream outputStream, RawImage image)
        {
            factory.Serialize(outputStream, image);
        }
    }
}