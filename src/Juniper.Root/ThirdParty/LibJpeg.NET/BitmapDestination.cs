using System;
using System.IO;

using BitMiracle.LibJpeg.Classic;

namespace BitMiracle.LibJpeg
{
    internal class BitmapDestination : IDecompressDestination
    {
        private byte[][] m_pixels;

        private int m_rowWidth;       /* physical width of one row in the BMP file */

        private int m_currentRow;  /* next row# to write to virtual array */
        private LoadedImageAttributes m_parameters;

        public BitmapDestination(Stream output)
        {
            Output = output;
        }

        public Stream Output { get; }

        public void SetImageAttributes(LoadedImageAttributes parameters)
        {
            m_parameters = parameters ?? throw new ArgumentNullException(nameof(parameters));
        }

        /// <summary>
        /// Startup: normally writes the file header.
        /// In this module we may as well postpone everything until finish_output.
        /// </summary>
        public void BeginWrite()
        {
            //Determine width of rows in the BMP file (padded to 4-byte boundary).
            m_rowWidth = m_parameters.Width * m_parameters.Components;
            while (m_rowWidth % 4 != 0)
            {
                m_rowWidth++;
            }

            m_pixels = new byte[m_rowWidth][];
            for (var i = 0; i < m_rowWidth; i++)
            {
                m_pixels[i] = new byte[m_parameters.Height];
            }

            m_currentRow = 0;
        }

        /// <summary>
        /// Write some pixel data.
        /// </summary>
        public void ProcessPixelsRow(byte[] row)
        {
            if (m_parameters.Colorspace == Colorspace.Grayscale || m_parameters.QuantizeColors)
            {
                PutGrayRow(row);
            }
            else
            {
                if (m_parameters.Colorspace == Colorspace.CMYK)
                {
                    PutCmykRow(row);
                }
                else
                {
                    PutRgbRow(row);
                }
            }

            ++m_currentRow;
        }

        /// <summary>
        /// Finish up at the end of the file.
        /// Here is where we really output the BMP file.
        /// </summary>
        public void EndWrite()
        {
            WriteHeader();
            WritePixels();

            /* Make sure we wrote the output file OK */
            Output.Flush();
        }

        /// <summary>
        /// This version is for grayscale OR quantized color output
        /// </summary>
        private void PutGrayRow(byte[] row)
        {
            for (var i = 0; i < m_parameters.Width; ++i)
            {
                m_pixels[i][m_currentRow] = row[i];
            }
        }

        /// <summary>
        /// This version is for writing 24-bit pixels
        /// </summary>
        private void PutRgbRow(byte[] row)
        {
            /* Transfer data.  Note destination values must be in BGR order
             * (even though Microsoft's own documents say the opposite).
             */
            for (var i = 0; i < m_parameters.Width; ++i)
            {
                var firstComponent = i * 3;
                var red = row[firstComponent];
                var green = row[firstComponent + 1];
                var blue = row[firstComponent + 2];
                m_pixels[firstComponent][m_currentRow] = blue;
                m_pixels[firstComponent + 1][m_currentRow] = green;
                m_pixels[firstComponent + 2][m_currentRow] = red;
            }
        }

        /// <summary>
        /// This version is for writing 24-bit pixels
        /// </summary>
        private void PutCmykRow(byte[] row)
        {
            /* Transfer data.  Note destination values must be in BGR order
             * (even though Microsoft's own documents say the opposite).
             */
            for (var i = 0; i < m_parameters.Width; ++i)
            {
                var firstComponent = i * 4;
                m_pixels[firstComponent][m_currentRow] = row[firstComponent + 2];
                m_pixels[firstComponent + 1][m_currentRow] = row[firstComponent + 1];
                m_pixels[firstComponent + 2][m_currentRow] = row[firstComponent + 0];
                m_pixels[firstComponent + 3][m_currentRow] = row[firstComponent + 3];
            }
        }

        /// <summary>
        /// Write a Windows-style BMP file header, including colormap if needed
        /// </summary>
        private void WriteHeader()
        {
            int bits_per_pixel;
            int cmap_entries;

            /* Compute colormap size and total file size */
            if (m_parameters.Colorspace == Colorspace.Grayscale || m_parameters.QuantizeColors)
            {
                bits_per_pixel = 8;
                cmap_entries = 256;
            }
            else
            {
                cmap_entries = 0;

                if (m_parameters.Colorspace == Colorspace.RGB)
                {
                    bits_per_pixel = 24;
                }
                else if (m_parameters.Colorspace == Colorspace.CMYK)
                {
                    bits_per_pixel = 32;
                }
                else
                {
                    throw new InvalidOperationException();
                }
            }

            byte[] infoHeader;
            if (m_parameters.Colorspace == Colorspace.RGB)
            {
                infoHeader = CreateBitmapInfoHeader(bits_per_pixel, cmap_entries);
            }
            else
            {
                infoHeader = CreateBitmapV4InfoHeader(bits_per_pixel);
            }

            /* File size */
            const int fileHeaderSize = 14;
            var infoHeaderSize = infoHeader.Length;
            var paletteSize = cmap_entries * 4;
            var offsetToPixels = fileHeaderSize + infoHeaderSize + paletteSize; /* Header and colormap */
            var fileSize = offsetToPixels + (m_rowWidth * m_parameters.Height);

            var fileHeader = CreateBitmapFileHeader(offsetToPixels, fileSize);

            Output.Write(fileHeader, 0, fileHeader.Length);
            Output.Write(infoHeader, 0, infoHeader.Length);

            if (cmap_entries > 0)
            {
                WriteColormap(cmap_entries, 4);
            }
        }

        private static byte[] CreateBitmapFileHeader(int offsetToPixels, int fileSize)
        {
            var bmpfileheader = new byte[14];
            bmpfileheader[0] = 0x42;    /* first 2 bytes are ASCII 'B', 'M' */
            bmpfileheader[1] = 0x4D;
            PUT_4B(bmpfileheader, 2, fileSize);
            /* we leave bfReserved1 & bfReserved2 = 0 */
            PUT_4B(bmpfileheader, 10, offsetToPixels); /* bfOffBits */
            return bmpfileheader;
        }

        private byte[] CreateBitmapInfoHeader(int bits_per_pixel, int cmap_entries)
        {
            var bmpinfoheader = new byte[40];
            FillBitmapInfoHeader(bits_per_pixel, cmap_entries, bmpinfoheader);
            return bmpinfoheader;
        }

        private void FillBitmapInfoHeader(int bitsPerPixel, int cmap_entries, byte[] infoHeader)
        {
            /* Fill the info header (Microsoft calls this a BITMAPINFOHEADER) */
            PUT_2B(infoHeader, 0, infoHeader.Length);   /* biSize */
            PUT_4B(infoHeader, 4, m_parameters.Width); /* biWidth */
            PUT_4B(infoHeader, 8, m_parameters.Height); /* biHeight */
            PUT_2B(infoHeader, 12, 1);   /* biPlanes - must be 1 */
            PUT_2B(infoHeader, 14, bitsPerPixel); /* biBitCount */
            /* we leave biCompression = 0, for none */
            /* we leave biSizeImage = 0; this is correct for uncompressed data */

            if (m_parameters.DensityUnit == DensityUnit.DotsCm)
            {
                /* if have density in dots/cm, then */
                PUT_4B(infoHeader, 24, m_parameters.DensityX * 100); /* XPels/M */
                PUT_4B(infoHeader, 28, m_parameters.DensityY * 100); /* XPels/M */
            }
            PUT_2B(infoHeader, 32, cmap_entries); /* biClrUsed */
            /* we leave biClrImportant = 0 */
        }

        private byte[] CreateBitmapV4InfoHeader(int bitsPerPixel)
        {
            var infoHeader = new byte[40 + 68];
            FillBitmapInfoHeader(bitsPerPixel, 0, infoHeader);

            PUT_4B(infoHeader, 56, 0x02); /* CSType == 0x02 (CMYK) */

            return infoHeader;
        }

        /// <summary>
        /// Write the colormap.
        /// Windows uses BGR0 map entries; OS/2 uses BGR entries.
        /// </summary>
        private void WriteColormap(int map_colors, int map_entry_size)
        {
            var colormap = m_parameters.Colormap;
            var num_colors = m_parameters.ActualNumberOfColors;

            int i;
            if (colormap != null)
            {
                if (m_parameters.ComponentsPerSample == 3)
                {
                    /* Normal case with RGB colormap */
                    for (i = 0; i < num_colors; i++)
                    {
                        Output.WriteByte(colormap[2][i]);
                        Output.WriteByte(colormap[1][i]);
                        Output.WriteByte(colormap[0][i]);
                        if (map_entry_size == 4)
                        {
                            Output.WriteByte(0);
                        }
                    }
                }
                else
                {
                    /* Grayscale colormap (only happens with grayscale quantization) */
                    for (i = 0; i < num_colors; i++)
                    {
                        Output.WriteByte(colormap[0][i]);
                        Output.WriteByte(colormap[0][i]);
                        Output.WriteByte(colormap[0][i]);
                        if (map_entry_size == 4)
                        {
                            Output.WriteByte(0);
                        }
                    }
                }
            }
            else
            {
                /* If no colormap, must be grayscale data.  Generate a linear "map". */
                for (i = 0; i < 256; i++)
                {
                    Output.WriteByte((byte)i);
                    Output.WriteByte((byte)i);
                    Output.WriteByte((byte)i);
                    if (map_entry_size == 4)
                    {
                        Output.WriteByte(0);
                    }
                }
            }

            /* Pad colormap with zeros to ensure specified number of colormap entries */
            if (i > map_colors)
            {
                throw new InvalidOperationException("Too many colors");
            }

            for (; i < map_colors; i++)
            {
                Output.WriteByte(0);
                Output.WriteByte(0);
                Output.WriteByte(0);
                if (map_entry_size == 4)
                {
                    Output.WriteByte(0);
                }
            }
        }

        private void WritePixels()
        {
            for (var row = m_parameters.Height - 1; row >= 0; --row)
            {
                for (var col = 0; col < m_rowWidth; ++col)
                {
                    Output.WriteByte(m_pixels[col][row]);
                }
            }
        }

        private static void PUT_2B(byte[] array, int offset, int value)
        {
            array[offset] = (byte)(value & 0xFF);
            array[offset + 1] = (byte)((value >> 8) & 0xFF);
        }

        private static void PUT_4B(byte[] array, int offset, int value)
        {
            array[offset] = (byte)(value & 0xFF);
            array[offset + 1] = (byte)((value >> 8) & 0xFF);
            array[offset + 2] = (byte)((value >> 16) & 0xFF);
            array[offset + 3] = (byte)((value >> 24) & 0xFF);
        }
    }
}
