using Hjg.Pngcs;

using System.IO;

namespace Juniper.Imaging
{
    public class PngFactory : IImageFactory<ImageLines>
    {
        private readonly int compressionLevel;
        private readonly int IDATMaxSize;

        public PngFactory(int compressionLevel = 9, int IDATMaxSize = 0x1000)
        {
            this.compressionLevel = compressionLevel;
            this.IDATMaxSize = IDATMaxSize;
        }

        public MediaType.Image InputContentType => MediaType.Image_Png;

        public MediaType.Image OutputContentType => InputContentType;

        /// <summary>
        /// Decodes a raw file buffer of PNG data into raw image buffer, with width and height saved.
        /// </summary>
        /// <param name="stream">Png bytes.</param>
        public ImageLines Deserialize(Stream stream) =>
            Deserialize(stream, true);

        public ImageLines Dequeue(Stream stream) =>
            Deserialize(stream, false);

        private ImageLines Deserialize(Stream stream, bool closeWhenDone)
        {
            if (stream is null)
            {
                throw new System.ArgumentNullException(nameof(stream));
            }

            using var png = new PngReader(stream)
            {
                ShouldCloseStream = closeWhenDone
            };

            png.SetUnpackedMode(true);
            var image = png.ReadRowsByte();
            png.End();
            return image;
        }

        /// <summary>
        /// Encodes a raw file buffer of image data into a PNG image.
        /// </summary>
        /// <param name="stream">Png bytes.</param>
        public long Serialize(Stream stream, ImageLines value) =>
            Serialize(stream, value, true);

        public long Enqueue(Stream stream, ImageLines value) =>
            Serialize(stream, value, false);

        private long Serialize(Stream stream, ImageLines value, bool closeWhenDone)
        {
            if (stream is null)
            {
                throw new System.ArgumentNullException(nameof(stream));
            }

            if (value is null)
            {
                throw new System.ArgumentNullException(nameof(value));
            }

            var info = value.ImgInfo;

            using var png = new PngWriter(stream, info)
            {
                CompLevel = compressionLevel,
                IdatMaxSize = IDATMaxSize,
                ShouldCloseStream = closeWhenDone
            };

            png.SetFilterType(FilterType.FILTER_PAETH);

            var metadata = png.GetMetadata();
            metadata.SetDpi(100);

            for (var i = 0; i < value.Nrows; ++i)
            {
                png.WriteRow(value.GetImageLineAtMatrixRow(i), i);
            }

            var length = stream.Length;
            png.End();

            return length;
        }
    }
}