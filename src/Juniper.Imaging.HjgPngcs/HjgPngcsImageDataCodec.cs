using System;
using System.IO;

using Hjg.Pngcs;

using Juniper.Progress;
using Juniper.Serialization;

namespace Juniper.Imaging.HjgPngcs
{
    public class HjgPngcsImageDataCodec : AbstractImageDataDecoder
    {
        private readonly int compressionLevel;
        private readonly int IDATMaxSize;

        /// <summary>
        ///
        /// </summary>
        /// <param name="compressionLevel">values 0 - 9</param>
        /// <param name="IDATMaxSize"></param>
        public HjgPngcsImageDataCodec(int compressionLevel = 9, int IDATMaxSize = 0x1000)
        {
            this.compressionLevel = compressionLevel;
            this.IDATMaxSize = IDATMaxSize;
        }

        public override HTTP.MediaType.Image Format { get { return HTTP.MediaType.Image.Png; } }

        public override ImageInfo GetImageInfo(byte[] data, DataSource source = DataSource.None)
        {
            return ImageInfo.ReadPNG(data, source);
        }

        /// <summary>
        /// Decodes a raw file buffer of PNG data into raw image buffer, with width and height saved.
        /// </summary>
        /// <param name="imageStream">Png bytes.</param>
        public override ImageData Deserialize(Stream imageStream)
        {
            var source = imageStream.DetermineSource();
            var png = new PngReader(imageStream);
            png.SetUnpackedMode(true);
            var rows = png.ReadRowsByte();
            var numRows = rows.Nrows;
            var data = new byte[numRows * rows.elementsPerRow];
            for (var i = 0; i < numRows; ++i)
            {
                var rowIndex = ImageData.GetRowIndex(numRows, i, true);
                var row = rows.ScanlinesB[rowIndex];
                Array.Copy(row, 0, data, i * rows.elementsPerRow, row.Length);
            }

            return new ImageData(
                source,
                rows.elementsPerRow / rows.channels,
                numRows,
                rows.channels,
                HTTP.MediaType.Image.Png,
                data);
        }

        /// <summary>
        /// Encodes a raw file buffer of image data into a PNG image.
        /// </summary>
        /// <param name="outputStream">Png bytes.</param>
        public override void Serialize(Stream outputStream, ImageData image, IProgress prog = null)
        {
            var subProgs = prog.Split("Copying", "Saving");
            var copyProg = subProgs[0];
            var saveProg = subProgs[1];
            var info = new Hjg.Pngcs.ImageInfo(
                image.info.dimensions.width,
                image.info.dimensions.height,
                ImageData.BitsPerComponent,
                false);

            var png = new PngWriter(outputStream, info)
            {
                CompLevel = compressionLevel,
                IdatMaxSize = IDATMaxSize
            };

            png.SetFilterType(FilterType.FILTER_PAETH);

            var metadata = png.GetMetadata();
            metadata.SetDpi(100);

            var line = new ImageLine(info, ImageLine.ESampleType.BYTE);
            for (var i = 0; i < image.info.dimensions.height; ++i)
            {
                copyProg?.Report(i, image.info.dimensions.height);
                var row = ImageData.GetRowIndex(image.info.dimensions.height, i, true);
                Array.Copy(image.data, row * image.info.stride, line.ScanlineB, 0, image.info.stride);
                png.WriteRow(line, i);
                copyProg?.Report(i + 1, image.info.dimensions.height);
            }

            saveProg?.Report(0);
            png.End();
            saveProg?.Report(1);
        }
    }
}