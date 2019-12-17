using BitMiracle.LibJpeg.Classic;

namespace BitMiracle.LibJpeg
{
    internal class DecompressionParameters
    {
        public int TraceLevel { get; set; }

        /* Decompression processing parameters --- these fields must be set before
         * calling jpeg_start_decompress().  Note that jpeg_read_header() initializes
         * them to default values.
         */

        // colorspace for output
        public Colorspace OutColorspace { get; set; }

        // fraction by which to scale image
        public int ScaleNumerator { get; set; } = 1;

        public int ScaleDenominator { get; set; } = 1;

        // true=multiple output passes
        public bool BufferedImage { get; set; }

        // true=downsampled data wanted
        public bool RawDataOut { get; set; }

        // IDCT algorithm selector
        public DCTMethod DCTMethod { get; set; }

        // true=apply fancy upsampling
        public bool DoFancyUpsampling { get; set; } = true;

        // true=apply interblock smoothing
        public bool DoBlockSmoothing { get; set; } = true;

        // true=colormapped output wanted
        public bool QuantizeColors { get; set; }

        /* the following are ignored if not quantize_colors: */

        // type of color dithering to use
        public DitherMode DitherMode { get; set; } = DitherMode.FloydSteinberg;

        // true=use two-pass color quantization
        public bool TwoPassQuantize { get; set; } = true;

        // max # colors to use in created colormap
        public int DesiredNumberOfColors { get; set; } = 256;

        /* these are significant only in buffered-image mode: */

        // enable future use of 1-pass quantizer
        public bool EnableOnePassQuantizer { get; set; }

        // enable future use of external colormap
        public bool EnableExternalQuant { get; set; }

        // enable future use of 2-pass quantizer
        public bool EnableTwoPassQuantizer { get; set; }
    }
}
