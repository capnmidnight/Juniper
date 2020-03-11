using System;

using BitMiracle.LibJpeg;

using Juniper.IO;

namespace Juniper.Imaging
{
    public class LibJpegNETImageDataTranscoder : IImageTranscoder<JpegImage, ImageData>
    {
        private readonly bool padAlpha;

        public LibJpegNETImageDataTranscoder(bool padAlpha = false)
        {
            this.padAlpha = padAlpha;
        }

        /// <summary>
        /// Decodes a raw file buffer of JPEG data into raw image buffer, with width and height saved.
        /// </summary>
        /// <param name="imageStream">Jpeg bytes.</param>
        public ImageData Translate(JpegImage value, IProgress prog = null)
        {
            if (value is null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            var inputStride = value.Width * value.ComponentsPerSample;
            var outputComponents = padAlpha ? value.ComponentsPerSample + 1 : value.ComponentsPerSample;
            var outputStride = value.Width * outputComponents;
            var numRows = value.Height;
            var data = new byte[numRows * outputStride];
            for (var i = 0; i < value.Height; ++i)
            {
                prog.Report(i, value.Height);
                var row = value.GetRow(i);
                var outputI = i * outputStride;
                Array.Copy(row.ToBytes(), 0, data, outputI, inputStride);
                if(inputStride != outputStride)
                {
                    for(var j = outputI + outputStride - 1; j >= outputI; --j)
                    {
                        var outputComponent = j % outputComponents;
                        var inputComponent = outputComponent % value.ComponentsPerSample;
                        var outputXY = j / outputComponents;
                        var inputXY = outputXY * value.ComponentsPerSample;
                        var inputI = inputXY + inputComponent;
                        var v = outputComponent == inputComponent ? data[inputI] : byte.MaxValue;
                        data[j] = v;
                    }
                }
                prog.Report(i + 1, value.Height);
            }

            return new ImageData(
                value.Width,
                value.Height,
                value.ComponentsPerSample,
                data);
        }

        /// <summary>
        /// Encodes a raw file buffer of image data into a JPEG image.
        /// </summary>
        /// <param name="outputStream">Jpeg bytes.</param>
        public JpegImage Translate(ImageData image, IProgress prog = null)
        {
            if (image is null)
            {
                throw new ArgumentNullException(nameof(image));
            }

            var subProgs = prog.Split("Copying", "Saving");
            var copyProg = subProgs[0];
            var saveProg = subProgs[1];
            var rows = new SampleRow[image.Info.Dimensions.Height];
            var rowBuffer = new byte[image.Info.Stride];
            var components = (byte)Math.Min(image.Info.Components, 3);
            for (var y = 0; y < image.Info.Dimensions.Height; ++y)
            {
                copyProg.Report(y, image.Info.Dimensions.Height);
                for (var x = 0; x < image.Info.Dimensions.Width; ++x)
                {
                    var imageDataIndex = (y * image.Info.Stride) + (x * image.Info.Components);
                    var rowIndex = x * components;
                    Array.Copy(image.GetData(), imageDataIndex, rowBuffer, rowIndex, components);
                }

                rows[y] = new SampleRow(
                    rowBuffer,
                    image.Info.Dimensions.Width,
                    Units.Bits.PER_BYTE,
                    components);
                copyProg.Report(y + 1, image.Info.Dimensions.Height);
            }

            saveProg.Report(0);
            var jpeg = new JpegImage(rows, Colorspace.RGB);
            saveProg.Report(1);
            return jpeg;
        }
    }
}