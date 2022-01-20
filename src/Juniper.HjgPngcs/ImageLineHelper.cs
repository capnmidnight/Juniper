using System;

using Hjg.Pngcs.Chunks;

namespace Hjg.Pngcs
{
    /// <summary>
    /// <para>Bunch of utility static methods to process/analyze an image line.</para>
    /// <para>Not essential at all, some methods are probably to be removed if future releases.</para>
    /// <para>TODO: document this better</para>
    /// </summary>
    public static class ImageLineHelper
    {
        /// <summary>
        /// Given an indexed line with a palette, unpacks as a RGB array
        /// </summary>
        /// <param name="line">ImageLine as returned from PngReader</param>
        /// <param name="pal">Palette chunk</param>
        /// <param name="trns">TRNS chunk (optional)</param>
        /// <param name="buf">Preallocated array, optional</param>
        /// <returns>R G B (one byte per sample)</returns>
        public static int[] Palette2rgb(ImageLine line, PngChunkPLTE pal, PngChunkTRNS trns, int[] buf)
        {
            if (line is null)
            {
                throw new ArgumentNullException(nameof(line));
            }

            if (pal is null)
            {
                throw new ArgumentNullException(nameof(pal));
            }

            var isalpha = trns is object;
            var channels = isalpha ? 4 : 3;
            var nsamples = line.ImgInfo.Cols * channels;
            if (buf is null || buf.Length < nsamples)
            {
                buf = new int[nsamples];
            }

            if (!line.SamplesUnpacked)
            {
                line = line.UnpackToNewImageLine();
            }

            var isbyte = line.SampleType == Hjg.Pngcs.ImageLine.ESampleType.BYTE;
            var nindexesWithAlpha = trns?.GetPalletteAlpha().Length ?? 0;
            for (var c = 0; c < line.ImgInfo.Cols; c++)
            {
                var index = isbyte ? (line.ScanlineB[c] & 0xFF) : line.Scanline[c];
                pal.GetEntryRgb(index, buf, c * channels);
                if (isalpha)
                {
                    buf[(c * channels) + 3] = index < nindexesWithAlpha ? trns.GetPalletteAlpha()[index] : 255;
                }
            }

            return buf;
        }

        public static int[] Palette2rgb(ImageLine line, PngChunkPLTE pal, int[] buf)
        {
            return Palette2rgb(line, pal, null, buf);
        }

        public static int ToARGB8(int r, int g, int b)
        {
            unchecked
            {
                return ((int)(0xFF000000)) | (r << 16) | (g << 8) | b;
            }
        }

        public static int ToARGB8(int r, int g, int b, int a)
        {
            return (a << 24) | (r << 16) | (g << 8) | b;
        }

        public static int ToARGB8(int[] buff, int offset, bool alpha)
        {
            if (buff is null)
            {
                throw new ArgumentNullException(nameof(buff));
            }

            return alpha
                ? ToARGB8(buff[offset++], buff[offset++], buff[offset++], buff[offset])
                : ToARGB8(buff[offset++], buff[offset++], buff[offset]);
        }

        public static int ToARGB8(byte[] buff, int offset, bool alpha)
        {
            if (buff is null)
            {
                throw new ArgumentNullException(nameof(buff));
            }

            return alpha
                ? ToARGB8(buff[offset++], buff[offset++], buff[offset++], buff[offset])
                : ToARGB8(buff[offset++], buff[offset++], buff[offset]);
        }

        public static void FromARGB8(int val, int[] buff, int offset, bool alpha)
        {
            if (buff is null)
            {
                throw new ArgumentNullException(nameof(buff));
            }

            buff[offset++] = ((val >> 16) & 0xFF);
            buff[offset++] = ((val >> 8) & 0xFF);
            buff[offset] = (val & 0xFF);
            if (alpha)
            {
                buff[offset + 1] = ((val >> 24) & 0xFF);
            }
        }

        public static void FromARGB8(int val, byte[] buff, int offset, bool alpha)
        {
            if (buff is null)
            {
                throw new ArgumentNullException(nameof(buff));
            }

            buff[offset++] = (byte)((val >> 16) & 0xFF);
            buff[offset++] = (byte)((val >> 8) & 0xFF);
            buff[offset] = (byte)(val & 0xFF);
            if (alpha)
            {
                buff[offset + 1] = (byte)((val >> 24) & 0xFF);
            }
        }

        public static int GetPixelToARGB8(ImageLine line, int column)
        {
            if (line is null)
            {
                throw new ArgumentNullException(nameof(line));
            }

            if (line.IsInt())
            {
                return ToARGB8(line.Scanline, column * line.channels, line.ImgInfo.Alpha);
            }
            else
            {
                return ToARGB8(line.ScanlineB, column * line.channels, line.ImgInfo.Alpha);
            }
        }

        public static void SetPixelFromARGB8(ImageLine line, int column, int argb)
        {
            if (line is null)
            {
                throw new ArgumentNullException(nameof(line));
            }

            if (line.IsInt())
            {
                FromARGB8(argb, line.Scanline, column * line.channels, line.ImgInfo.Alpha);
            }
            else
            {
                FromARGB8(argb, line.ScanlineB, column * line.channels, line.ImgInfo.Alpha);
            }
        }

        public static void SetPixel(ImageLine line, int col, int r, int g, int b, int a)
        {
            if (line is null)
            {
                throw new ArgumentNullException(nameof(line));
            }

            var offset = col * line.channels;
            if (line.IsInt())
            {
                line.Scanline[offset++] = r;
                line.Scanline[offset++] = g;
                line.Scanline[offset] = b;
                if (line.ImgInfo.Alpha)
                {
                    line.Scanline[offset + 1] = a;
                }
            }
            else
            {
                line.ScanlineB[offset++] = (byte)r;
                line.ScanlineB[offset++] = (byte)g;
                line.ScanlineB[offset] = (byte)b;
                if (line.ImgInfo.Alpha)
                {
                    line.ScanlineB[offset + 1] = (byte)a;
                }
            }
        }

        public static void SetPixel(ImageLine line, int col, int r, int g, int b)
        {
            if (line is null)
            {
                throw new ArgumentNullException(nameof(line));
            }

            SetPixel(line, col, r, g, b, line.MaxSampleVal);
        }

        public static double ReadDouble(ImageLine line, int pos)
        {
            if (line is null)
            {
                throw new ArgumentNullException(nameof(line));
            }

            if (line.IsInt())
            {
                return line.Scanline[pos] / (line.MaxSampleVal + 0.9);
            }
            else
            {
                return line.ScanlineB[pos] / (line.MaxSampleVal + 0.9);
            }
        }

        public static void WriteDouble(ImageLine line, double d, int pos)
        {
            if (line is null)
            {
                throw new ArgumentNullException(nameof(line));
            }

            if (line.IsInt())
            {
                line.Scanline[pos] = (int)(d * (line.MaxSampleVal + 0.99));
            }
            else
            {
                line.ScanlineB[pos] = (byte)(d * (line.MaxSampleVal + 0.99));
            }
        }

        public static int Interpol(int a, int b, int c, int d, double dx, double dy)
        {
            // a b -> x (0-1)
            // c d
            var e = (a * (1.0 - dx)) + (b * dx);
            var f = (c * (1.0 - dx)) + (d * dx);
            return (int)((e * (1 - dy)) + (f * dy) + 0.5);
        }

        public static int ClampTo_0_255(int i)
        {
            if (i > 255)
            {
                return 255;
            }
            else if (i < 0)
            {
                return 0;
            }
            else
            {
                return i;
            }
        }

        /**
         * [0,1)
         */

        public static double ClampDouble(double i)
        {
            if (i < 0)
            {
                return 0;
            }
            else if (i >= 1)
            {
                return 0.999999;
            }
            else
            {
                return i;
            }
        }

        public static int ClampTo_0_65535(int i)
        {
            if (i > 65535)
            {
                return 65535;
            }
            else if (i < 0)
            {
                return 0;
            }
            else
            {
                return i;
            }
        }

        public static int ClampTo_128_127(int x)
        {
            if (x > 127)
            {
                return 127;
            }
            else if (x < -128)
            {
                return -128;
            }
            else
            {
                return x;
            }
        }

        public static int[] Unpack(ImageInfo imgInfo, int[] src, int[] dst, bool scale)
        {
            if (imgInfo is null)
            {
                throw new ArgumentNullException(nameof(imgInfo));
            }

            if (src is null)
            {
                throw new ArgumentNullException(nameof(src));
            }

            var len1 = imgInfo.SamplesPerRow;
            var len0 = imgInfo.SamplesPerRowPacked;
            if (dst is null || dst.Length < len1)
            {
                dst = new int[len1];
            }

            if (imgInfo.Packed)
            {
                ImageLine.UnpackInplaceInt(imgInfo, src, dst, scale);
            }
            else
            {
                Array.Copy(src, 0, dst, 0, len0);
            }

            return dst;
        }

        public static byte[] Unpack(ImageInfo imgInfo, byte[] src, byte[] dst, bool scale)
        {
            if (imgInfo is null)
            {
                throw new ArgumentNullException(nameof(imgInfo));
            }

            if (src is null)
            {
                throw new ArgumentNullException(nameof(src));
            }

            var len1 = imgInfo.SamplesPerRow;
            var len0 = imgInfo.SamplesPerRowPacked;
            if (dst is null || dst.Length < len1)
            {
                dst = new byte[len1];
            }

            if (imgInfo.Packed)
            {
                ImageLine.UnpackInplaceByte(imgInfo, src, dst, scale);
            }
            else
            {
                Array.Copy(src, 0, dst, 0, len0);
            }

            return dst;
        }

        public static int[] Pack(ImageInfo imgInfo, int[] src, int[] dst, bool scale)
        {
            if (imgInfo is null)
            {
                throw new ArgumentNullException(nameof(imgInfo));
            }

            if (src is null)
            {
                throw new ArgumentNullException(nameof(src));
            }

            var len0 = imgInfo.SamplesPerRowPacked;
            if (dst is null || dst.Length < len0)
            {
                dst = new int[len0];
            }

            if (imgInfo.Packed)
            {
                ImageLine.PackInplaceInt(imgInfo, src, dst, scale);
            }
            else
            {
                Array.Copy(src, 0, dst, 0, len0);
            }

            return dst;
        }

        public static byte[] Pack(ImageInfo imgInfo, byte[] src, byte[] dst, bool scale)
        {
            if (imgInfo is null)
            {
                throw new ArgumentNullException(nameof(imgInfo));
            }

            if (src is null)
            {
                throw new ArgumentNullException(nameof(src));
            }

            var len0 = imgInfo.SamplesPerRowPacked;
            if (dst is null || dst.Length < len0)
            {
                dst = new byte[len0];
            }

            if (imgInfo.Packed)
            {
                ImageLine.PackInplaceByte(imgInfo, src, dst, scale);
            }
            else
            {
                Array.Copy(src, 0, dst, 0, len0);
            }

            return dst;
        }
    }
}