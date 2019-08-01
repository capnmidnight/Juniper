using System;
using System.IO;

using Hjg.Pngcs;
using Juniper.Progress;
using Juniper.Serialization;

namespace Juniper.Image.PNG
{
    public class Factory : IFactory<ImageData>
    {
        public static ImageData Read(byte[] data, ImageSource source = ImageSource.None)
        {
            int width = 0,
                height = 0;

            int i = 8; // skip the PNG signature

            while (i < data.Length)
            {
                int len = 0;
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

                    int components = 0;
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
                        ImageFormat.PNG,
                        width, height, components,
                        data);
                }

                i += len;
                i += 4;
            }

            return default;
        }

        public static ImageData Read(Stream stream)
        {
            var source = ImageData.DetermineSource(stream);
            using (var mem = new MemoryStream())
            {
                stream.CopyTo(mem);
                mem.Flush();
                return Read(mem.ToArray(), source);
            }
        }

        public static ImageData Read(string fileName)
        {
            return Read(File.ReadAllBytes(fileName), ImageSource.File);
        }

        public static ImageData Read(FileInfo file)
        {
            return Read(file.FullName);
        }

        /// <summary>
        /// Decodes a raw file buffer of PNG data into raw image buffer, with width and height saved.
        /// </summary>
        /// <param name="imageStream">Png bytes.</param>
        public ImageData Deserialize(Stream imageStream)
        {
            var source = ImageData.DetermineSource(imageStream);
            var png = new PngReader(imageStream);
            png.SetUnpackedMode(true);
            var rows = png.ReadRowsByte();
            int numRows = rows.Nrows;
            var data = new byte[numRows * rows.elementsPerRow];
            for (var i = 0; i < numRows; ++i)
            {
                var rowIndex = ImageData.GetRowIndex(numRows, i, true);
                var row = rows.ScanlinesB[rowIndex];
                Array.Copy(row, 0, data, i * rows.elementsPerRow, row.Length);
            }

            return new ImageData(
                source,
                ImageFormat.PNG,
                rows.elementsPerRow / rows.channels,
                numRows,
                data);
        }

        /// <summary>
        /// Encodes a raw file buffer of image data into a PNG image.
        /// </summary>
        /// <param name="outputStream">Png bytes.</param>
        public void Serialize(Stream outputStream, ImageData image, IProgress prog = null)
        {
            var info = new ImageInfo(
                image.dimensions.width,
                image.dimensions.height,
                ImageData.BitsPerComponent,
                false);

            var png = new PngWriter(outputStream, info)
            {
                IdatMaxSize = 0x1000,
                CompLevel = 8
            };

            png.SetFilterType(FilterType.FILTER_PAETH);

            var metadata = png.GetMetadata();
            metadata.SetDpi(100);

            var line = new ImageLine(info, ImageLine.ESampleType.BYTE);
            for (var i = 0; i < image.dimensions.height; ++i)
            {
                var row = ImageData.GetRowIndex(image.dimensions.height, i, true);
                Array.Copy(image.data, row * image.stride, line.ScanlineB, 0, image.stride);
                png.WriteRow(line, i);
            }

            png.End();
        }
    }
}