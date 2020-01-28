using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

using BitMiracle.LibJpeg.Classic.Internal;

namespace BitMiracle.LibJpeg.Classic
{
    /// <summary>
    /// JPEG decompression routine.
    /// </summary>
    /// <seealso cref="JpegCompressStruct"/>
    public class JpegDecompressStruct : JpegCommonStruct
    {
        /// <summary>
        /// The delegate for application-supplied marker processing methods.
        /// </summary>
        /// <param name="cinfo">Decompressor.</param>
        /// <returns>Return <c>true</c> to indicate success. <c>false</c> should be returned only 
        /// if you are using a suspending data source and it tells you to suspend.
        /// </returns>
        /// <remarks>Although the marker code is not explicitly passed, the routine can find it 
        /// in the <see cref="JpegDecompressStruct.UnreadMarker"/>. At the time of call, 
        /// the marker proper has been read from the data source module. The processor routine 
        /// is responsible for reading the marker length word and the remaining parameter bytes, if any.
        /// </remarks>
        public delegate bool JpegMarkerParserMethod(JpegDecompressStruct cinfo);

        /* Source of compressed data */
        internal JpegSourceManager m_src;

        internal int imageWidth; /* nominal image width (from SOF marker) */
        internal int imageHeight;    /* nominal image height */
        internal int numComponents;     /* # of color components in JPEG image */
        internal JColorSpace jpegColorSpace; /* colorspace of JPEG image */

        internal JColorSpace outColorSpace; /* colorspace for output */
        internal int scaleNum;
        internal int scaleDenom; /* fraction by which to scale image */
        internal bool bufferedImage;    /* true=multiple output passes */
        internal bool rawDataOut;      /* true=downsampled data wanted */
        internal JDctMethod dctMethod;    /* IDCT algorithm selector */
        internal bool doFancyUpsampling;   /* true=apply fancy upsampling */
        internal bool doBlockSmoothing;    /* true=apply interblock smoothing */
        internal bool quantizeColors;   /* true=colormapped output wanted */
        internal JDitherMode ditherMode;  /* type of color dithering to use */
        internal bool twoPassQuantize; /* true=use two-pass color quantization */
        internal int desiredNumberOfColors;   /* max # colors to use in created colormap */
        internal bool enable1PassQuant;    /* enable future use of 1-pass quantizer */
        internal bool enableExternalQuant;/* enable future use of external colormap */
        internal bool enable2PassQuant;    /* enable future use of 2-pass quantizer */

        internal int outputWidth;    /* scaled image width */
        internal int outputHeight;   /* scaled image height */
        internal int outColorComponents;   /* # of color components in out_color_space */
        /* # of color components returned
         * output_components is 1 (a colormap index) when quantizing colors;
         * otherwise it equals out_color_components.
         */
        internal int outputComponents;

        internal int recommendOutBufferHeight;  /* min recommended height of scanline buffer */

        internal int actualColorCount;    /* number of entries in use */
        internal byte[][] m_colormap;     /* The color map as a 2-D pixel array */

        internal int outputScanline; /* 0 .. output_height-1  */

        internal int inputScanNumber;  /* Number of SOS markers seen so far */
        internal int inputIMcuRow;  /* Number of iMCU rows completed */

        internal int outputScanNumber; /* Nominal scan number being displayed */
        internal int outputIMcuRow; /* Number of iMCU rows read */

        internal int[][] coefBits; /* -1 or current Al value for each coef */

        /* Internal JPEG parameters --- the application usually need not look at
         * these fields.  Note that the decompressor output side may not use
         * any parameters that can change between scans.
         */

        /* Quantization and Huffman tables are carried forward across input
         * datastreams when processing abbreviated JPEG datastreams.
         */

        internal JQuantTable[] m_quant_tbl_ptrs = new JQuantTable[JpegConstants.NUM_QUANT_TBLS];
        /* ptrs to coefficient quantization tables, or null if not defined */

        internal JHuffmanTable[] m_dc_huff_tbl_ptrs = new JHuffmanTable[JpegConstants.NUM_HUFF_TBLS];
        internal JHuffmanTable[] m_ac_huff_tbl_ptrs = new JHuffmanTable[JpegConstants.NUM_HUFF_TBLS];
        /* ptrs to Huffman coding tables, or null if not defined */

        /* These parameters are never carried across datastreams, since they
         * are given in SOF/SOS markers or defined to be reset by SOI.
         */

        internal int m_dataPrecision;     /* bits of precision in image data */

        internal bool isBaseline;        /* TRUE if Baseline SOF0 encountered */
        internal bool progressiveMode;  /* true if SOFn specifies progressive mode */
        internal bool arithCode;     /* TRUE=arithmetic coding, FALSE=Huffman */

        internal byte[] arithDcL = new byte[JpegConstants.NUM_ARITH_TBLS]; /* L values for DC arith-coding tables */
        internal byte[] arithDcU = new byte[JpegConstants.NUM_ARITH_TBLS]; /* U values for DC arith-coding tables */
        internal byte[] arithAcK = new byte[JpegConstants.NUM_ARITH_TBLS]; /* Kx values for AC arith-coding tables */

        internal int m_restart_interval; /* MCUs per restart interval, or 0 for no restart */

        /* These fields record data obtained from optional markers recognized by
         * the JPEG library.
         */
        internal bool m_saw_JFIF_marker;   /* true iff a JFIF APP0 marker was found */
        /* Data copied from JFIF marker; only valid if saw_JFIF_marker is true: */
        internal byte m_JFIF_major_version;   /* JFIF version number */
        internal byte m_JFIF_minor_version;

        internal DensityUnit m_densityUnit;     /* JFIF code for pixel size units */
        internal short m_xDensity;       /* Horizontal pixel density */
        internal short m_yDensity;       /* Vertical pixel density */

        internal bool m_saw_Adobe_marker;  /* true iff an Adobe APP14 marker was found */
        internal byte m_Adobe_transform;  /* Color transform code from Adobe marker */

        internal JColorTransform color_transform;
        /* Color transform identifier derived from LSE marker, otherwise zero */

        internal bool m_CCIR601_sampling;  /* true=first samples are cosited */

        internal List<JpegMarkerStruct> m_marker_list; /* Head of list of saved markers */

        /* Remaining fields are known throughout decompressor, but generally
         * should not be touched by a surrounding application.
         */

        /*
         * These fields are computed during decompression startup
         */
        internal int m_max_h_samp_factor;  /* largest h_samp_factor */
        internal int m_maxVSampleFactor;  /* largest v_samp_factor */

        internal int min_DCT_h_scaled_size;  /* smallest DCT_h_scaled_size of any component */
        internal int min_DCT_v_scaled_size;  /* smallest DCT_v_scaled_size of any component */

        internal int m_total_iMCU_rows; /* # of iMCU rows in image */
        /* The coefficient controller's input and output progress is measured in
         * units of "iMCU" (interleaved MCU) rows.  These are the same as MCU rows
         * in fully interleaved JPEG scans, but are used whether the scan is
         * interleaved or not.  We define an iMCU row as v_samp_factor DCT block
         * rows of each component.  Therefore, the IDCT output contains
         * v_samp_factor*DCT_v_scaled_size sample rows of a component per iMCU row.
         */

        internal byte[] m_sample_range_limit; /* table for fast range-limiting */
        internal int m_sampleRangeLimitOffset;

        /*
         * These fields are valid during any one scan.
         * They describe the components and MCUs actually appearing in the scan.
         * Note that the decompressor output side must not use these fields.
         */
        internal int m_comps_in_scan;      /* # of JPEG components in this scan */
        internal int[] m_cur_comp_info = new int[JpegConstants.MAX_COMPS_IN_SCAN];
        /* *cur_comp_info[i] describes component that appears i'th in SOS */

        internal int m_MCUs_per_row;    /* # of MCUs across the image */
        internal int m_MCU_rows_in_scan;    /* # of MCU rows in the image */

        internal int m_blocks_in_MCU;      /* # of DCT blocks per MCU */
        internal int[] m_MCU_membership = new int[JpegConstants.D_MAX_BLOCKS_IN_MCU];
        /* MCU_membership[i] is index in cur_comp_info of component owning */
        /* i'th block in an MCU */

        /* progressive JPEG parameters for scan */
        internal int m_Ss;
        internal int m_Se;
        internal int m_Ah;
        internal int m_Al;

        /* These fields are derived from Se of first SOS marker. */
        internal int block_size;     /* the basic DCT block size: 1..16 */
        internal int[] natural_order; /* natural-order position array for entropy decode */
        internal int lim_Se;            /* min( Se, DCTSIZE2-1 ) for entropy decode */

        /* This field is shared between entropy decoder and marker parser.
         * It is either zero or the code of a JPEG marker that has been
         * read from the data source, but has not yet been processed.
         */
        internal int m_unreadMarker;

        /*
         * Links to decompression subobjects (methods, private variables of modules)
         */
        internal JpegDecompMaster m_master;
        internal JpegDMainController m_main;
        internal JpegDCoefController m_coef;
        internal JpegDPostController m_post;
        internal JpegInputController m_inputctl;
        internal JpegMarkerReader m_marker;
        internal JpegEntropyDecoder m_entropy;
        internal JpegInverseDct m_idct;
        internal JpegUpSampler m_upsample;
        internal JpegColorDeconverter m_cconvert;
        internal IJpegColorQuantizer m_cquantize;

        /// <summary>
        /// Initializes a new instance of the <see cref="JpegDecompressStruct"/> class.
        /// </summary>
        /// <seealso cref="JpegCompressStruct"/>
        public JpegDecompressStruct()
        {
            Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JpegDecompressStruct"/> class.
        /// </summary>
        /// <param name="errorManager">The error manager.</param>
        /// <seealso cref="JpegCompressStruct"/>
        public JpegDecompressStruct(JpegErrorMessage errorManager)
            : base(errorManager)
        {
            Initialize();
        }

        /// <summary>
        /// Retrieves <c>true</c> because this is a decompressor.
        /// </summary>
        /// <value><c>true</c></value>
        public override bool IsDecompressor => true;

        /// <summary>
        /// Gets or sets the source for decompression.
        /// </summary>
        /// <value>The source for decompression.</value>
        public LibJpeg.Classic.JpegSourceManager Src
        {
            get { return m_src; }
            set { m_src = value; }
        }

        /* Basic description of image --- filled in by jpeg_read_header(). */
        /* Application may inspect these values to decide how to process image. */

        /// <summary>
        /// Gets the width of image, set by <see cref="JpegDecompressStruct.jpeg_read_header"/>
        /// </summary>
        /// <value>The width of image.</value>
        /// <seealso href="../articles/KB/decompression-parameter-selection.html">Decompression parameter selection</seealso>
        public int Image_width => imageWidth;

        /// <summary>
        /// Gets the height of image, set by <see cref="JpegDecompressStruct.JpegReadHeader"/>
        /// </summary>
        /// <value>The height of image.</value>
        /// <seealso href="../articles/KB/decompression-parameter-selection.html">Decompression parameter selection</seealso>
        public int Image_height => imageHeight;

        /// <summary>
        /// Gets the number of color components in JPEG image.
        /// </summary>
        /// <value>The number of color components.</value>
        /// <seealso href="../articles/KB/decompression-parameter-selection.html">Decompression parameter selection</seealso>
        public int Num_components => numComponents;

        /// <summary>
        /// Gets or sets the colorspace of JPEG image.
        /// </summary>
        /// <value>The colorspace of JPEG image.</value>
        /// <seealso href="../articles/KB/decompression-parameter-selection.html">Decompression parameter selection</seealso>
        public LibJpeg.Classic.JColorSpace Jpeg_color_space
        {
            get { return jpegColorSpace; }
            set { jpegColorSpace = value; }
        }

        /// <summary>
        /// Gets the list of loaded special markers.
        /// </summary>
        /// <remarks>All the special markers in the file appear in this list, in order of 
        /// their occurrence in the file (but omitting any markers of types you didn't ask for)
        /// </remarks>
        /// <value>The list of loaded special markers.</value>
        /// <seealso href="../articles/KB/special-markers.html">Special markers</seealso>
        public ReadOnlyCollection<JpegMarkerStruct> Marker_list => new ReadOnlyCollection<JpegMarkerStruct>(m_marker_list);

        /* Decompression processing parameters --- these fields must be set before
         * calling jpeg_start_decompress().  Note that jpeg_read_header() initializes
         * them to default values.
         */

        /// <summary>
        /// Gets or sets the output color space.
        /// </summary>
        /// <value>The output color space.</value>
        /// <seealso href="../articles/KB/decompression-parameter-selection.html">Decompression parameter selection</seealso>
        public LibJpeg.Classic.JColorSpace Out_color_space
        {
            get { return outColorSpace; }
            set { outColorSpace = value; }
        }

        /// <summary>
        /// Gets or sets the numerator of the fraction of image scaling.
        /// </summary>
        /// <value>Scale the image by the fraction Scale_num/<see cref="JpegDecompressStruct.Scale_denom">Scale_denom</see>. 
        /// Default is 1/1, or no scaling. Currently, the only supported scaling ratios are 1/1, 1/2, 1/4, and 1/8.
        /// (The library design allows for arbitrary scaling ratios but this is not likely to be implemented any time soon.)
        /// </value>
        /// <remarks>Smaller scaling ratios permit significantly faster decoding since fewer pixels 
        /// need to be processed and a simpler <see cref="JDctMethod">DCT method</see> can be used.</remarks>
        /// <seealso cref="JpegDecompressStruct.Scale_denom"/>
        /// <seealso href="../articles/KB/decompression-parameter-selection.html">Decompression parameter selection</seealso>
        public int Scale_num
        {
            get { return scaleNum; }
            set { scaleNum = value; }
        }

        /// <summary>
        /// Gets or sets the denominator of the fraction of image scaling.
        /// </summary>
        /// <value>Scale the image by the fraction <see cref="JpegDecompressStruct.Scale_num">Scale_num</see>/Scale_denom. 
        /// Default is 1/1, or no scaling. Currently, the only supported scaling ratios are 1/1, 1/2, 1/4, and 1/8.
        /// (The library design allows for arbitrary scaling ratios but this is not likely to be implemented any time soon.)
        /// </value>
        /// <remarks>Smaller scaling ratios permit significantly faster decoding since fewer pixels 
        /// need to be processed and a simpler <see cref="JDctMethod">DCT method</see> can be used.</remarks>
        /// <seealso cref="JpegDecompressStruct.Scale_num"/>
        /// <seealso href="../articles/KB/decompression-parameter-selection.html">Decompression parameter selection</seealso>
        public int Scale_denom
        {
            get { return scaleDenom; }
            set { scaleDenom = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to use buffered-image mode.
        /// </summary>
        /// <value><c>true</c> if buffered-image mode is turned on; otherwise, <c>false</c>.</value>
        /// <seealso href="../articles/KB/buffered-image-mode.html">Buffered-image mode</seealso>
        public bool Buffered_image
        {
            get { return bufferedImage; }
            set { bufferedImage = value; }
        }

        /// <summary>
        /// Enable or disable raw data output.
        /// </summary>
        /// <value><c>true</c> if raw data output is enabled; otherwise, <c>false</c>.</value>
        /// <remarks>Default value: <c>false</c><br/>
        /// Set this to true before <see cref="JpegDecompressStruct.JpegStartDecompress"/> 
        /// if you need to obtain raw data output.
        /// </remarks>
        /// <seealso cref="JpegReadRawData"/>
        public bool Raw_data_out
        {
            get { return rawDataOut; }
            set { rawDataOut = value; }
        }

        /// <summary>
        /// Gets or sets the algorithm used for the DCT step.
        /// </summary>
        /// <value>The algorithm used for the DCT step.</value>
        /// <seealso href="../articles/KB/decompression-parameter-selection.html">Decompression parameter selection</seealso>
        public LibJpeg.Classic.JDctMethod Dct_method
        {
            get { return dctMethod; }
            set { dctMethod = value; }
        }

        /// <summary>
        /// Enable or disable upsampling of chroma components.
        /// </summary>
        /// <value>If <c>true</c>, do careful upsampling of chroma components. 
        /// If <c>false</c>, a faster but sloppier method is used. 
        /// The visual impact of the sloppier method is often very small.
        /// </value>
        /// <remarks>Default value: <c>true</c></remarks>
        /// <seealso href="../articles/KB/decompression-parameter-selection.html">Decompression parameter selection</seealso>
        public bool Do_fancy_upsampling
        {
            get { return doFancyUpsampling; }
            set { doFancyUpsampling = value; }
        }

        /// <summary>
        /// Apply interblock smoothing in early stages of decoding progressive JPEG files.
        /// </summary>
        /// <value>If <c>true</c>, interblock smoothing is applied in early stages of decoding progressive JPEG files; 
        /// if <c>false</c>, not. Early progression stages look "fuzzy" with smoothing, "blocky" without.</value>
        /// <remarks>Default value: <c>true</c><br/>
        /// In any case, block smoothing ceases to be applied after the first few AC coefficients are 
        /// known to full accuracy, so it is relevant only when using 
        /// <see href="../articles/KB/buffered-image-mode.html">buffered-image mode</see> for progressive images.
        /// </remarks>
        /// <seealso href="../articles/KB/decompression-parameter-selection.html">Decompression parameter selection</seealso>
        public bool Do_block_smoothing
        {
            get { return doBlockSmoothing; }
            set { doBlockSmoothing = value; }
        }

        /// <summary>
        /// Colors quantization.
        /// </summary>
        /// <value>If set <c>true</c>, colormapped output will be delivered.<br/>
        /// Default value: <c>false</c>, meaning that full-color output will be delivered.
        /// </value>
        /// <seealso href="../articles/KB/decompression-parameter-selection.html">Decompression parameter selection</seealso>
        public bool Quantize_colors
        {
            get { return quantizeColors; }
            set { quantizeColors = value; }
        }

        /* the following are ignored if not quantize_colors: */

        /// <summary>
        /// Selects color dithering method.
        /// </summary>
        /// <value>Default value: <see cref="JDitherMode.JDITHER_FS"/>.</value>
        /// <remarks>Ignored if <see cref="JpegDecompressStruct.Quantize_colors"/> is <c>false</c>.<br/>
        /// At present, ordered dither is implemented only in the single-pass, standard-colormap case. 
        /// If you ask for ordered dither when <see cref="JpegDecompressStruct.Two_pass_quantize"/> is <c>true</c>
        /// or when you supply an external color map, you'll get F-S dithering.
        /// </remarks>
        /// <seealso cref="JpegDecompressStruct.Quantize_colors"/>
        /// <seealso href="../articles/KB/decompression-parameter-selection.html">Decompression parameter selection</seealso>
        public LibJpeg.Classic.JDitherMode Dither_mode
        {
            get { return ditherMode; }
            set { ditherMode = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to use two-pass color quantization.
        /// </summary>
        /// <value>If <c>true</c>, an extra pass over the image is made to select a custom color map for the image.
        /// This usually looks a lot better than the one-size-fits-all colormap that is used otherwise.
        /// Ignored when the application supplies its own color map.<br/>
        /// 
        /// Default value: <c>true</c>
        /// </value>
        /// <remarks>Ignored if <see cref="JpegDecompressStruct.Quantize_colors"/> is <c>false</c>.<br/>
        /// </remarks>
        /// <seealso cref="JpegDecompressStruct.Quantize_colors"/>
        /// <seealso href="../articles/KB/decompression-parameter-selection.html">Decompression parameter selection</seealso>
        public bool Two_pass_quantize
        {
            get { return twoPassQuantize; }
            set { twoPassQuantize = value; }
        }

        /// <summary>
        /// Maximum number of colors to use in generating a library-supplied color map.
        /// </summary>
        /// <value>Default value: 256.</value>
        /// <remarks>Ignored if <see cref="JpegDecompressStruct.Quantize_colors"/> is <c>false</c>.<br/>
        /// The actual number of colors is returned in a <see cref="JpegDecompressStruct.ActualNumberOfColors"/>.
        /// </remarks>
        /// <seealso cref="JpegDecompressStruct.Quantize_colors"/>
        /// <seealso href="../articles/KB/decompression-parameter-selection.html">Decompression parameter selection</seealso>
        public int Desired_number_of_colors
        {
            get { return desiredNumberOfColors; }
            set { desiredNumberOfColors = value; }
        }

        /* these are significant only in buffered-image mode: */

        /// <summary>
        /// Enable future use of 1-pass quantizer.
        /// </summary>
        /// <value>Default value: <c>false</c></value>
        /// <remarks>Significant only in buffered-image mode.</remarks>
        /// <seealso href="../articles/KB/buffered-image-mode.html">Buffered-image mode</seealso>
        public bool Enable_1pass_quant
        {
            get { return enable1PassQuant; }
            set { enable1PassQuant = value; }
        }

        /// <summary>
        /// Enable future use of external colormap.
        /// </summary>
        /// <value>Default value: <c>false</c></value>
        /// <remarks>Significant only in buffered-image mode.</remarks>
        /// <seealso href="../articles/KB/buffered-image-mode.html">Buffered-image mode</seealso>
        public bool Enable_external_quant
        {
            get { return enableExternalQuant; }
            set { enableExternalQuant = value; }
        }

        /// <summary>
        /// Enable future use of 2-pass quantizer.
        /// </summary>
        /// <value>Default value: <c>false</c></value>
        /// <remarks>Significant only in buffered-image mode.</remarks>
        /// <seealso href="../articles/KB/buffered-image-mode.html">Buffered-image mode</seealso>
        public bool Enable_2pass_quant
        {
            get { return enable2PassQuant; }
            set { enable2PassQuant = value; }
        }

        /* Description of actual output image that will be returned to application.
         * These fields are computed by jpeg_start_decompress().
         * You can also use jpeg_calc_output_dimensions() to determine these values
         * in advance of calling jpeg_start_decompress().
         */

        /// <summary>
        /// Gets the actual width of output image.
        /// </summary>
        /// <value>The width of output image.</value>
        /// <remarks>Computed by <see cref="JpegDecompressStruct.jpeg_start_decompress"/>.
        /// You can also use <see cref="JpegDecompressStruct.jpeg_calc_output_dimensions"/> to determine this value
        /// in advance of calling <see cref="JpegDecompressStruct.jpeg_start_decompress"/>.</remarks>
        /// <seealso cref="JpegDecompressStruct.Output_height"/>
        public int Output_width => outputWidth;

        /// <summary>
        /// Gets the actual height of output image.
        /// </summary>
        /// <value>The height of output image.</value>
        /// <remarks>Computed by <see cref="JpegDecompressStruct.JpegStartDecompress"/>.
        /// You can also use <see cref="JpegDecompressStruct.JpegCalcOutputDimensions"/> to determine this value
        /// in advance of calling <see cref="JpegDecompressStruct.JpegStartDecompress"/>.</remarks>
        /// <seealso cref="JpegDecompressStruct.Output_width"/>
        public int Output_height => outputHeight;

        /// <summary>
        /// Gets the number of color components in <see cref="JpegDecompressStruct.Out_color_space"/>.
        /// </summary>
        /// <remarks>Computed by <see cref="JpegDecompressStruct.JpegStartDecompress"/>.
        /// You can also use <see cref="JpegDecompressStruct.JpegCalcOutputDimensions"/> to determine this value
        /// in advance of calling <see cref="JpegDecompressStruct.JpegStartDecompress"/>.</remarks>
        /// <value>The number of color components.</value>
        /// <seealso cref="JpegDecompressStruct.Out_color_space"/>
        /// <seealso href="../articles/KB/decompression-parameter-selection.html">Decompression parameter selection</seealso>
        public int Out_color_components => outColorComponents;

        /// <summary>
        /// Gets the number of color components returned.
        /// </summary>
        /// <remarks>Computed by <see cref="JpegDecompressStruct.JpegStartDecompress"/>.
        /// You can also use <see cref="JpegDecompressStruct.JpegCalcOutputDimensions"/> to determine this value
        /// in advance of calling <see cref="JpegDecompressStruct.JpegStartDecompress"/>.</remarks>
        /// <value>When <see cref="JpegDecompressStruct.Quantize_colors">quantizing colors</see>, 
        /// <c>Output_components</c> is 1, indicating a single color map index per pixel. 
        /// Otherwise it equals to <see cref="JpegDecompressStruct.Out_color_components"/>.
        /// </value>
        /// <seealso cref="JpegDecompressStruct.Out_color_space"/>
        /// <seealso href="../articles/KB/decompression-parameter-selection.html">Decompression parameter selection</seealso>
        public int Output_components => outputComponents;

        /// <summary>
        /// Gets the recommended height of scanline buffer.
        /// </summary>
        /// <value>In high-quality modes, <c>Rec_outbuf_height</c> is always 1, but some faster, 
        /// lower-quality modes set it to larger values (typically 2 to 4).</value>
        /// <remarks><para>
        /// Computed by <see cref="JpegDecompressStruct.JpegStartDecompress"/>.
        /// You can also use <see cref="JpegDecompressStruct.JpegCalcOutputDimensions"/> to determine this value
        /// in advance of calling <see cref="JpegDecompressStruct.JpegStartDecompress"/>.<br/>
        /// </para>
        /// <para>
        /// <c>Rec_outbuf_height</c> is the recommended minimum height (in scanlines) 
        /// of the buffer passed to <see cref="JpegDecompressStruct.JpegReadScanlines"/>.
        /// If the buffer is smaller, the library will still work, but time will be wasted due 
        /// to unnecessary data copying. If you are going to ask for a high-speed processing mode, 
        /// you may as well go to the trouble of honoring <c>Rec_outbuf_height</c> so as to avoid data copying.
        /// (An output buffer larger than <c>Rec_outbuf_height</c> lines is OK, but won't provide 
        /// any material speed improvement over that height.)
        /// </para>
        /// </remarks>
        /// <seealso href="../articles/KB/decompression-parameter-selection.html">Decompression parameter selection</seealso>
        public int Rec_outbuf_height => recommendOutBufferHeight;

        /* When quantizing colors, the output colormap is described by these fields.
         * The application can supply a colormap by setting colormap non-null before
         * calling jpeg_start_decompress; otherwise a colormap is created during
         * jpeg_start_decompress or jpeg_start_output.
         * The map has out_color_components rows and actual_number_of_colors columns.
         */

        /// <summary>
        /// The number of colors in the color map.
        /// </summary>
        /// <value>The number of colors in the color map.</value>
        /// <seealso cref="JpegDecompressStruct.Colormap"/>
        /// <seealso href="../articles/KB/decompression-parameter-selection.html">Decompression parameter selection</seealso>
        public int ActualNumberOfColors
        {
            get { return actualColorCount; }
            set { actualColorCount = value; }
        }

        /// <summary>
        /// The color map, represented as a 2-D pixel array of <see cref="JpegDecompressStruct.Out_color_components"/> rows 
        /// and <see cref="JpegDecompressStruct.ActualNumberOfColors"/> columns.
        /// </summary>
        /// <value>Colormap is set to <c>null</c> by <see cref="JpegDecompressStruct.JpegReadHeader"/>.
        /// The application can supply a color map by setting <c>Colormap</c> non-null and setting 
        /// <see cref="JpegDecompressStruct.ActualNumberOfColors"/> to the map size.
        /// </value>
        /// <remarks>Ignored if not quantizing.<br/>
        /// Implementation restriction: at present, an externally supplied <c>Colormap</c>
        /// is only accepted for 3-component output color spaces.
        /// </remarks>
        /// <seealso cref="JpegDecompressStruct.ActualNumberOfColors"/>
        /// <seealso cref="JpegDecompressStruct.Quantize_colors"/>
        /// <seealso href="../articles/KB/decompression-parameter-selection.html">Decompression parameter selection</seealso>
        public byte[][] Colormap
        {
            get { return m_colormap; }
            set { m_colormap = value; }
        }

        /* State variables: these variables indicate the progress of decompression.
         * The application may examine these but must not modify them.
         */

        /* Row index of next scanline to be read from jpeg_read_scanlines().
         * Application may use this to control its processing loop, e.g.,
         * "while (output_scanline < output_height)".
         */

        /// <summary>
        /// Gets the number of scanlines returned so far.
        /// </summary>
        /// <value>The output_scanline.</value>
        /// <remarks>Usually you can just use this variable as the loop counter, 
        /// so that the loop test looks like 
        /// <c>while (cinfo.Output_scanline &lt; cinfo.Output_height)</c></remarks>
        /// <seealso href="../articles/KB/decompression-details.html">Decompression details</seealso>
        public int Output_scanline => outputScanline;

        /* Current input scan number and number of iMCU rows completed in scan.
         * These indicate the progress of the decompressor input side.
         */

        /// <summary>
        /// Gets the number of SOS markers seen so far.
        /// </summary>
        /// <value>The number of SOS markers seen so far.</value>
        /// <remarks>Indicates the progress of the decompressor input side.</remarks>
        public int Input_scan_number => inputScanNumber;

        /// <summary>
        /// Gets the number of iMCU rows completed.
        /// </summary>
        /// <value>The number of iMCU rows completed.</value>
        /// <remarks>Indicates the progress of the decompressor input side.</remarks>
        public int Input_iMCU_row => inputIMcuRow;

        /* The "output scan number" is the notional scan being displayed by the
         * output side.  The decompressor will not allow output scan/row number
         * to get ahead of input scan/row, but it can fall arbitrarily far behind.
         */

        /// <summary>
        /// Gets the nominal scan number being displayed.
        /// </summary>
        /// <value>The nominal scan number being displayed.</value>
        public int Output_scan_number => outputScanNumber;

        /// <summary>
        /// Gets the number of iMCU rows read.
        /// </summary>
        /// <value>The number of iMCU rows read.</value>
        public int Output_iMCU_row => outputIMcuRow;

        /* Current progression status.  coef_bits[c][i] indicates the precision
         * with which component c's DCT coefficient i (in zigzag order) is known.
         * It is -1 when no data has yet been received, otherwise it is the point
         * transform (shift) value for the most recent scan of the coefficient
         * (thus, 0 at completion of the progression).
         * This is null when reading a non-progressive file.
         */

        /// <summary>
        /// Gets the current progression status..
        /// </summary>
        /// <value><c>Coef_bits[c][i]</c> indicates the precision with 
        /// which component c's DCT coefficient i (in zigzag order) is known. 
        /// It is <c>-1</c> when no data has yet been received, otherwise 
        /// it is the point transform (shift) value for the most recent scan of the coefficient 
        /// (thus, 0 at completion of the progression). This is null when reading a non-progressive file.
        /// </value>
        /// <seealso href="../articles/KB/progressive-jpeg.html">Progressive JPEG support</seealso>
        public int[][] Coef_bits => coefBits;

        // These fields record data obtained from optional markers 
        // recognized by the JPEG library.

        /// <summary>
        /// Gets the resolution information from JFIF marker.
        /// </summary>
        /// <value>The information from JFIF marker.</value>
        /// <seealso cref="JpegDecompressStruct.XDensity"/>
        /// <seealso cref="JpegDecompressStruct.YDensity"/>
        /// <seealso href="../articles/KB/decompression-parameter-selection.html">Decompression parameter selection</seealso>
        public DensityUnit DensityUnit => m_densityUnit;

        /// <summary>
        /// Gets the horizontal component of pixel ratio.
        /// </summary>
        /// <value>The horizontal component of pixel ratio.</value>
        /// <seealso cref="JpegDecompressStruct.YDensity"/>
        /// <seealso cref="JpegDecompressStruct.DensityUnit"/>
        public short XDensity => m_xDensity;

        /// <summary>
        /// Gets the vertical component of pixel ratio.
        /// </summary>
        /// <value>The vertical component of pixel ratio.</value>
        /// <seealso cref="JpegDecompressStruct.XDensity"/>
        /// <seealso cref="JpegDecompressStruct.DensityUnit"/>
        public short YDensity => m_yDensity;

        /// <summary>
        /// Gets the data precision.
        /// </summary>
        /// <value>The data precision.</value>
        public int DataPrecision => m_dataPrecision;

        /// <summary>
        /// Gets the largest vertical sample factor.
        /// </summary>
        /// <value>The largest vertical sample factor.</value>
        public int MaxVSampleFactor => m_maxVSampleFactor;

        /// <summary>
        /// Gets the last read and unprocessed JPEG marker.
        /// </summary>
        /// <value>It is either zero or the code of a JPEG marker that has been
        /// read from the data source, but has not yet been processed.
        /// </value>
        /// <seealso cref="JpegDecompressStruct.JpegSetMarkerProcessor"/>
        /// <seealso href="../articles/KB/special-markers.html">Special markers</seealso>
        public int UnreadMarker => m_unreadMarker;

        /// <summary>
        /// Comp_info[i] describes component that appears i'th in SOF
        /// </summary>
        /// <value>The components in SOF.</value>
        /// <seealso cref="JpegComponentInfo"/>
        public JpegComponentInfo[] CompInfo { get; internal set; }

        /// <summary>
        /// Sets input stream.
        /// </summary>
        /// <param name="infile">The input stream.</param>
        /// <remarks>
        /// The caller must have already opened the stream, and is responsible
        /// for closing it after finishing decompression.
        /// </remarks>
        /// <seealso href="../articles/KB/decompression-details.html">Decompression details</seealso>
        public void JpegStdioSrc(Stream infile)
        {
            if (infile is null)
            {
                throw new ArgumentNullException(nameof(infile));
            }
            /* The source object and input buffer are made permanent so that a series
            * of JPEG images can be read from the same file by calling jpeg_stdio_src
            * only before the first one.  (If we discarded the buffer at the end of
            * one image, we'd likely lose the start of the next one.)
            * This makes it unsafe to use this manager and a different source
            * manager serially with the same JPEG object.  Caveat programmer.
            */
            if (m_src is null)
            {
                /* first time for this JPEG object? */
                m_src = new MySourceManager(this);
            }

            if (m_src is MySourceManager m)
            {
                m.Attach(infile);
            }
        }

        /// <summary>
        /// Decompression startup: this will read the source datastream header markers, up to the beginning of the compressed data proper.
        /// </summary>
        /// <param name="require_image">Read a description of <b>Return Value</b>.</param>
        /// <returns>
        /// <para>
        /// If you pass <c>require_image=true</c> (normal case), you need not check for a
        /// <see cref="ReadResult.JPEG_HEADER_TABLES_ONLY"/> return code; an abbreviated file will cause
        /// an error exit. <see cref="ReadResult.JPEG_SUSPENDED"/> is only possible if you use a data source
        /// module that can give a suspension return.<br/><br/>
        /// </para>
        /// <para>
        /// This method will read as far as the first SOS marker (ie, actual start of compressed data),
        /// and will save all tables and parameters in the JPEG object. It will also initialize the
        /// decompression parameters to default values, and finally return <see cref="ReadResult.JPEG_HEADER_OK"/>.
        /// On return, the application may adjust the decompression parameters and then call
        /// <see cref="JpegDecompressStruct.JpegStartDecompress"/>. (Or, if the application only wanted to
        /// determine the image parameters, the data need not be decompressed. In that case, call
        /// <see cref="JpegCommonStruct.JpegAbort"/> to release any temporary space.)<br/><br/>
        /// </para>
        /// <para>
        /// If an abbreviated (tables only) datastream is presented, the routine will return
        /// <see cref="ReadResult.JPEG_HEADER_TABLES_ONLY"/> upon reaching EOI. The application may then re-use
        /// the JPEG object to read the abbreviated image datastream(s). It is unnecessary (but OK) to call
        /// <see cref="JpegCommonStruct.JpegAbort">jpeg_abort</see> in this case.
        /// The <see cref="ReadResult.JPEG_SUSPENDED"/> return code only occurs if the data source module
        /// requests suspension of the decompressor. In this case the application should load more source
        /// data and then re-call <c>jpeg_read_header</c> to resume processing.<br/><br/>
        /// </para>
        /// <para>
        /// If a non-suspending data source is used and <c>require_image</c> is <c>true</c>,
        /// then the return code need not be inspected since only <see cref="ReadResult.JPEG_HEADER_OK"/> is possible.
        /// </para>
        /// </returns>
        /// <remarks>Need only initialize JPEG object and supply a data source before calling.<br/>
        /// On return, the image dimensions and other info have been stored in the JPEG object.
        /// The application may wish to consult this information before selecting decompression parameters.<br/>
        /// This routine is now just a front end to <see cref="JpegConsumeInput"/>, with some extra error checking.
        /// </remarks>
        /// <seealso href="../articles/KB/decompression-details.html">Decompression details</seealso>
        /// <seealso href="../articles/KB/decompression-parameter-selection.html">Decompression parameter selection</seealso>
        public ReadResult JpegReadHeader(bool require_image)
        {
            if (globalState != JpegState.DSTATE_START && globalState != JpegState.DSTATE_INHEADER)
            {
                ErrExit(JMessageCode.JERR_BAD_STATE, (int)globalState);
            }

            var retcode = JpegConsumeInput();

            switch (retcode)
            {
                case ReadResult.JPEG_REACHED_SOS:
                return ReadResult.JPEG_HEADER_OK;
                case ReadResult.JPEG_REACHED_EOI:
                if (require_image)      /* Complain if application wanted an image */
                {
                    ErrExit(JMessageCode.JERR_NO_IMAGE);
                }
                /* Reset to start state; it would be safer to require the application to
* call jpeg_abort, but we can't change it now for compatibility reasons.
* A side effect is to free any temporary memory (there shouldn't be any).
*/
                JpegAbort(); /* sets state = DSTATE_START */
                return ReadResult.JPEG_HEADER_TABLES_ONLY;

                case ReadResult.JPEG_SUSPENDED:
                /* no work */
                break;
            }

            return ReadResult.JPEG_SUSPENDED;
        }

        //////////////////////////////////////////////////////////////////////////
        // Main entry points for decompression

        /// <summary>
        /// Decompression initialization.
        /// </summary>
        /// <returns>Returns <c>false</c> if suspended. The return value need be inspected 
        /// only if a suspending data source is used.
        /// </returns>
        /// <remarks><para><see cref="JpegDecompressStruct.JpegReadHeader">jpeg_read_header</see> must be completed before calling this.<br/></para>
        /// <para>If a multipass operating mode was selected, this will do all but the last pass, and thus may take a great deal of time.</para>
        /// </remarks>
        /// <seealso cref="JpegDecompressStruct.JpegFinishDecompress"/>
        /// <seealso href="../articles/KB/decompression-details.html">Decompression details</seealso>
        public bool JpegStartDecompress()
        {
            if (globalState == JpegState.DSTATE_READY)
            {
                /* First call: initialize master control, select active modules */
                m_master = new JpegDecompMaster(this);
                if (bufferedImage)
                {
                    /* No more work here; expecting jpeg_start_output next */
                    globalState = JpegState.DSTATE_BUFIMAGE;
                    return true;
                }

                globalState = JpegState.DSTATE_PRELOAD;
            }

            if (globalState == JpegState.DSTATE_PRELOAD)
            {
                /* If file has multiple scans, absorb them all into the coef buffer */
                if (m_inputctl.HasMultipleScans())
                {
                    while (true)
                    {
                        ReadResult retcode;
                        /* Call progress monitor hook if present */
                        prog?.Updated();

                        /* Absorb some more input */
                        retcode = m_inputctl.ConsumeInput();
                        if (retcode == ReadResult.JPEG_SUSPENDED)
                        {
                            return false;
                        }

                        if (retcode == ReadResult.JPEG_REACHED_EOI)
                        {
                            break;
                        }

                        /* Advance progress counter if appropriate */
                        if (prog is object && (retcode == ReadResult.JPEG_ROW_COMPLETED || retcode == ReadResult.JPEG_REACHED_SOS))
                        {
                            prog.PassCounter++;
                            if (prog.PassCounter >= prog.PassLimit)
                            {
                                /* underestimated number of scans; ratchet up one scan */
                                prog.PassLimit += m_total_iMCU_rows;
                            }
                        }
                    }
                }

                outputScanNumber = inputScanNumber;
            }
            else if (globalState != JpegState.DSTATE_PRESCAN)
            {
                ErrExit(JMessageCode.JERR_BAD_STATE, (int)globalState);
            }

            /* Perform any dummy output passes, and set up for the final pass */
            return OutputPassSetup();
        }

        /// <summary>
        /// Read some scanlines of data from the JPEG decompressor.
        /// </summary>
        /// <param name="scanlines">Buffer for filling.</param>
        /// <param name="max_lines">Required number of lines.</param>
        /// <returns>The return value will be the number of lines actually read. 
        /// This may be less than the number requested in several cases, including 
        /// bottom of image, data source suspension, and operating modes that emit multiple scanlines at a time.
        /// </returns>
        /// <remarks>We warn about excess calls to <c>jpeg_read_scanlines</c> since this likely signals an 
        /// application programmer error. However, an oversize buffer <c>(max_lines > scanlines remaining)</c> 
        /// is not an error.
        /// </remarks>
        /// <seealso href="../articles/KB/decompression-details.html">Decompression details</seealso>
        public int JpegReadScanlines(byte[][] scanlines, int max_lines)
        {
            if (globalState != JpegState.DSTATE_SCANNING)
            {
                ErrExit(JMessageCode.JERR_BAD_STATE, (int)globalState);
            }

            if (outputScanline >= outputHeight)
            {
                WarnMS(JMessageCode.JWRN_TOO_MUCH_DATA);
                return 0;
            }

            /* Call progress monitor hook if present */
            if (prog is object)
            {
                prog.PassCounter = outputScanline;
                prog.PassLimit = outputHeight;
                prog.Updated();
            }

            /* Process some data */
            var row_ctr = 0;
            m_main.ProcessData(scanlines, ref row_ctr, max_lines);
            outputScanline += row_ctr;
            return row_ctr;
        }

        /// <summary>
        /// Finish JPEG decompression.
        /// </summary>
        /// <returns>Returns <c>false</c> if suspended. The return value need be inspected 
        /// only if a suspending data source is used.
        /// </returns>
        /// <remarks>This will normally just verify the file trailer and release temp storage.</remarks>
        /// <seealso cref="JpegDecompressStruct.JpegStartDecompress"/>
        /// <seealso href="../articles/KB/decompression-details.html">Decompression details</seealso>
        public bool JpegFinishDecompress()
        {
            if ((globalState == JpegState.DSTATE_SCANNING || globalState == JpegState.DSTATE_RAW_OK) && !bufferedImage)
            {
                /* Terminate final pass of non-buffered mode */
                if (outputScanline < outputHeight)
                {
                    ErrExit(JMessageCode.JERR_TOO_LITTLE_DATA);
                }

                m_master.FinishOutputPass();
                globalState = JpegState.DSTATE_STOPPING;
            }
            else if (globalState == JpegState.DSTATE_BUFIMAGE)
            {
                /* Finishing after a buffered-image operation */
                globalState = JpegState.DSTATE_STOPPING;
            }
            else if (globalState != JpegState.DSTATE_STOPPING)
            {
                /* STOPPING = repeat call after a suspension, anything else is error */
                ErrExit(JMessageCode.JERR_BAD_STATE, (int)globalState);
            }

            /* Read until EOI */
            while (!m_inputctl.EOIReached())
            {
                if (m_inputctl.ConsumeInput() == ReadResult.JPEG_SUSPENDED)
                {
                    /* Suspend, come back later */
                    return false;
                }
            }

            /* Do final cleanup */
            m_src.TermSource();

            /* We can use jpeg_abort to release memory and reset global_state */
            JpegAbort();
            return true;
        }

        /// <summary>
        /// Alternate entry point to read raw data.
        /// </summary>
        /// <param name="data">The raw data.</param>
        /// <param name="max_lines">The number of scanlines for reading.</param>
        /// <returns>The number of lines actually read.</returns>
        /// <remarks>Replaces <see cref="JpegDecompressStruct.JpegReadScanlines">jpeg_read_scanlines</see> 
        /// when reading raw downsampled data. Processes exactly one iMCU row per call, unless suspended.
        /// </remarks>
        public int JpegReadRawData(byte[][][] data, int max_lines)
        {
            if (data is null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            if (globalState != JpegState.DSTATE_RAW_OK)
            {
                ErrExit(JMessageCode.JERR_BAD_STATE, (int)globalState);
            }

            if (outputScanline >= outputHeight)
            {
                WarnMS(JMessageCode.JWRN_TOO_MUCH_DATA);
                return 0;
            }

            /* Call progress monitor hook if present */
            if (prog is object)
            {
                prog.PassCounter = outputScanline;
                prog.PassLimit = outputHeight;
                prog.Updated();
            }

            /* Verify that at least one iMCU row can be returned. */
            var lines_per_iMCU_row = m_maxVSampleFactor * min_DCT_v_scaled_size;
            if (max_lines < lines_per_iMCU_row)
            {
                ErrExit(JMessageCode.JERR_BUFFER_SIZE);
            }

            var componentCount = data.Length; // maybe we should use max_lines here
            var cb = new ComponentBuffer[componentCount];
            for (var i = 0; i < componentCount; i++)
            {
                cb[i] = new ComponentBuffer();
                cb[i].SetBuffer(data[i], null, 0);
            }

            /* Decompress directly into user's buffer. */
            if (m_coef.DecompressData(cb) == ReadResult.JPEG_SUSPENDED)
            {
                /* suspension forced, can do nothing more */
                return 0;
            }

            /* OK, we processed one iMCU row. */
            outputScanline += lines_per_iMCU_row;
            return lines_per_iMCU_row;
        }

        //////////////////////////////////////////////////////////////////////////
        // Additional entry points for buffered-image mode.

        /// <summary>
        /// Is there more than one scan?
        /// </summary>
        /// <returns><c>true</c> if image has more than one scan; otherwise, <c>false</c></returns>
        /// <remarks>If you are concerned about maximum performance on baseline JPEG files,
        /// you should use <see href="../articles/KB/buffered-image-mode.html">buffered-image mode</see> only
        /// when the incoming file actually has multiple scans. This can be tested by calling this method.
        /// </remarks>
        public bool JpegHasMultipleScans()
        {
            /* Only valid after jpeg_read_header completes */
            if (globalState < JpegState.DSTATE_READY || globalState > JpegState.DSTATE_STOPPING)
            {
                ErrExit(JMessageCode.JERR_BAD_STATE, (int)globalState);
            }

            return m_inputctl.HasMultipleScans();
        }

        /// <summary>
        /// Initialize for an output pass in <see href="../articles/KB/buffered-image-mode.html">buffered-image mode</see>.
        /// </summary>
        /// <param name="scan_number">Indicates which scan of the input file is to be displayed; 
        /// the scans are numbered starting at 1 for this purpose.</param>
        /// <returns><c>true</c> if done; <c>false</c> if suspended</returns>
        /// <seealso cref="JpegDecompressStruct.JpegFinishOutput"/>
        /// <seealso href="../articles/KB/buffered-image-mode.html">Buffered-image mode</seealso>
        public bool JpegStartOutput(int scan_number)
        {
            if (globalState != JpegState.DSTATE_BUFIMAGE && globalState != JpegState.DSTATE_PRESCAN)
            {
                ErrExit(JMessageCode.JERR_BAD_STATE, (int)globalState);
            }

            /* Limit scan number to valid range */
            if (scan_number <= 0)
            {
                scan_number = 1;
            }

            if (m_inputctl.EOIReached() && scan_number > inputScanNumber)
            {
                scan_number = inputScanNumber;
            }

            outputScanNumber = scan_number;
            /* Perform any dummy output passes, and set up for the real pass */
            return OutputPassSetup();
        }

        /// <summary>
        /// Finish up after an output pass in <see href="../articles/KB/buffered-image-mode.html">buffered-image mode</see>.
        /// </summary>
        /// <returns>Returns <c>false</c> if suspended. The return value need be inspected only if a suspending data source is used.</returns>
        /// <seealso cref="JpegDecompressStruct.JpegStartOutput"/>
        /// <seealso href="../articles/KB/buffered-image-mode.html">Buffered-image mode</seealso>
        public bool JpegFinishOutput()
        {
            if ((globalState == JpegState.DSTATE_SCANNING || globalState == JpegState.DSTATE_RAW_OK) && bufferedImage)
            {
                /* Terminate this pass. */
                /* We do not require the whole pass to have been completed. */
                m_master.FinishOutputPass();
                globalState = JpegState.DSTATE_BUFPOST;
            }
            else if (globalState != JpegState.DSTATE_BUFPOST)
            {
                /* BUFPOST = repeat call after a suspension, anything else is error */
                ErrExit(JMessageCode.JERR_BAD_STATE, (int)globalState);
            }

            /* Read markers looking for SOS or EOI */
            while (inputScanNumber <= outputScanNumber && !m_inputctl.EOIReached())
            {
                if (m_inputctl.ConsumeInput() == ReadResult.JPEG_SUSPENDED)
                {
                    /* Suspend, come back later */
                    return false;
                }
            }

            globalState = JpegState.DSTATE_BUFIMAGE;
            return true;
        }

        /// <summary>
        /// Indicates if we have finished reading the input file.
        /// </summary>
        /// <returns><c>true</c> if we have finished reading the input file.</returns>
        /// <seealso href="../articles/KB/buffered-image-mode.html">Buffered-image mode</seealso>
        public bool JpegInputComplete()
        {
            /* Check for valid jpeg object */
            if (globalState < JpegState.DSTATE_START || globalState > JpegState.DSTATE_STOPPING)
            {
                ErrExit(JMessageCode.JERR_BAD_STATE, (int)globalState);
            }

            return m_inputctl.EOIReached();
        }

        /// <summary>
        /// Consume data in advance of what the decompressor requires.
        /// </summary>
        /// <returns>The result of data consumption.</returns>
        /// <remarks>This routine can be called at any time after initializing the JPEG object.
        /// It reads some additional data and returns when one of the indicated significant events
        /// occurs. If called after the EOI marker is reached, it will immediately return
        /// <see cref="ReadResult.JPEG_REACHED_EOI"/> without attempting to read more data.</remarks>
        public ReadResult JpegConsumeInput()
        {
            var retcode = ReadResult.JPEG_SUSPENDED;

            /* NB: every possible DSTATE value should be listed in this switch */
            switch (globalState)
            {
                case JpegState.DSTATE_START:
                JpegConsumeInputStart();
                retcode = JpegConsumeInputInHeader();
                break;
                case JpegState.DSTATE_INHEADER:
                retcode = JpegConsumeInputInHeader();
                break;
                case JpegState.DSTATE_READY:
                /* Can't advance past first SOS until start_decompress is called */
                retcode = ReadResult.JPEG_REACHED_SOS;
                break;
                case JpegState.DSTATE_PRELOAD:
                case JpegState.DSTATE_PRESCAN:
                case JpegState.DSTATE_SCANNING:
                case JpegState.DSTATE_RAW_OK:
                case JpegState.DSTATE_BUFIMAGE:
                case JpegState.DSTATE_BUFPOST:
                case JpegState.DSTATE_STOPPING:
                retcode = m_inputctl.ConsumeInput();
                break;
                default:
                ErrExit(JMessageCode.JERR_BAD_STATE, (int)globalState);
                break;
            }

            return retcode;
        }

        /// <summary>
        /// Pre-calculate output image dimensions and related values for current decompression parameters.
        /// </summary>
        /// <remarks>This is allowed for possible use by application. Hence it mustn't do anything 
        /// that can't be done twice. Also note that it may be called before the master module is initialized!
        /// </remarks>
        public void JpegCalcOutputDimensions()
        {
            // Do computations that are needed before master selection phase
            // This function is used for full decompression.
            /* Prevent application from calling me at wrong times */
            if (globalState != JpegState.DSTATE_READY)
            {
                ErrExit(JMessageCode.JERR_BAD_STATE, (int)globalState);
            }

            /* Compute core output image dimensions and DCT scaling choices. */
            m_inputctl.JpegCoreOutputDimensions();

            /* In selecting the actual DCT scaling for each component, we try to
            * scale up the chroma components via IDCT scaling rather than upsampling.
            * This saves time if the upsampler gets to use 1:1 scaling.
            * Note this code adapts subsampling ratios which are powers of 2.
            */
            for (var ci = 0; ci < numComponents; ci++)
            {
                var ssize = 1;

                var compptr = CompInfo[ci];
                var upSampleSize = doFancyUpsampling
                    ? JpegConstants.DCTSIZE
                    : JpegConstants.DCTSIZE / 2;
                while (min_DCT_h_scaled_size * ssize <= upSampleSize
                    && (m_max_h_samp_factor % (compptr.H_samp_factor * ssize * 2)) == 0)
                {
                    ssize *= 2;
                }

                compptr.DCT_h_scaled_size = min_DCT_h_scaled_size * ssize;

                ssize = 1;
                while (min_DCT_v_scaled_size * ssize <= upSampleSize
                   && (m_maxVSampleFactor % (compptr.V_samp_factor * ssize * 2)) == 0)
                {
                    ssize *= 2;
                }

                compptr.DCT_v_scaled_size = min_DCT_v_scaled_size * ssize;

                /* We don't support IDCT ratios larger than 2. */
                if (compptr.DCT_h_scaled_size > compptr.DCT_v_scaled_size * 2)
                {
                    compptr.DCT_h_scaled_size = compptr.DCT_v_scaled_size * 2;
                }
                else if (compptr.DCT_v_scaled_size > compptr.DCT_h_scaled_size * 2)
                {
                    compptr.DCT_v_scaled_size = compptr.DCT_h_scaled_size * 2;
                }
            }

            /* Recompute downsampled dimensions of components;
            * application needs to know these if using raw downsampled data.
            */
            for (var ci = 0; ci < numComponents; ci++)
            {
                /* Size in samples, after IDCT scaling */
                CompInfo[ci].downsampled_width = (int)JpegUtils.jdiv_round_up(
                    imageWidth * CompInfo[ci].H_samp_factor * CompInfo[ci].DCT_h_scaled_size,
                    m_max_h_samp_factor * block_size);

                CompInfo[ci].downsampled_height = (int)JpegUtils.jdiv_round_up(
                    imageHeight * CompInfo[ci].V_samp_factor * CompInfo[ci].DCT_v_scaled_size,
                    m_maxVSampleFactor * block_size);
            }

            /* Report number of components in selected colorspace. */
            /* Probably this should be in the color conversion module... */
            switch (outColorSpace)
            {
                case JColorSpace.JCS_GRAYSCALE:
                outColorComponents = 1;
                break;

                case JColorSpace.JCS_RGB:
                case JColorSpace.JCS_BG_RGB:
                outColorComponents = JpegConstants.RGB_PIXELSIZE;
                break;

                case JColorSpace.JCS_YCbCr:
                case JColorSpace.JCS_BG_YCC:
                outColorComponents = 3;
                break;

                case JColorSpace.JCS_CMYK:
                case JColorSpace.JCS_YCCK:
                outColorComponents = 4;
                break;

                default:
                /* else must be same colorspace as in file */
                outColorComponents = numComponents;
                break;
            }

            if (quantizeColors)
            {
                outputComponents = 1;
            }
            else
            {
                outputComponents = outColorComponents;
            }

            /* See if upsampler will want to emit more than one row at a time */
            if (UseMergedUpSample())
            {
                recommendOutBufferHeight = m_maxVSampleFactor;
            }
            else
            {
                recommendOutBufferHeight = 1;
            }
        }

        /// <summary>
        /// Read or write the raw DCT coefficient arrays from a JPEG file (useful for lossless transcoding).
        /// </summary>
        /// <returns>Returns <c>null</c> if suspended. This case need be checked only 
        /// if a suspending data source is used.
        /// </returns>
        /// <remarks>
        /// <para><see cref="JpegDecompressStruct.JpegReadHeader">jpeg_read_header</see> must be completed before calling this.<br/></para>
        /// <para>
        /// The entire image is read into a set of virtual coefficient-block arrays, one per component.
        /// The return value is an array of virtual-array descriptors.<br/>
        /// </para>
        /// <para>
        /// An alternative usage is to simply obtain access to the coefficient arrays during a 
        /// <see href="../articles/KB/buffered-image-mode.html">buffered-image mode</see> decompression operation. This is allowed after any 
        /// <see cref="JpegDecompressStruct.JpegFinishOutput">jpeg_finish_output</see> call. The arrays can be accessed 
        /// until <see cref="JpegDecompressStruct.JpegFinishDecompress">jpeg_finish_decompress</see> is called. 
        /// Note that any call to the library may reposition the arrays, 
        /// so don't rely on <see cref="JVirtArray{T}.Access"/> results to stay valid across library calls.
        /// </para>
        /// </remarks>
        public JVirtArray<JBlock>[] JpegReadCoefficients()
        {
            if (globalState == JpegState.DSTATE_READY)
            {
                /* First call: initialize active modules */
                TransdecodeMasterSelection();
                globalState = JpegState.DSTATE_RDCOEFS;
            }

            if (globalState == JpegState.DSTATE_RDCOEFS)
            {
                /* Absorb whole file into the coef buffer */
                while (true)
                {
                    ReadResult retcode;
                    /* Call progress monitor hook if present */
                    prog?.Updated();

                    /* Absorb some more input */
                    retcode = m_inputctl.ConsumeInput();
                    if (retcode == ReadResult.JPEG_SUSPENDED)
                    {
                        return null;
                    }

                    if (retcode == ReadResult.JPEG_REACHED_EOI)
                    {
                        break;
                    }

                    /* Advance progress counter if appropriate */
                    if (prog is object && (retcode == ReadResult.JPEG_ROW_COMPLETED || retcode == ReadResult.JPEG_REACHED_SOS))
                    {
                        prog.PassCounter++;
                        if (prog.PassCounter >= prog.PassLimit)
                        {
                            /* startup underestimated number of scans; ratchet up one scan */
                            prog.PassLimit += m_total_iMCU_rows;
                        }
                    }
                }

                /* Set state so that jpeg_finish_decompress does the right thing */
                globalState = JpegState.DSTATE_STOPPING;
            }

            /* At this point we should be in state DSTATE_STOPPING if being used
            * standalone, or in state DSTATE_BUFIMAGE if being invoked to get access
            * to the coefficients during a full buffered-image-mode decompression.
            */
            if ((globalState == JpegState.DSTATE_STOPPING || globalState == JpegState.DSTATE_BUFIMAGE) && bufferedImage)
            {
                return m_coef.GetCoefArrays();
            }

            /* Oops, improper usage */
            ErrExit(JMessageCode.JERR_BAD_STATE, (int)globalState);
            /* keep compiler happy */
            return null;
        }

        /// <summary>
        /// Initializes the compression object with default parameters, then copy from the source object 
        /// all parameters needed for lossless transcoding.
        /// </summary>
        /// <param name="dstinfo">Target JPEG compression object.</param>
        /// <remarks>Parameters that can be varied without loss (such as scan script and 
        /// Huffman optimization) are left in their default states.</remarks>
        public void JpegCopyCriticalParameters(JpegCompressStruct dstinfo)
        {
            if (dstinfo is null)
            {
                throw new ArgumentNullException(nameof(dstinfo));
            }
            /* Safety check to ensure start_compress not called yet. */
            if (dstinfo.globalState != JpegState.CSTATE_START)
            {
                ErrExit(JMessageCode.JERR_BAD_STATE, (int)dstinfo.globalState);
            }

            /* Copy fundamental image dimensions */
            dstinfo.m_image_width = imageWidth;
            dstinfo.m_image_height = imageHeight;
            dstinfo.m_input_components = numComponents;
            dstinfo.m_in_color_space = jpegColorSpace;
            dstinfo.jpeg_width = Output_width;
            dstinfo.jpeg_height = Output_height;
            dstinfo.min_DCT_h_scaled_size = min_DCT_h_scaled_size;
            dstinfo.min_DCT_v_scaled_size = min_DCT_v_scaled_size;

            /* Initialize all parameters to default values */
            dstinfo.JpegSetDefaults();

            /* jpeg_set_defaults may choose wrong colorspace, eg YCbCr if input is RGB.
             * Fix it to get the right header markers for the image colorspace.
             * Note: Entropy table assignment in jpeg_set_colorspace depends
             * on color_transform.
             */
            dstinfo.ColorTransform = color_transform;
            dstinfo.JpegSetColorspace(jpegColorSpace);
            dstinfo.m_data_precision = m_dataPrecision;
            dstinfo.CIR601sampling = m_CCIR601_sampling;

            /* Copy the source's quantization tables. */
            for (var tblno = 0; tblno < JpegConstants.NUM_QUANT_TBLS; tblno++)
            {
                if (m_quant_tbl_ptrs[tblno] is object)
                {
                    if (dstinfo.m_quant_tbl_ptrs[tblno] is null)
                    {
                        dstinfo.m_quant_tbl_ptrs[tblno] = new JQuantTable();
                    }

                    Buffer.BlockCopy(m_quant_tbl_ptrs[tblno].quantBal, 0,
                        dstinfo.m_quant_tbl_ptrs[tblno].quantBal, 0,
                        dstinfo.m_quant_tbl_ptrs[tblno].quantBal.Length * sizeof(short));

                    dstinfo.m_quant_tbl_ptrs[tblno].SentTable = false;
                }
            }

            /* Copy the source's per-component info.
            * Note we assume jpeg_set_defaults has allocated the dest comp_info array.
            */
            dstinfo.m_num_components = numComponents;
            if (dstinfo.m_num_components < 1 || dstinfo.m_num_components > JpegConstants.MAX_COMPONENTS)
            {
                ErrExit(JMessageCode.JERR_COMPONENT_COUNT, dstinfo.m_num_components, JpegConstants.MAX_COMPONENTS);
            }

            for (var ci = 0; ci < dstinfo.m_num_components; ci++)
            {
                dstinfo.Component_info[ci].Component_id = CompInfo[ci].Component_id;
                dstinfo.Component_info[ci].H_samp_factor = CompInfo[ci].H_samp_factor;
                dstinfo.Component_info[ci].V_samp_factor = CompInfo[ci].V_samp_factor;
                dstinfo.Component_info[ci].Quant_tbl_no = CompInfo[ci].Quant_tbl_no;

                /* Make sure saved quantization table for component matches the qtable
                * slot.  If not, the input file re-used this qtable slot.
                * IJG encoder currently cannot duplicate this.
                */
                var tblno = dstinfo.Component_info[ci].Quant_tbl_no;
                if (tblno < 0 || tblno >= JpegConstants.NUM_QUANT_TBLS || m_quant_tbl_ptrs[tblno] is null)
                {
                    ErrExit(JMessageCode.JERR_NO_QUANT_TABLE, tblno);
                }

                var c_quant = CompInfo[ci].quant_table;
                if (c_quant is object)
                {
                    var slot_quant = m_quant_tbl_ptrs[tblno];
                    for (var coefi = 0; coefi < JpegConstants.DCTSIZE2; coefi++)
                    {
                        if (c_quant.quantBal[coefi] != slot_quant.quantBal[coefi])
                        {
                            ErrExit(JMessageCode.JERR_MISMATCHED_QUANT_TABLE, tblno);
                        }
                    }
                }
                /* Note: we do not copy the source's entropy table assignments;
                * instead we rely on jpeg_set_colorspace to have made a suitable choice.
                */
            }

            /* Also copy JFIF version and resolution information, if available.
            * Strictly speaking this isn't "critical" info, but it's nearly
            * always appropriate to copy it if available.  In particular,
            * if the application chooses to copy JFIF 1.02 extension markers from
            * the source file, we need to copy the version to make sure we don't
            * emit a file that has 1.02 extensions but a claimed version of 1.01.
            */
            if (m_saw_JFIF_marker)
            {
                if (m_JFIF_major_version == 1 || m_JFIF_major_version == 2)
                {
                    dstinfo.m_JFIF_major_version = m_JFIF_major_version;
                    dstinfo.m_JFIF_minor_version = m_JFIF_minor_version;
                }

                dstinfo.m_density_unit = m_densityUnit;
                dstinfo.m_X_density = m_xDensity;
                dstinfo.m_Y_density = m_yDensity;
            }
        }

        /// <summary>
        /// Aborts processing of a JPEG decompression operation.
        /// </summary>
        /// <seealso cref="JpegCommonStruct.JpegAbort"/>
        public void JpegAbortDecompress()
        {
            JpegAbort();
        }

        /// <summary>
        /// Sets processor for special marker.
        /// </summary>
        /// <param name="marker_code">The marker code.</param>
        /// <param name="routine">The processor.</param>
        /// <remarks>Allows you to supply your own routine to process 
        /// COM and/or APPn markers on-the-fly as they are read.
        /// </remarks>
        /// <seealso href="../articles/KB/special-markers.html">Special markers</seealso>
        public void JpegSetMarkerProcessor(int marker_code, JpegMarkerParserMethod routine)
        {
            m_marker.JpegSetMarkerProcessor(marker_code, routine);
        }

        /// <summary>
        /// Control saving of COM and APPn markers into <see cref="JpegDecompressStruct.Marker_list">Marker_list</see>.
        /// </summary>
        /// <param name="marker_code">The marker type to save (see JPEG_MARKER enumeration).<br/>
        /// To arrange to save all the special marker types, you need to call this 
        /// routine 17 times, for COM and APP0-APP15 markers.</param>
        /// <param name="length_limit">If the incoming marker is longer than <c>length_limit</c> data bytes, 
        /// only <c>length_limit</c> bytes will be saved; this parameter allows you to avoid chewing up memory 
        /// when you only need to see the first few bytes of a potentially large marker. If you want to save 
        /// all the data, set <c>length_limit</c> to 0xFFFF; that is enough since marker lengths are only 16 bits. 
        /// As a special case, setting <c>length_limit</c> to 0 prevents that marker type from being saved at all. 
        /// (That is the default behavior, in fact.)
        /// </param>
        /// <seealso cref="JpegDecompressStruct.Marker_list"/>
        /// <seealso href="../articles/KB/special-markers.html">Special markers</seealso>
        public void JpegSaveMarkers(int marker_code, int length_limit)
        {
            m_marker.JpegSaveMarkers(marker_code, length_limit);
        }

        /// <summary>
        /// Determine whether merged upsample/color conversion should be used.
        /// CRUCIAL: this must match the actual capabilities of merged upsampler!
        /// </summary>
        internal bool UseMergedUpSample()
        {
            /* Merging is the equivalent of plain box-filter upsampling. */
            /* The following condition is only needed if fancy shall select
             * a different upsampling method.  In our current implementation
             * fancy only affects the DCT scaling, thus we can use fancy
             * upsampling and merged upsample simultaneously, in particular
             * with scaled DCT sizes larger than the default DCTSIZE.
             */
            if (m_CCIR601_sampling)
            {
                return false;
            }

            /* my_upsampler only supports YCC=>RGB color conversion */
            if ((jpegColorSpace != JColorSpace.JCS_YCbCr && jpegColorSpace != JColorSpace.JCS_BG_YCC)
                || numComponents != 3
                || outColorSpace != JColorSpace.JCS_RGB
                || outColorComponents != JpegConstants.RGB_PIXELSIZE
                || color_transform != JColorTransform.JCT_NONE)
            {
                return false;
            }

            /* and it only handles 2h1v or 2h2v sampling ratios */
            if (CompInfo[0].H_samp_factor != 2
                || CompInfo[1].H_samp_factor != 1
                || CompInfo[2].H_samp_factor != 1
                || CompInfo[0].V_samp_factor > 2
                || CompInfo[1].V_samp_factor != 1
                || CompInfo[2].V_samp_factor != 1)
            {
                return false;
            }

            /* furthermore, it doesn't work if we've scaled the IDCTs differently */
            if (CompInfo[0].DCT_h_scaled_size != min_DCT_h_scaled_size
                || CompInfo[1].DCT_h_scaled_size != min_DCT_h_scaled_size
                || CompInfo[2].DCT_h_scaled_size != min_DCT_h_scaled_size
                || CompInfo[0].DCT_v_scaled_size != min_DCT_v_scaled_size
                || CompInfo[1].DCT_v_scaled_size != min_DCT_v_scaled_size
                || CompInfo[2].DCT_v_scaled_size != min_DCT_v_scaled_size)
            {
                return false;
            }

            /* ??? also need to test for upsample-time rescaling, when & if supported */
            /* by golly, it'll work... */
            return true;
        }

        /// <summary>
        /// Initialization of JPEG compression objects.
        /// The error manager must already be set up (in case memory manager fails).
        /// </summary>
        private void Initialize()
        {
            /* Zero out pointers to permanent structures. */
            prog = null;
            m_src = null;

            for (var i = 0; i < JpegConstants.NUM_QUANT_TBLS; i++)
            {
                m_quant_tbl_ptrs[i] = null;
            }

            for (var i = 0; i < JpegConstants.NUM_HUFF_TBLS; i++)
            {
                m_dc_huff_tbl_ptrs[i] = null;
                m_ac_huff_tbl_ptrs[i] = null;
            }

            /* Initialize marker processor so application can override methods
            * for COM, APPn markers before calling jpeg_read_header.
            */
            m_marker_list = new List<JpegMarkerStruct>();
            m_marker = new JpegMarkerReader(this);

            /* And initialize the overall input controller. */
            m_inputctl = new JpegInputController(this);

            /* OK, I'm ready */
            globalState = JpegState.DSTATE_START;
        }

        /// <summary>
        /// Master selection of decompression modules for transcoding (that is, reading 
        /// raw DCT coefficient arrays from an input JPEG file.)
        /// This substitutes for initialization of the full decompressor.
        /// </summary>
        private void TransdecodeMasterSelection()
        {
            /* This is effectively a buffered-image operation. */
            bufferedImage = true;

            /* Compute output image dimensions and related values. */
            m_inputctl.JpegCoreOutputDimensions();

            if (arithCode)
            {
                m_entropy = new ArithEntropyDecoder(this);
            }
            else
            {
                m_entropy = new HuffmanEntropyDecoder(this);
            }

            /* Always get a full-image coefficient buffer. */
            m_coef = new JpegDCoefController(this, true);

            /* Initialize input side of decompressor to consume first scan. */
            m_inputctl.StartInputPass();

            /* Initialize progress monitoring. */
            if (prog is object)
            {
                var nscans = 1;
                /* Estimate number of scans to set pass_limit. */
                if (progressiveMode)
                {
                    /* Arbitrarily estimate 2 interleaved DC scans + 3 AC scans/component. */
                    nscans = 2 + (3 * numComponents);
                }
                else if (m_inputctl.HasMultipleScans())
                {
                    /* For a nonprogressive multiscan file, estimate 1 scan per component. */
                    nscans = numComponents;
                }

                prog.PassCounter = 0;
                prog.PassLimit = m_total_iMCU_rows * nscans;
                prog.CompletedPasses = 0;
                prog.TotalPasses = 1;
            }
        }

        /// <summary>
        /// Set up for an output pass, and perform any dummy pass(es) needed.
        /// Common subroutine for jpeg_start_decompress and jpeg_start_output.
        /// Entry: global_state = DSTATE_PRESCAN only if previously suspended.
        /// Exit: If done, returns true and sets global_state for proper output mode.
        ///       If suspended, returns false and sets global_state = DSTATE_PRESCAN.
        /// </summary>
        private bool OutputPassSetup()
        {
            if (globalState != JpegState.DSTATE_PRESCAN)
            {
                /* First call: do pass setup */
                m_master.PrepareForOutputPass();
                outputScanline = 0;
                globalState = JpegState.DSTATE_PRESCAN;
            }

            /* Loop over any required dummy passes */
            while (m_master.IsDummyPass())
            {
                /* Crank through the dummy pass */
                while (outputScanline < outputHeight)
                {
                    int last_scanline;
                    /* Call progress monitor hook if present */
                    if (prog is object)
                    {
                        prog.PassCounter = outputScanline;
                        prog.PassLimit = outputHeight;
                        prog.Updated();
                    }

                    /* Process some data */
                    last_scanline = outputScanline;
                    m_main.ProcessData(null, ref outputScanline, 0);
                    if (outputScanline == last_scanline)
                    {
                        /* No progress made, must suspend */
                        return false;
                    }
                }

                /* Finish up dummy pass, and set up for another one */
                m_master.FinishOutputPass();
                m_master.PrepareForOutputPass();
                outputScanline = 0;
            }

            /* Ready for application to drive output pass through
            * jpeg_read_scanlines or jpeg_read_raw_data.
            */
            if (rawDataOut)
            {
                globalState = JpegState.DSTATE_RAW_OK;
            }
            else
            {
                globalState = JpegState.DSTATE_SCANNING;
            }

            return true;
        }

        /// <summary>
        /// Set default decompression parameters.
        /// </summary>
        private void DefaultDecompressParams()
        {
            /* Guess the input colorspace, and set output colorspace accordingly. */
            /* Note application may override our guesses. */
            switch (numComponents)
            {
                case 1:
                jpegColorSpace = JColorSpace.JCS_GRAYSCALE;
                outColorSpace = JColorSpace.JCS_GRAYSCALE;
                break;

                case 3:
                var cid0 = CompInfo[0].Component_id;
                var cid1 = CompInfo[1].Component_id;
                var cid2 = CompInfo[2].Component_id;

                // Use Adobe marker info, otherwise try to guess from the component IDs
                if (m_saw_Adobe_marker)
                {
                    switch (m_Adobe_transform)
                    {
                        case 0:
                        jpegColorSpace = JColorSpace.JCS_RGB;
                        break;
                        case 1:
                        jpegColorSpace = JColorSpace.JCS_YCbCr;
                        break;
                        default:
                        WarnMS(JMessageCode.JWRN_ADOBE_XFORM, m_Adobe_transform);
                        /* assume it's YCbCr */
                        jpegColorSpace = JColorSpace.JCS_YCbCr;
                        break;
                    }
                }
                else if (cid0 == 0x01 && cid1 == 0x02 && cid2 == 0x03)
                {
                    jpegColorSpace = JColorSpace.JCS_YCbCr;
                }
                else if (cid0 == 0x01 && cid1 == 0x22 && cid2 == 0x23)
                {
                    jpegColorSpace = JColorSpace.JCS_BG_YCC;
                }
                else if (cid0 == 0x52 && cid1 == 0x47 && cid2 == 0x42)
                {
                    /* ASCII 'R', 'G', 'B' */
                    jpegColorSpace = JColorSpace.JCS_RGB;
                }
                else if (cid0 == 0x72 && cid1 == 0x67 && cid2 == 0x62)
                {
                    /* ASCII 'r', 'g', 'b' */
                    jpegColorSpace = JColorSpace.JCS_BG_RGB;
                }
                else if (m_saw_JFIF_marker)
                {
                    /* assume it's YCbCr */
                    jpegColorSpace = JColorSpace.JCS_YCbCr;
                }
                else
                {
                    TraceMS(1, JMessageCode.JTRC_UNKNOWN_IDS, cid0, cid1, cid2);
                    /* assume it's YCbCr */
                    jpegColorSpace = JColorSpace.JCS_YCbCr;
                }
                /* Always guess RGB is proper output colorspace. */
                outColorSpace = JColorSpace.JCS_RGB;
                break;

                case 4:
                if (m_saw_Adobe_marker)
                {
                    switch (m_Adobe_transform)
                    {
                        case 0:
                        jpegColorSpace = JColorSpace.JCS_CMYK;
                        break;
                        case 2:
                        jpegColorSpace = JColorSpace.JCS_YCCK;
                        break;
                        default:
                        WarnMS(JMessageCode.JWRN_ADOBE_XFORM, m_Adobe_transform);
                        /* assume it's YCCK */
                        jpegColorSpace = JColorSpace.JCS_YCCK;
                        break;
                    }
                }
                else
                {
                    /* No special markers, assume straight CMYK. */
                    jpegColorSpace = JColorSpace.JCS_CMYK;
                }

                outColorSpace = JColorSpace.JCS_CMYK;
                break;

                default:
                jpegColorSpace = JColorSpace.JCS_UNKNOWN;
                outColorSpace = JColorSpace.JCS_UNKNOWN;
                break;
            }

            /* Set defaults for other decompression parameters. */
            scaleNum = block_size;       /* 1:1 scaling */
            scaleDenom = block_size;
            bufferedImage = false;
            rawDataOut = false;
            dctMethod = JpegConstants.JDCT_DEFAULT;
            doFancyUpsampling = true;
            doBlockSmoothing = true;
            quantizeColors = false;

            /* We set these in case application only sets quantize_colors. */
            ditherMode = JDitherMode.JDITHER_FS;
            twoPassQuantize = true;
            desiredNumberOfColors = 256;
            m_colormap = null;

            /* Initialize for no mode change in buffered-image mode. */
            enable1PassQuant = false;
            enableExternalQuant = false;
            enable2PassQuant = false;
        }

        private void JpegConsumeInputStart()
        {
            /* Start-of-datastream actions: reset appropriate modules */
            m_inputctl.ResetInputController();

            /* Initialize application's data source module */
            m_src.InitSource();
            globalState = JpegState.DSTATE_INHEADER;
        }

        private ReadResult JpegConsumeInputInHeader()
        {
            var retcode = m_inputctl.ConsumeInput();
            if (retcode == ReadResult.JPEG_REACHED_SOS)
            {
                /* Found SOS, prepare to decompress */
                /* Set up default parameters based on header data */
                DefaultDecompressParams();

                /* Set global state: ready for start_decompress */
                globalState = JpegState.DSTATE_READY;
            }

            return retcode;
        }
    }
}
