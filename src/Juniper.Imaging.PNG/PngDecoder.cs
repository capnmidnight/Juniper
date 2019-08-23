using System;
using System.IO;

using Hjg.Pngcs;

using Juniper.Progress;
using Juniper.Serialization;

namespace Juniper.Imaging.PNG
{
    public class PngDecoder : AbstractImageDataDecoder
    {
        private readonly int compressionLevel;
        private readonly int IDATMaxSize;

        /// <summary>
        ///
        /// </summary>
        /// <param name="compressionLevel">values 0 - 9</param>
        /// <param name="IDATMaxSize"></param>
        public PngDecoder(int compressionLevel = 9, int IDATMaxSize = 0x1000)
        {
            this.compressionLevel = compressionLevel;
            this.IDATMaxSize = IDATMaxSize;
        }

        public override HTTP.MediaType.Image Format { get { return HTTP.MediaType.Image.Png; } }

        public override ImageData Read(byte[] data, DataSource source = DataSource.None)
        {
            int width = 0,
                height = 0;

            var i = 8; // skip the PNG signature

            while (i < data.Length)
            {
                var len = 0;
                len = len << 8 | data[i++];
                len = len << 8 | data[i++];
                len = len << 8 | data[i++];
                len = len << 8 | data[i++];

                var chunk = System.Text.Encoding.UTF8.GetString(data, i, 4);
                i += 4;

                if (chunk == "IHDR")
                {
                    width = width << 8 | data[i++];
                    width = width << 8 | data[i++];
                    width = width << 8 | data[i++];
                    width = width << 8 | data[i++];

                    height = height << 8 | data[i++];
                    height = height << 8 | data[i++];
                    height = height << 8 | data[i++];
                    height = height << 8 | data[i++];

                    var bitDepth = data[i + 9];
                    var colorType = data[i + 10];

                    var components = 0;
                    switch (colorType)
                    {
                        case 0: components = (int)Math.Ceiling((float)bitDepth / 8); break;
                        case 2: components = 3; break;
                        case 3: components = 1; break;
                        case 4: components = (int)Math.Ceiling((float)bitDepth / 8) + 1; break;
                        case 6: components = 4; break;
                    }

                    return new ImageData(
                        source,
                        width,
                        height, components, HTTP.MediaType.Image.Png,
                        data);
                }

                i += len;
                i += 4;
            }

            return default;
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
            var info = new ImageInfo(
                image.dimensions.width,
                image.dimensions.height,
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
            for (var i = 0; i < image.dimensions.height; ++i)
            {
                copyProg?.Report(i, image.dimensions.height);
                var row = ImageData.GetRowIndex(image.dimensions.height, i, true);
                Array.Copy(image.data, row * image.stride, line.ScanlineB, 0, image.stride);
                png.WriteRow(line, i);
                copyProg?.Report(i + 1, image.dimensions.height);
            }

            saveProg.Report(0);
            png.End();
            saveProg.Report(1);
        }
    }
}