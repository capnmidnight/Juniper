using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using BitMiracle.LibJpeg;
using Juniper.Progress;

namespace Juniper.Image
{
    public static class Decoder
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
        /// Decodes a raw file buffer of PNG data into raw image buffer, with width and height saved.
        /// </summary>
        /// <param name="imageStream">Png bytes.</param>
        public static RawImage DecodePNG(bool flipImage, Stream imageStream)
        {
            var png = new Hjg.Pngcs.PngReader(imageStream);
            png.SetUnpackedMode(true);
            var rows = png.ReadRowsByte();
            int numRows = rows.Nrows;
            var data = new byte[numRows * rows.elementsPerRow];
            for (var i = 0; i < numRows; ++i)
            {
                var rowIndex = GetRowIndex(numRows, i, flipImage);
                var row = rows.ScanlinesB[rowIndex];
                Array.Copy(row, 0, data, i * rows.elementsPerRow, row.Length);
            }

            return new RawImage(
                DetermineSource(imageStream),
                rows.elementsPerRow / rows.channels,
                numRows,
                data);
        }

        /// <summary>
        /// Decodes a raw file buffer of JPEG data into raw image buffer, with width and height saved.
        /// </summary>
        /// <param name="imageStream">Jpeg bytes.</param>
        public static RawImage DecodeJPEG(bool flipImage, Stream imageStream)
        {
            using (var jpeg = new JpegImage(imageStream))
            {
                var stride = jpeg.Width * jpeg.ComponentsPerSample;
                int numRows = jpeg.Height;
                var data = new byte[numRows * stride];
                for (var i = 0; i < jpeg.Height; ++i)
                {
                    var rowIndex = GetRowIndex(numRows, i, flipImage);
                    var row = jpeg.GetRow(rowIndex);
                    Array.Copy(row.ToBytes(), 0, data, i * stride, stride);
                }

                return new RawImage(
                    DetermineSource(imageStream),
                    jpeg.Width,
                    jpeg.Height,
                    data);
            }
        }

        private static RawImage.ImageSource DetermineSource(Stream imageStream)
        {
            var source = RawImage.ImageSource.None;
            if (imageStream is FileStream)
            {
                source = RawImage.ImageSource.File;
            }
            else if (imageStream is CachingStream)
            {
                source = RawImage.ImageSource.Network;
            }

            return source;
        }

        public static Task<RawImage> DecodePNGAsync(bool flipImage, byte[] bytes)
        {
            using (var mem = new MemoryStream(bytes))
            {
                return DecodePNGAsync(flipImage, mem);
            }
        }

        public static RawImage DecodePNG(bool flipImage, byte[] bytes)
        {
            using (var mem = new MemoryStream(bytes))
            {
                return DecodePNG(flipImage, mem);
            }
        }

        public static Task<RawImage> DecodePNGAsync(bool flipImage, Stream imageStream)
        {
            return Task.Run(() => DecodePNG(flipImage, imageStream));
        }

        public static Task<RawImage> DecodeJPEGAsync(bool flipImage, byte[] bytes)
        {
            using (var mem = new MemoryStream(bytes))
            {
                return DecodeJPEGAsync(flipImage, mem);
            }
        }

        public static RawImage DecodeJPEG(bool flipImage, byte[] bytes)
        {
            using (var mem = new MemoryStream(bytes))
            {
                return DecodeJPEG(flipImage, mem);
            }
        }

        public static Task<RawImage> DecodeJPEGAsync(bool flipImage, Stream imageStream)
        {
            return Task.Run(() => DecodeJPEG(flipImage, imageStream));
        }

        public static Task<RawImage> DecodeResponseAsync(bool flipImage, HttpWebResponse response)
        {
            return Task.Run(() => DecodeResponse(flipImage, response));
        }

        public static RawImage DecodeResponse(bool flipImage, HttpWebResponse response)
        {
            using (var stream = response.GetResponseStream())
            {
                if (response.ContentType == "image/jpeg")
                {
                    return Decode(SupportedFormats.JPEG, flipImage, stream);
                }
                else if (response.ContentType == "image/png")
                {
                    return Decode(SupportedFormats.PNG, flipImage, stream);
                }
                else
                {
                    throw new FormatException($"Image format `{response.ContentType}` could not be decoded. Supported formats: JPEG, PNG. Source: {response.ResponseUri}");
                }
            }
        }

        public static RawImage Decode(SupportedFormats format, bool flipImage, Stream stream)
        {
            if (format == SupportedFormats.JPEG)
            {
                return DecodeJPEG(flipImage, stream);
            }
            else if (format == SupportedFormats.PNG)
            {
                return DecodePNG(flipImage, stream);
            }
            else
            {
                throw new ArgumentException($"Image format `{format}` has not been implemented yet.");
            }
        }
    }
}