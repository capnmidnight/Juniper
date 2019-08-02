using System;
using System.IO;
using Juniper.Progress;
using Juniper.Serialization;

namespace Juniper.Image
{
    public class Factory : IFactory<ImageData>
    {
        public static ImageData Read(ImageFormat format, byte[] data, DataSource source = DataSource.None)
        {
            if (format == ImageFormat.JPEG)
            {
                return JPEG.JpegFactory.Read(data, source);
            }
            else if (format == ImageFormat.PNG)
            {
                return PNG.PngFactory.Read(data, source);
            }
            else
            {
                throw new ArgumentException($"Can't figure out how to read image format {format} from the provided data.");
            }
        }

        public static ImageData Read(ImageFormat format, Stream stream)
        {
            var source = stream.DetermineSource();
            using (var mem = new MemoryStream())
            {
                stream.CopyTo(mem);
                mem.Flush();
                return Read(format, mem.ToArray(), source);
            }
        }

        public static ImageData Read(ImageFormat format, string fileName)
        {
            return Read(format, File.ReadAllBytes(fileName), DataSource.File);
        }

        public static ImageData Read(ImageFormat format, FileInfo file)
        {
            return Read(format, file.FullName);
        }

        private IFactory<ImageData> factory;
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
                    factory = new PNG.PngFactory();
                }
                else if (value == ImageFormat.JPEG)
                {
                    format = value;
                    factory = new JPEG.JpegFactory();
                }
                else
                {
                    throw new ArgumentException($"Image format `{format}` has not been implemented yet.");
                }
            }
        }

        public ImageData Deserialize(Stream imageStream)
        {
            return factory.Deserialize(imageStream);
        }

        public void Serialize(Stream outputStream, ImageData image, IProgress prog = null)
        {
            factory.Serialize(outputStream, image, prog);
        }
    }
}