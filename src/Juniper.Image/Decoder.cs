using BitMiracle.LibJpeg;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace Juniper.Image
{
    public static class Decoder
    {
        /// <summary>
        /// Decodes a raw file buffer of PNG data into raw image buffer, with width and height saved.
        /// </summary>
        /// <param name="imageStream">Png bytes.</param>
        public static Task<RawImage> DecodePNGAsync(byte[] bytes)
        {
            using (var mem = new MemoryStream(bytes))
            {
                return DecodePNGAsync(mem);
            }
        }

        public static RawImage DecodePNG(byte[] bytes)
        {
            using (var mem = new MemoryStream(bytes))
            {
                return DecodePNG(mem);
            }
        }

        public static Task<RawImage> DecodePNGAsync(Stream imageStream)
        {
            return Task.Run(() => DecodePNG(imageStream));
        }

        public static RawImage DecodePNG(Stream imageStream)
        {
            var png = new Hjg.Pngcs.PngReader(imageStream);
            png.SetUnpackedMode(true);
            var rows = png.ReadRowsByte();
            var data = new byte[rows.Nrows * rows.elementsPerRow];
            for (var i = 0; i < rows.Nrows; ++i)
            {
                var row = rows.ScanlinesB[rows.Nrows - i - 1];
                Array.Copy(row, 0, data, i * rows.elementsPerRow, row.Length);
            }

            return new RawImage
            {
                width = rows.elementsPerRow / rows.channels,
                height = rows.Nrows,
                data = data
            };
        }

        public static Task<RawImage> DecodeJPEGAsync(byte[] bytes)
        {
            using (var mem = new MemoryStream(bytes))
            {
                return DecodeJPEGAsync(mem);
            }
        }

        public static RawImage DecodeJPEG(byte[] bytes)
        {
            using (var mem = new MemoryStream(bytes))
            {
                return DecodeJPEG(mem);
            }
        }

        public static Task<RawImage> DecodeJPEGAsync(Stream imageStream)
        {
            return Task.Run(() => DecodeJPEG(imageStream));
        }

        public static RawImage DecodeJPEG(Stream imageStream)
        {
            using (var jpeg = new JpegImage(imageStream))
            {
                var stride = jpeg.Width * jpeg.ComponentsPerSample;
                var data = new byte[jpeg.Height * stride];
                for (var i = 0; i < jpeg.Height; ++i)
                {
                    var row = jpeg.GetRow(i);
                    Array.Copy(row.ToBytes(), 0, data, i * stride, stride);
                }

                return new RawImage
                {
                    width = jpeg.Width,
                    height = jpeg.Height,
                    data = data
                };
            }
        }

        public static Task<RawImage> DecodeResponseAsync(HttpWebResponse response)
        {
            return Task.Run(() => DecodeResponse(response));
        }

        public static RawImage DecodeResponse(HttpWebResponse response)
        {
            using (var stream = response.GetResponseStream())
            {
                if (response.ContentType == "image/jpeg")
                {
                    return DecodeJPEG(stream);
                }
                else if (response.ContentType == "imaage/png")
                {
                    return DecodePNG(stream);
                }
                else
                {
                    throw new FormatException($"Image format `{response.ContentType}` could not be decoded. Supported formats: JPEG, PNG. Source: {response.ResponseUri}");
                }
            }
        }
    }
}
