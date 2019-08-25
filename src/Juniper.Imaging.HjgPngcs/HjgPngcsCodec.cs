using System;
using System.IO;
using System.Linq;

using Hjg.Pngcs;

using Juniper.Progress;

namespace Juniper.Imaging.HjgPngcs
{
    public class HjgPngcsCodec : IImageDecoder<ImageLines>
    {
        private readonly int compressionLevel;
        private readonly int IDATMaxSize;

        /// <summary>
        ///
        /// </summary>
        /// <param name="compressionLevel">values 0 - 9</param>
        /// <param name="IDATMaxSize"></param>
        public HjgPngcsCodec(int compressionLevel = 9, int IDATMaxSize = 0x1000)
        {
            this.compressionLevel = compressionLevel;
            this.IDATMaxSize = IDATMaxSize;
        }

        public HTTP.MediaType.Image Format { get { return HTTP.MediaType.Image.Png; } }

        public ImageInfo GetImageInfo(byte[] data)
        {
            return ImageInfo.ReadPNG(data);
        }

        public int GetWidth(ImageLines image)
        {
            return image.ImgInfo.BytesPerRow / image.ImgInfo.BytesPixel;
        }

        public int GetHeight(ImageLines image)
        {
            return image.Nrows;
        }

        /// <summary>
        /// Decodes a raw file buffer of PNG data into raw image buffer, with width and height saved.
        /// </summary>
        /// <param name="imageStream">Png bytes.</param>
        public ImageLines Deserialize(Stream imageStream)
        {
            var png = new PngReader(imageStream);
            png.SetUnpackedMode(true);
            var lines = png.ReadRowsByte();
            png.End();

            return lines;
        }

        /// <summary>
        /// Encodes a raw file buffer of image data into a PNG image.
        /// </summary>
        /// <param name="outputStream">Png bytes.</param>
        public void Serialize(Stream outputStream, ImageLines image, IProgress prog = null)
        {
            var subProgs = prog.Split("Copying", "Saving");
            var copyProg = subProgs[0];
            var saveProg = subProgs[1];
            var info = image.ImgInfo;

            var png = new PngWriter(outputStream, info)
            {
                CompLevel = compressionLevel,
                IdatMaxSize = IDATMaxSize
            };

            png.SetFilterType(FilterType.FILTER_PAETH);

            var metadata = png.GetMetadata();
            metadata.SetDpi(100);

            for (var i = 0; i < image.Nrows; ++i)
            {
                copyProg?.Report(i, image.Nrows);
                png.WriteRow(image.GetImageLineAtMatrixRow(i), i);
                copyProg?.Report(i + 1, image.Nrows);
            }

            saveProg?.Report(0);
            png.End();
            saveProg?.Report(1);
        }

        public ImageLines Concatenate(ImageLines[,] images, IProgress prog)
        {
            this.ValidateImages(images, prog,
                out var rows, out var columns,
                out var tileWidth,
                out var tileHeight);

            var firstImage = images.Where(img => img != null).First();

            var combinedInfo = new Hjg.Pngcs.ImageInfo(
                columns * tileWidth,
                rows * tileHeight,
                firstImage.bitDepth,
                firstImage.ImgInfo.Alpha);

            var combinedLines = new ImageLines(
                combinedInfo,
                firstImage.sampleType,
                firstImage.SamplesUnpacked,
                0,
                rows * tileHeight,
                combinedInfo.SamplesPerRow);

            for(var y = 0; y < rows; ++y)
            {
                for(var x = 0; x < columns; ++x)
                {
                    var tile = images[y, x];
                    if(tile != null)
                    {
                        for(var i = 0; i < tileHeight; ++i)
                        {
                            var bufferY = y * tileHeight + i;
                            var bufferX = x * tileWidth;
                            if (firstImage.sampleType == ImageLine.ESampleType.INT)
                            {
                                var bufferLine = combinedLines.Scanlines[bufferY];
                                var tileLine = tile.Scanlines[i];
                                Array.Copy(tileLine, 0, bufferLine, bufferX, tileLine.Length);
                            }
                            else
                            {
                                var bufferLine = combinedLines.ScanlinesB[bufferY];
                                var tileLine = tile.ScanlinesB[i];
                                Array.Copy(tileLine, 0, bufferLine, bufferX, tileLine.Length);
                            }
                        }
                    }
                }
            }

            return combinedLines;
        }
    }
}