using System;
using System.Diagnostics;
using System.IO;

using BitMiracle.LibJpeg.Classic;

namespace BitMiracle.LibJpeg
{
    /// <summary>
    /// Internal wrapper for classic jpeg compressor and decompressor
    /// </summary>
    internal class Jpeg
    {
        private CompressionParameters m_compressionParameters = new CompressionParameters();
        private DecompressionParameters m_decompressionParameters = new DecompressionParameters();

        /// <summary>
        /// Advanced users may set specific parameters of compression
        /// </summary>
        public CompressionParameters CompressionParameters
        {
            get { return m_compressionParameters; }
            set { m_compressionParameters = value ?? throw new ArgumentNullException(nameof(value)); }
        }

        /// <summary>
        /// Advanced users may set specific parameters of decompression
        /// </summary>
        public DecompressionParameters DecompressionParameters
        {
            get { return m_decompressionParameters; }
            set { m_decompressionParameters = value ?? throw new ArgumentNullException(nameof(value)); }
        }

        /// <summary>
        /// Compresses any image described as ICompressSource to JPEG
        /// </summary>
        /// <param name="source">Contains description of input image</param>
        /// <param name="output">Stream for output of compressed JPEG</param>
        public void Compress(IRawImage source, Stream output)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (output == null)
            {
                throw new ArgumentNullException(nameof(output));
            }

            ClassicCompressor.Image_width = source.Width;
            ClassicCompressor.Image_height = source.Height;
            ClassicCompressor.In_color_space = (JColorSpace)source.Colorspace;
            ClassicCompressor.Input_components = source.ComponentsPerPixel;
            //m_compressor.Data_precision = source.DataPrecision;

            ClassicCompressor.JpegSetDefaults();

            //we need to set density parameters after setting of default jpeg parameters
            //m_compressor.Density_unit = source.DensityUnit;
            //m_compressor.X_density = (short)source.DensityX;
            //m_compressor.Y_density = (short)source.DensityY;

            applyParameters(m_compressionParameters);

            // Specify data destination for compression
            ClassicCompressor.JpegStdioDest(output);

            // Start compression
            ClassicCompressor.JpegStartCompression(true);

            // Process  pixels
            source.BeginRead();
            while (ClassicCompressor.Next_scanline < ClassicCompressor.Image_height)
            {
                var row = source.GetPixelRow();
                var rowForDecompressor = new byte[1][];
                rowForDecompressor[0] = row ?? throw new InvalidDataException("Row of pixels is null");
                ClassicCompressor.JpegWriteScanlines(rowForDecompressor, 1);
            }

            source.EndRead();

            // Finish compression and release memory
            ClassicCompressor.JpegFinishCompress();
        }

        /// <summary>
        /// Decompresses JPEG image to any image described as ICompressDestination
        /// </summary>
        /// <param name="jpeg">Stream with JPEG data</param>
        /// <param name="destination">Stream for output of compressed JPEG</param>
        public void Decompress(Stream jpeg, IDecompressDestination destination)
        {
            if (jpeg == null)
            {
                throw new ArgumentNullException(nameof(jpeg));
            }

            if (destination == null)
            {
                throw new ArgumentNullException(nameof(destination));
            }

            beforeDecompress(jpeg);

            // Start decompression
            ClassicDecompressor.JpegStartDecompress();

            var parameters = getImageParametersFromDecompressor();
            destination.SetImageAttributes(parameters);
            destination.BeginWrite();

            /* Process data */
            while (ClassicDecompressor.Output_scanline < ClassicDecompressor.Output_height)
            {
                var row = JpegCommonStruct.AllocJpegSamples(ClassicDecompressor.Output_width * ClassicDecompressor.Output_components, 1);
                ClassicDecompressor.JpegReadScanlines(row, 1);
                destination.ProcessPixelsRow(row[0]);
            }

            destination.EndWrite();

            // Finish decompression and release memory.
            ClassicDecompressor.JpegFinishDecompress();
        }

        /// <summary>
        /// Tunes decompressor
        /// </summary>
        /// <param name="jpeg">Stream with input compressed JPEG data</param>
        private void beforeDecompress(Stream jpeg)
        {
            ClassicDecompressor.JpegStdioSrc(jpeg);
            /* Read file header, set default decompression parameters */
            ClassicDecompressor.JpegReadHeader(true);

            applyParameters(m_decompressionParameters);
            ClassicDecompressor.JpegCalcOutputDimensions();
        }

        private LoadedImageAttributes getImageParametersFromDecompressor()
        {
            return new LoadedImageAttributes
            {
                Colorspace = (Colorspace)ClassicDecompressor.Out_color_space,
                QuantizeColors = ClassicDecompressor.Quantize_colors,
                Width = ClassicDecompressor.Output_width,
                Height = ClassicDecompressor.Output_height,
                ComponentsPerSample = ClassicDecompressor.Out_color_components,
                Components = ClassicDecompressor.Output_components,
                ActualNumberOfColors = ClassicDecompressor.ActualNumberOfColors,
                Colormap = ClassicDecompressor.Colormap,
                DensityUnit = ClassicDecompressor.DensityUnit,
                DensityX = ClassicDecompressor.XDensity,
                DensityY = ClassicDecompressor.YDensity
            };
        }

        public JpegCompressStruct ClassicCompressor { get; } = new JpegCompressStruct(new JpegErrorMessage());

        public JpegDecompressStruct ClassicDecompressor { get; } = new JpegDecompressStruct(new JpegErrorMessage());

        /// <summary>
        /// Delegate for application-supplied marker processing methods.
        /// Need not pass marker code since it is stored in cinfo.unread_marker.
        /// </summary>
        public delegate bool MarkerParser(Jpeg decompressor);

        /* Install a special processing method for COM or APPn markers. */
        public void SetMarkerProcessor(int markerCode, MarkerParser routine)
        {
            bool f(JpegDecompressStruct _)
            {
                return routine(this);
            }

            ClassicDecompressor.JpegSetMarkerProcessor(markerCode, f);
        }

        private void applyParameters(DecompressionParameters parameters)
        {
            Debug.Assert(parameters != null);

            if (parameters.OutColorspace != Colorspace.Unknown)
            {
                ClassicDecompressor.Out_color_space = (JColorSpace)parameters.OutColorspace;
            }

            ClassicDecompressor.Scale_num = parameters.ScaleNumerator;
            ClassicDecompressor.Scale_denom = parameters.ScaleDenominator;
            ClassicDecompressor.Buffered_image = parameters.BufferedImage;
            ClassicDecompressor.Raw_data_out = parameters.RawDataOut;
            ClassicDecompressor.Dct_method = (JDctMethod)parameters.DCTMethod;
            ClassicDecompressor.Dither_mode = (JDitherMode)parameters.DitherMode;
            ClassicDecompressor.Do_fancy_upsampling = parameters.DoFancyUpsampling;
            ClassicDecompressor.Do_block_smoothing = parameters.DoBlockSmoothing;
            ClassicDecompressor.Quantize_colors = parameters.QuantizeColors;
            ClassicDecompressor.Two_pass_quantize = parameters.TwoPassQuantize;
            ClassicDecompressor.Desired_number_of_colors = parameters.DesiredNumberOfColors;
            ClassicDecompressor.Enable_1pass_quant = parameters.EnableOnePassQuantizer;
            ClassicDecompressor.Enable_external_quant = parameters.EnableExternalQuant;
            ClassicDecompressor.Enable_2pass_quant = parameters.EnableTwoPassQuantizer;
            ClassicDecompressor.Err.TraceLevel = parameters.TraceLevel;
        }

        private void applyParameters(CompressionParameters parameters)
        {
            Debug.Assert(parameters != null);

            ClassicCompressor.SmoothingFactor = parameters.SmoothingFactor;
            ClassicCompressor.JpegSetQuality(parameters.Quality, true);
            if (parameters.SimpleProgressive)
            {
                ClassicCompressor.JpegSimpleProgression();
            }
        }
    }
}
