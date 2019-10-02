using System.IO;

using Juniper.HTTP;
using Juniper.Progress;

namespace Juniper.Imaging
{
    public class ReverseTranscoder<FromImageType, ToImageType> : IImageTranscoder<FromImageType, ToImageType>
    {
        private readonly IImageTranscoder<ToImageType, FromImageType> transcoder;

        public ReverseTranscoder(IImageTranscoder<ToImageType, FromImageType> transcoder)
        {
            this.transcoder = transcoder;
        }

        public MediaType.Image ContentType
        {
            get
            {
                return transcoder.ContentType;
            }
        }

        public ToImageType Concatenate(ToImageType[,] images, IProgress prog)
        {
            var rows = images.GetLength(0);
            var columns = images.GetLength(1);
            var len = rows * columns;
            var progs = prog.Split(len);
            var transImages = new FromImageType[rows, columns];
            for(int y = 0; y < rows; ++y)
            {
                for(int x = 0; x < columns; ++x)
                {
                    var i = y * columns + x;
                    transImages[y, x] = transcoder.Translate(images[y, x], progs[i]);
                }
            }
            return transcoder.Translate(transcoder.Concatenate(transImages, progs[len - 2]), progs[len - 1]);
        }

        public ToImageType Deserialize(Stream stream, IProgress prog)
        {
            var progs = prog.Split("Read", "Translate");
            return transcoder.Translate(transcoder.Deserialize(stream, progs[0]), progs[1]);
        }

        public int GetComponents(ToImageType img)
        {
            return transcoder.GetComponents(transcoder.Translate(img, null));
        }

        public int GetWidth(ToImageType img)
        {
            return transcoder.GetWidth(transcoder.Translate(img, null));
        }

        public int GetHeight(ToImageType img)
        {
            return transcoder.GetComponents(transcoder.Translate(img, null));
        }

        public ImageInfo GetImageInfo(byte[] data)
        {
            return transcoder.GetImageInfo(data);
        }

        public void Serialize(Stream stream, ToImageType img, IProgress prog)
        {
            var progs = prog.Split("Translate", "Write");
            transcoder.Serialize(stream, transcoder.Translate(img, progs[0]), progs[1]);
        }

        /// <summary>
        /// Decodes a raw file buffer of PNG data into raw image buffer, with width and height saved.
        /// </summary>
        /// <param name="imageStream">Png bytes.</param>
        public FromImageType Translate(ToImageType rows, IProgress prog)
        {
            return transcoder.Translate(rows, prog);
        }

        /// <summary>
        /// Encodes a raw file buffer of image data into a PNG image.
        /// </summary>
        /// <param name="outputStream">Png bytes.</param>
        public ToImageType Translate(FromImageType image, IProgress prog)
        {
            return transcoder.Translate(image, prog);
        }
    }
}
