using System;
using System.IO;

using BitMiracle.LibJpeg;

using Juniper.Progress;
using Juniper.Serialization;

namespace Juniper.Imaging.LibJpegNET
{
    public class LibJpegNETImageDataCodec : AbstractImageDataDecoder
    {
        private readonly LibJpegNETCodec subCodec;

        public LibJpegNETImageDataCodec(int quality = 100, int smoothingFactor = 1, bool progressive = false)
        {
            subCodec = new LibJpegNETCodec(quality, smoothingFactor, progressive);
        }

        public override ImageInfo GetImageInfo(byte[] data, DataSource source = DataSource.None)
        {
            return ImageInfo.ReadJPEG(data, source);
        }

        public override HTTP.MediaType.Image Format { get { return HTTP.MediaType.Image.Jpeg; } }

        /// <summary>
        /// Decodes a raw file buffer of JPEG data into raw image buffer, with width and height saved.
        /// </summary>
        /// <param name="imageStream">Jpeg bytes.</param>
        public override ImageData Deserialize(Stream imageStream)
        {
            var source = imageStream.DetermineSource();
            using (var jpeg = subCodec.Deserialize(imageStream))
            {
                var stride = jpeg.Width * jpeg.ComponentsPerSample;
                var numRows = jpeg.Height;
                var data = new byte[numRows * stride];
                for (var i = 0; i < jpeg.Height; ++i)
                {
                    var rowIndex = ImageData.GetRowIndex(numRows, i, true);
                    var row = jpeg.GetRow(rowIndex);
                    Array.Copy(row.ToBytes(), 0, data, i * stride, stride);
                }

                return new ImageData(
                    source,
                    jpeg.Width,
                    jpeg.Height,
                    jpeg.ComponentsPerSample,
                    HTTP.MediaType.Image.Jpeg,
                    data);
            }
        }

        /// <summary>
        /// Encodes a raw file buffer of image data into a JPEG image.
        /// </summary>
        /// <param name="outputStream">Jpeg bytes.</param>
        public override void Serialize(Stream outputStream, ImageData image, IProgress prog = null)
        {
            var subProgs = prog.Split("Copying", "Saving");
            var copyProg = subProgs[0];
            var saveProg = subProgs[1];
            var rows = new SampleRow[image.info.dimensions.height];
            var rowBuffer = new byte[image.info.stride];
            for (var i = 0; i < image.info.dimensions.height; ++i)
            {
                copyProg.Report(i, image.info.dimensions.height);
                var rowIndex = ImageData.GetRowIndex(image.info.dimensions.height, i, true);
                var imageDataIndex = rowIndex * image.info.stride;
                Array.Copy(image.data, imageDataIndex, rowBuffer, 0, rowBuffer.Length);
                rows[i] = new SampleRow(
                    rowBuffer,
                    image.info.dimensions.width,
                    ImageData.BitsPerComponent,
                    (byte)image.info.components);
                copyProg.Report(i + 1, image.info.dimensions.height);
            }

            saveProg?.Report(0);
            using (var jpeg = new JpegImage(rows, Colorspace.RGB))
            {
                subCodec.Serialize(jpeg, saveProg);
            }
            saveProg?.Report(1);
        }
    }
}