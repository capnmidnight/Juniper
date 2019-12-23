using System.IO;

using Hjg.Pngcs;

using Juniper.Progress;

namespace Juniper.Imaging
{
    public class HjgPngcsCodec : IImageCodec<ImageLines>
    {
        private readonly int compressionLevel;
        private readonly int IDATMaxSize;

        public HjgPngcsCodec(int compressionLevel = 9, int IDATMaxSize = 0x1000)
        {
            this.compressionLevel = compressionLevel;
            this.IDATMaxSize = IDATMaxSize;
        }

        public MediaType.Image ContentType
        {
            get { return MediaType.Image.Png; }
        }

        /// <summary>
        /// Decodes a raw file buffer of PNG data into raw image buffer, with width and height saved.
        /// </summary>
        /// <param name="stream">Png bytes.</param>
        public ImageLines Deserialize(Stream stream, IProgress prog)
        {
            prog.Report(0);
            ImageLines image = null;
            using (stream)
            {
                var png = new PngReader(stream);
                png.SetUnpackedMode(true);
                image = png.ReadRowsByte();
                png.End();
            }

            prog.Report(1);
            return image;
        }

        /// <summary>
        /// Encodes a raw file buffer of image data into a PNG image.
        /// </summary>
        /// <param name="stream">Png bytes.</param>
        public void Serialize(Stream stream, ImageLines value, IProgress prog = null)
        {
            var subProgs = prog.Split("Copying", "Saving");
            var copyProg = subProgs[0];
            var saveProg = subProgs[1];
            var info = value.ImgInfo;

            var png = new PngWriter(stream, info)
            {
                CompLevel = compressionLevel,
                IdatMaxSize = IDATMaxSize
            };

            png.SetFilterType(FilterType.FILTER_PAETH);

            var metadata = png.GetMetadata();
            metadata.SetDpi(100);

            for (var i = 0; i < value.Nrows; ++i)
            {
                copyProg.Report(i, value.Nrows);
                png.WriteRow(value.GetImageLineAtMatrixRow(i), i);
                copyProg.Report(i + 1, value.Nrows);
            }

            saveProg.Report(0);
            png.End();
            saveProg.Report(1);
        }
    }
}