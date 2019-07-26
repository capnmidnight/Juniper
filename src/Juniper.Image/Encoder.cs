using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace Juniper.Image
{
    public static class Encoder
    {
        public enum SupportedFormats
        {
            Unsupported,
            JPEG,
            PNG
        }

        private static int GetRowIndex(int numRows, int i, bool flipImage)
        {
            int rowIndex = i;
            if (flipImage)
            {
                rowIndex = numRows - i - 1;
            }

            return rowIndex;
        }

        /// <summary>
        /// Encodes a raw file buffer of image data into a PNG image.
        /// </summary>
        /// <param name="outputStream">Png bytes.</param>
        public static void EncodePNG(RawImage image, Stream outputStream, bool flipImage)
        {
            var info = new Hjg.Pngcs.ImageInfo(
                image.dimensions.width,
                image.dimensions.height,
                RawImage.BitsPerComponent,
                false);

            var png = new Hjg.Pngcs.PngWriter(outputStream, info);
            png.SetFilterType(Hjg.Pngcs.FilterType.FILTER_VERYAGGRESSIVE);
            png.IdatMaxSize = 0x10000;
            png.CompLevel = 6;

            var metadata = png.GetMetadata();
            metadata.SetDpi(100);

            var line = new Hjg.Pngcs.ImageLine(info, Hjg.Pngcs.ImageLine.ESampleType.BYTE);
            for (var i = 0; i < image.dimensions.height; ++i)
            {
                var row = GetRowIndex(image.dimensions.height, i, flipImage);
                Array.Copy(image.data, row * image.stride, line.ScanlineB, 0, image.stride);
                png.WriteRow(line, i);
            }

            png.End();
        }

        public static Task EncodePNGAsync(RawImage image, Stream outputStream, bool flipImage)
        {
            return Task.Run(() => EncodePNG(image, outputStream, flipImage));
        }

        public static void EncodePNG(RawImage image, FileInfo file, bool flipImage)
        {
            using (var stream = file.OpenWrite())
            {
                EncodePNG(image, stream, flipImage);
            }
        }

        public static Task EncodePNGAsync(RawImage image, FileInfo file, bool flipImage)
        {
            return Task.Run(() => EncodePNG(image, file, flipImage));
        }

        public static void EncodePNG(RawImage image, HttpWebRequest request, bool flipImage)
        {
            using (var stream = request.GetRequestStream())
            {
                EncodePNG(image, stream, flipImage);
            }
        }

        public static Task EncodePNGAsync(RawImage image, HttpWebRequest request, bool flipImage)
        {
            return Task.Run(() => EncodePNG(image, request, flipImage));
        }

        public static void EncodePNG(RawImage image, string fileName, bool flipImage)
        {
            EncodePNG(image, new FileInfo(fileName), flipImage);
        }

        public static Task EncodePNGAsync(RawImage image, string fileName, bool flipImage)
        {
            return Task.Run(() => EncodePNG(image, fileName, flipImage));
        }

        public static byte[] EncodePNG(RawImage image, bool flipImage)
        {
            using (var mem = new MemoryStream())
            {
                EncodePNG(image, mem, flipImage);
                return mem.ToArray();
            }
        }

        public static Task<byte[]> EncodePNGAsync(RawImage image, bool flipImage)
        {
            return Task.Run(() => EncodePNG(image, flipImage));
        }

        /// <summary>
        /// Encodes a raw file buffer of image data into a JPEG image.
        /// </summary>
        /// <param name="outputStream">Jpeg bytes.</param>
        public static void EncodeJPEG(RawImage image, Stream outputStream, bool flipImage)
        {
            var rows = new BitMiracle.LibJpeg.SampleRow[image.dimensions.height];
            var buf = new byte[image.stride];
            for(int i = 0; i < image.dimensions.height; ++i)
            {
                var rowIndex = GetRowIndex(image.dimensions.height, i, flipImage);
                Array.Copy(image.data, rowIndex, buf, 0, buf.Length);
                rows[i] = new BitMiracle.LibJpeg.SampleRow(
                    buf,
                    image.dimensions.width,
                    RawImage.BitsPerComponent,
                    (byte)image.components);
            }

            using(var jpeg = new BitMiracle.LibJpeg.JpegImage(rows, BitMiracle.LibJpeg.Colorspace.RGB))
            {
                var compression = new BitMiracle.LibJpeg.CompressionParameters
                {
                    Quality = 10,
                    SimpleProgressive = false,
                    SmoothingFactor = 2
                };
                jpeg.WriteJpeg(outputStream, compression);
            }
        }

        public static Task EncodeJPEGAsync(RawImage image, Stream outputStream, bool flipImage)
        {
            return Task.Run(() => EncodeJPEG(image, outputStream, flipImage));
        }

        public static void EncodeJPEG(RawImage image, FileInfo file, bool flipImage)
        {
            using (var stream = file.OpenWrite())
            {
                EncodeJPEG(image, stream, flipImage);
            }
        }

        public static Task EncodeJPEGAsync(RawImage image, FileInfo file, bool flipImage)
        {
            return Task.Run(() => EncodeJPEG(image, file, flipImage));
        }

        public static void EncodeJPEG(RawImage image, HttpWebRequest request, bool flipImage)
        {
            using (var stream = request.GetRequestStream())
            {
                EncodeJPEG(image, stream, flipImage);
            }
        }

        public static Task EncodeJPEGAsync(RawImage image, HttpWebRequest request, bool flipImage)
        {
            return Task.Run(() => EncodeJPEG(image, request, flipImage));
        }

        public static void EncodeJPEG(RawImage image, string fileName, bool flipImage)
        {
            EncodeJPEG(image, new FileInfo(fileName), flipImage);
        }

        public static Task EncodeJPEGAsync(RawImage image, string fileName, bool flipImage)
        {
            return EncodeJPEGAsync(image, new FileInfo(fileName), flipImage);
        }

        public static byte[] EncodeJPEG(RawImage image, bool flipImage)
        {
            using (var mem = new MemoryStream())
            {
                EncodeJPEG(image, mem, flipImage);
                return mem.ToArray();
            }
        }

        public static Task<byte[]> EncodeJPEGAsync(RawImage image, bool flipImage)
        {
            return Task.Run(() => EncodeJPEG(image, flipImage));
        }

        public static void Encode(SupportedFormats format, RawImage image, Stream stream, bool flipImage)
        {
            if (format == SupportedFormats.JPEG)
            {
                EncodeJPEG(image, stream, flipImage);
            }
            else if (format == SupportedFormats.PNG)
            {
                EncodePNG(image, stream, flipImage);
            }
            else
            {
                throw new ArgumentException($"Image format `{format}` has not been implemented yet.");
            }
        }

        public static Task EncodeAsync(SupportedFormats format, RawImage image, Stream stream, bool flipImage)
        {
            if (format == SupportedFormats.JPEG)
            {
                return EncodeJPEGAsync(image, stream, flipImage);
            }
            else if (format == SupportedFormats.PNG)
            {
                return EncodePNGAsync(image, stream, flipImage);
            }
            else
            {
                throw new ArgumentException($"Image format `{format}` has not been implemented yet.");
            }
        }

        public static void Encode(SupportedFormats format, RawImage image, FileInfo file, bool flipImage)
        {
            using (var stream = file.OpenWrite())
            {
                Encode(format, image, stream, flipImage);
            }
        }

        public static Task EncodeAsync(SupportedFormats format, RawImage image, FileInfo file, bool flipImage)
        {
            if (format == SupportedFormats.JPEG)
            {
                return EncodeJPEGAsync(image, file, flipImage);
            }
            else if (format == SupportedFormats.PNG)
            {
                return EncodePNGAsync(image, file, flipImage);
            }
            else
            {
                throw new ArgumentException($"Image format `{format}` has not been implemented yet.");
            }
        }

        public static void Encode(SupportedFormats format, RawImage image, HttpWebRequest request, bool flipImage)
        {
            using (var stream = request.GetRequestStream())
            {
                Encode(format, image, stream, flipImage);
            }
        }

        public static Task EncodeAsync(SupportedFormats format, RawImage image, HttpWebRequest request, bool flipImage)
        {
            if (format == SupportedFormats.JPEG)
            {
                return EncodeJPEGAsync(image, request, flipImage);
            }
            else if (format == SupportedFormats.PNG)
            {
                return EncodePNGAsync(image, request, flipImage);
            }
            else
            {
                throw new ArgumentException($"Image format `{format}` has not been implemented yet.");
            }
        }

        public static void Encode(SupportedFormats format, RawImage image, string fileName, bool flipImage)
        {
            Encode(format, image, new FileInfo(fileName), flipImage);
        }

        public static Task EncodeAsync(SupportedFormats format, RawImage image, string fileName, bool flipImage)
        {
            return EncodeAsync(format, image, new FileInfo(fileName), flipImage);
        }

        public static byte[] Encode(SupportedFormats format, RawImage image, bool flipImage)
        {
            if (format == SupportedFormats.JPEG)
            {
                return EncodeJPEG(image, flipImage);
            }
            else if (format == SupportedFormats.PNG)
            {
                return EncodePNG(image, flipImage);
            }
            else
            {
                throw new ArgumentException($"Image format `{format}` has not been implemented yet.");
            }
        }

        public static Task<byte[]> EncodeAsync(SupportedFormats format, RawImage image, bool flipImage)
        {
            if (format == SupportedFormats.JPEG)
            {
                return EncodeJPEGAsync(image, flipImage);
            }
            else if (format == SupportedFormats.PNG)
            {
                return EncodePNGAsync(image, flipImage);
            }
            else
            {
                throw new ArgumentException($"Image format `{format}` has not been implemented yet.");
            }
        }
    }
}