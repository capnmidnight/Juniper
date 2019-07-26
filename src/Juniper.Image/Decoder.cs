using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

using Juniper.Progress;

namespace Juniper.Image
{
    public static class Decoder
    {
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
        public static RawImage DecodePNG(Stream imageStream, bool flipImage)
        {
            var source = DetermineSource(imageStream);
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
                source,
                rows.elementsPerRow / rows.channels,
                numRows,
                data);
        }

        public static Task<RawImage> DecodePNGAsync(Stream imageStream, bool flipImage)
        {
            return Task.Run(() => DecodePNG(imageStream, flipImage));
        }

        public static RawImage DecodePNG(FileInfo file, bool flipImage)
        {
            if (!file.Exists)
            {
                throw new FileNotFoundException("File not found!", file.FullName);
            }
            else
            {
                using (var stream = file.OpenRead())
                {
                    return DecodePNG(stream, flipImage);
                }
            }
        }

        public static Task<RawImage> DecodePNGAsync(FileInfo file, bool flipImage)
        {
            return Task.Run(() => DecodePNG(file, flipImage));
        }

        public static RawImage DecodePNG(HttpWebResponse response, bool flipImage)
        {
            using (var stream = response.GetResponseStream())
            {
                return DecodePNG(stream, flipImage);
            }
        }

        public static Task<RawImage> DecodePNGAsync(HttpWebResponse response, bool flipImage)
        {
            return Task.Run(() => DecodePNG(response, flipImage));
        }

        public static RawImage DecodePNG(string fileName, bool flipImage)
        {
            return DecodePNG(new FileInfo(fileName), flipImage);
        }

        public static Task<RawImage> DecodePNGAsync(string fileName, bool flipImage)
        {
            return DecodePNGAsync(new FileInfo(fileName), flipImage);
        }

        public static RawImage DecodePNG(byte[] bytes, bool flipImage)
        {
            using (var mem = new MemoryStream(bytes))
            {
                return DecodePNG(mem, flipImage);
            }
        }

        public static Task<RawImage> DecodePNGAsync(byte[] bytes, bool flipImage)
        {
            return Task.Run(() => DecodePNG(bytes, flipImage));
        }

        /// <summary>
        /// Decodes a raw file buffer of JPEG data into raw image buffer, with width and height saved.
        /// </summary>
        /// <param name="imageStream">Jpeg bytes.</param>
        public static RawImage DecodeJPEG(Stream imageStream, bool flipImage)
        {
            var source = DetermineSource(imageStream);
            using (var jpeg = new BitMiracle.LibJpeg.JpegImage(imageStream))
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
                    source,
                    jpeg.Width,
                    jpeg.Height,
                    data);
            }
        }

        public static Task<RawImage> DecodeJPEGAsync(Stream imageStream, bool flipImage)
        {
            return Task.Run(() => DecodeJPEG(imageStream, flipImage));
        }

        public static RawImage DecodeJPEG(FileInfo file, bool flipImage)
        {
            if (!file.Exists)
            {
                throw new FileNotFoundException("File not found!", file.FullName);
            }
            else
            {
                using (var stream = file.OpenRead())
                {
                    return DecodeJPEG(stream, flipImage);
                }
            }
        }

        public static Task<RawImage> DecodeJPEGAsync(FileInfo file, bool flipImage)
        {
            return Task.Run(() => DecodeJPEG(file, flipImage));
        }

        public static RawImage DecodeJPEG(HttpWebResponse response, bool flipImage)
        {
            using (var stream = response.GetResponseStream())
            {
                return DecodeJPEG(stream, flipImage);
            }
        }

        public static Task<RawImage> DecodeJPEGAsync(HttpWebResponse response, bool flipImage)
        {
            return Task.Run(() => DecodeJPEG(response, flipImage));
        }

        public static RawImage DecodeJPEG(string fileName, bool flipImage)
        {
            return DecodeJPEG(new FileInfo(fileName), flipImage);
        }

        public static Task<RawImage> DecodeJPEGAsync(string fileName, bool flipImage)
        {
            return DecodeJPEGAsync(new FileInfo(fileName), flipImage);
        }

        public static RawImage DecodeJPEG(byte[] bytes, bool flipImage)
        {
            using (var mem = new MemoryStream(bytes))
            {
                return DecodeJPEG(mem, flipImage);
            }
        }

        public static Task<RawImage> DecodeJPEGAsync(byte[] bytes, bool flipImage)
        {
            return Task.Run(() => DecodeJPEG(bytes, flipImage));
        }

        public static RawImage Decode(HttpWebResponse response, bool flipImage)
        {
            if(response.ContentType == "image/jpeg"
                || response.ContentType == "image/jpg")
            {
                return DecodeJPEG(response, flipImage);
            }
            else if(response.ContentType == "image/png")
            {
                return DecodePNG(response, flipImage);
            }
            else
            {
                throw new ArgumentException($"Image format `{response.ContentType}` has not been implemented yet.");
            }
        }

        public static Task<RawImage> DecodeAsync(HttpWebResponse response, bool flipImage)
        {
            return Task.Run(() => Decode(response, flipImage));
        }

        public static RawImage Decode(ImageFormat format, Stream stream, bool flipImage)
        {
            if (format == ImageFormat.JPEG)
            {
                return DecodeJPEG(stream, flipImage);
            }
            else if (format == ImageFormat.PNG)
            {
                return DecodePNG(stream, flipImage);
            }
            else
            {
                throw new ArgumentException($"Image format `{format}` has not been implemented yet.");
            }
        }

        public static Task<RawImage> DecodeAsync(ImageFormat format, Stream stream, bool flipImage)
        {
            return Task.Run(() => Decode(format, stream, flipImage));
        }

        public static RawImage Decode(ImageFormat format, FileInfo file, bool flipImage)
        {
            if (!file.Exists)
            {
                throw new FileNotFoundException("File not found!", file.FullName);
            }
            else
            {
                using (var stream = file.OpenRead())
                {
                    return Decode(format, stream, flipImage);
                }
            }
        }

        public static Task<RawImage> DecodeAsync(ImageFormat format, FileInfo file, bool flipImage)
        {
            return Task.Run(() => Decode(format, file, flipImage));
        }

        public static RawImage Decode(ImageFormat format, string fileName, bool flipImage)
        {
            return Decode(format, new FileInfo(fileName), flipImage);
        }

        public static Task<RawImage> DecodeAsync(ImageFormat format, string fileName, bool flipImage)
        {
            return DecodeAsync(format, new FileInfo(fileName), flipImage);
        }

        public static RawImage Decode(ImageFormat format, byte[] data, bool flipImage)
        {
            if (format == ImageFormat.JPEG)
            {
                return DecodeJPEG(data, flipImage);
            }
            else if (format == ImageFormat.PNG)
            {
                return DecodePNG(data, flipImage);
            }
            else
            {
                throw new ArgumentException($"Image format `{format}` has not been implemented yet.");
            }
        }

        public static Task<RawImage> DecodeAsync(ImageFormat format, byte[] data, bool flipImage)
        {
            return Task.Run(() => Decode(format, data, flipImage));
        }
    }
}