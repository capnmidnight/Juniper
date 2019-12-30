using System.IO;

using BitMiracle.LibJpeg.Classic;

namespace BitMiracle.LibJpeg
{

    /// <summary>
    /// Holds parameters of image for decompression (IDecomressDesination)
    /// </summary>
    internal class LoadedImageAttributes
    {
        /* Decompression processing parameters --- these fields must be set before
         * calling jpeg_start_decompress().  Note that jpeg_read_header() initializes
         * them to default values.
         */

        // colorspace for output
        public Colorspace Colorspace { get; internal set; }

        // true=colormapped output wanted
        public bool QuantizeColors { get; internal set; }

        /* Description of actual output image that will be returned to application.
         * These fields are computed by jpeg_start_decompress().
         * You can also use jpeg_calc_output_dimensions() to determine these values
         * in advance of calling jpeg_start_decompress().
         */

        // scaled image width
        public int Width { get; internal set; }

        // scaled image height
        public int Height { get; internal set; }

        // # of color components in out_color_space
        public int ComponentsPerSample { get; internal set; }

        // # of color components returned. it is 1 (a colormap index) when 
        // quantizing colors; otherwise it equals out_color_components.
        public int Components { get; internal set; }

        /* When quantizing colors, the output colormap is described by these fields.
         * The application can supply a colormap by setting colormap non-null before
         * calling jpeg_start_decompress; otherwise a colormap is created during
         * jpeg_start_decompress or jpeg_start_output.
         * The map has out_color_components rows and actual_number_of_colors columns.
         */

        // number of entries in use
        public int ActualNumberOfColors { get; internal set; }

        // The color map as a 2-D pixel array
        public byte[][] Colormap { get; internal set; }

        // These fields record data obtained from optional markers 
        // recognized by the JPEG library.

        // JFIF code for pixel size units
        public DensityUnit DensityUnit { get; internal set; }

        // Horizontal pixel density
        public int DensityX { get; internal set; }

        // Vertical pixel density
        public int DensityY { get; internal set; }
    }
}
