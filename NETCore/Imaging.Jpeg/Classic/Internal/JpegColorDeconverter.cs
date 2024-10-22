/*
 * This file contains output colorspace conversion routines.
 */

namespace BitMiracle.LibJpeg.Classic.Internal;

/// <summary>
/// Colorspace conversion
/// </summary>
internal class JpegColorDeconverter
{
    private const int SCALEBITS = 16;  /* speediest right-shift on some machines */
    private const int ONE_HALF = 1 << (SCALEBITS - 1);

    /* We allocate one big table for RGB->Y conversion and divide it up into
     * three parts, instead of doing three alloc_small requests.  This lets us
     * use a single table base address, which can be held in a register in the
     * inner loops on many machines (more than can hold all three addresses,
     * anyway).
     */

    private const int R_Y_OFF = 0; 			/* offset to R => Y section */
    private const int G_Y_OFF = (1 * (JpegConstants.MAXJSAMPLE + 1));	/* offset to G => Y section */
    private const int B_Y_OFF = (2 * (JpegConstants.MAXJSAMPLE + 1));	/* etc. */
    private const int TABLE_SIZE = (3 * (JpegConstants.MAXJSAMPLE + 1));

    private delegate void color_convert_func(ComponentBuffer[] input_buf, int input_row, byte[][] output_buf, int output_row, int num_rows);
    private readonly color_convert_func m_converter;

    private readonly JpegDecompressStruct m_cinfo;

    private int[] m_perComponentOffsets;

    /* Private state for YCbCr->RGB and BG_YCC->RGB conversion */
    private int[] m_Cr_r_tab;      /* => table for Cr to R conversion */
    private int[] m_Cb_b_tab;      /* => table for Cb to B conversion */
    private int[] m_Cr_g_tab;        /* => table for Cr to G conversion */
    private int[] m_Cb_g_tab;        /* => table for Cb to G conversion */

    /* Private state for RGB->Y conversion */
    private int[] rgb_y_tab;        /* => table for RGB to Y conversion */

    /// <summary>
    /// Module initialization routine for output colorspace conversion.
    /// </summary>
    public JpegColorDeconverter(JpegDecompressStruct cinfo)
    {
        m_cinfo = cinfo;

        /* Make sure num_components agrees with jpeg_color_space */
        switch (cinfo.jpegColorSpace)
        {
            case JColorSpace.JCS_GRAYSCALE:
            if (cinfo.numComponents != 1)
            {
                cinfo.ErrExit(JMessageCode.JERR_BAD_J_COLORSPACE);
            }

            break;

            case JColorSpace.JCS_RGB:
            case JColorSpace.JCS_YCbCr:
            case JColorSpace.JCS_BG_RGB:
            case JColorSpace.JCS_BG_YCC:
            if (cinfo.numComponents != 3)
            {
                cinfo.ErrExit(JMessageCode.JERR_BAD_J_COLORSPACE);
            }

            break;

            case JColorSpace.JCS_CMYK:
            case JColorSpace.JCS_YCCK:
            if (cinfo.numComponents != 4)
            {
                cinfo.ErrExit(JMessageCode.JERR_BAD_J_COLORSPACE);
            }

            break;

            case JColorSpace.JCS_NCHANNEL:
            if (cinfo.numComponents < 1)
            {
                cinfo.ErrExit(JMessageCode.JERR_BAD_J_COLORSPACE);
            }

            break;

            default:
            /* JCS_UNKNOWN can be anything */
            if (cinfo.numComponents < 1)
            {
                cinfo.ErrExit(JMessageCode.JERR_BAD_J_COLORSPACE);
            }

            break;
        }

        /* Support color transform only for RGB colorspaces */
        if (cinfo.color_transform != JColorTransform.JCT_NONE &&
            cinfo.jpegColorSpace != JColorSpace.JCS_RGB &&
            cinfo.jpegColorSpace != JColorSpace.JCS_BG_RGB)
        {
            cinfo.ErrExit(JMessageCode.JERR_CONVERSION_NOTIMPL);
        }

        /* Set out_color_components and conversion method based on requested space.
        * Also clear the component_needed flags for any unused components,
        * so that earlier pipeline stages can avoid useless computation.
        */

        switch (cinfo.outColorSpace)
        {
            case JColorSpace.JCS_GRAYSCALE:
            cinfo.outColorComponents = 1;
            switch (cinfo.jpegColorSpace)
            {
                case JColorSpace.JCS_GRAYSCALE:
                case JColorSpace.JCS_YCbCr:
                case JColorSpace.JCS_BG_YCC:
                m_converter = GrayscaleConvert;
                /* For color->grayscale conversion, only the Y (0) component is needed */
                for (var ci = 1; ci < cinfo.numComponents; ci++)
                {
                    cinfo.CompInfo[ci].component_needed = false;
                }

                break;

                case JColorSpace.JCS_RGB:
                switch (cinfo.color_transform)
                {
                    case JColorTransform.JCT_NONE:
                    m_converter = RgbGrayConvert;
                    break;

                    case JColorTransform.JCT_SUBTRACT_GREEN:
                    m_converter = Rgb1GrayConvert;
                    break;

                    default:
                    cinfo.ErrExit(JMessageCode.JERR_CONVERSION_NOTIMPL);
                    break;
                }

                BuildRgbYTable();
                break;

                default:
                cinfo.ErrExit(JMessageCode.JERR_CONVERSION_NOTIMPL);
                break;
            }
            break;

            case JColorSpace.JCS_RGB:
            cinfo.outColorComponents = JpegConstants.RGB_PIXELSIZE;
            switch (cinfo.jpegColorSpace)
            {
                case JColorSpace.JCS_GRAYSCALE:
                m_converter = GrayRgbConvert;
                break;

                case JColorSpace.JCS_YCbCr:
                m_converter = YccRgbConvert;
                BuildYccRgbTable();
                break;

                case JColorSpace.JCS_BG_YCC:
                m_converter = YccRgbConvert;
                BuildBgYccRgbTable();
                break;

                case JColorSpace.JCS_RGB:
                switch (cinfo.color_transform)
                {
                    case JColorTransform.JCT_NONE:
                    m_converter = RgbConvert;
                    break;

                    case JColorTransform.JCT_SUBTRACT_GREEN:
                    m_converter = Rgb1RgbConvert;
                    break;

                    default:
                    cinfo.ErrExit(JMessageCode.JERR_CONVERSION_NOTIMPL);
                    break;
                }
                break;

                case JColorSpace.JCS_CMYK:
                m_converter = CmykRgbConvert;
                break;

                case JColorSpace.JCS_YCCK:
                m_converter = YcckRgbConvert;
                BuildYccRgbTable();
                break;

                default:
                cinfo.ErrExit(JMessageCode.JERR_CONVERSION_NOTIMPL);
                break;
            }
            break;

            case JColorSpace.JCS_BG_RGB:
            cinfo.outColorComponents = JpegConstants.RGB_PIXELSIZE;
            if (cinfo.jpegColorSpace == JColorSpace.JCS_BG_RGB)
            {
                switch (cinfo.color_transform)
                {
                    case JColorTransform.JCT_NONE:
                    m_converter = RgbConvert;
                    break;

                    case JColorTransform.JCT_SUBTRACT_GREEN:
                    m_converter = Rgb1RgbConvert;
                    break;

                    default:
                    cinfo.ErrExit(JMessageCode.JERR_CONVERSION_NOTIMPL);
                    break;
                }
            }
            else
            {
                cinfo.ErrExit(JMessageCode.JERR_CONVERSION_NOTIMPL);
            }
            break;

            case JColorSpace.JCS_CMYK:
            cinfo.outColorComponents = 4;
            switch (cinfo.jpegColorSpace)
            {
                case JColorSpace.JCS_YCCK:
                m_converter = YcckCmykConvert;
                BuildYccRgbTable();
                break;

                case JColorSpace.JCS_CMYK:
                m_converter = NullConvert;
                break;

                default:
                cinfo.ErrExit(JMessageCode.JERR_CONVERSION_NOTIMPL);
                break;
            }
            break;

            case JColorSpace.JCS_NCHANNEL:
            if (cinfo.jpegColorSpace == JColorSpace.JCS_NCHANNEL)
            {
                m_converter = NullConvert;
            }
            else
            {
                cinfo.ErrExit(JMessageCode.JERR_CONVERSION_NOTIMPL);
            }

            break;

            default:
            /* Permit null conversion to same output space */
            if (cinfo.outColorSpace == cinfo.jpegColorSpace)
            {
                cinfo.outColorComponents = cinfo.numComponents;
                m_converter = NullConvert;
            }
            else
            {
                /* unsupported non-null conversion */
                cinfo.ErrExit(JMessageCode.JERR_CONVERSION_NOTIMPL);
            }
            break;
        }

        if (cinfo.quantizeColors)
        {
            cinfo.outputComponents = 1; /* single colormapped output component */
        }
        else
        {
            cinfo.outputComponents = cinfo.outColorComponents;
        }
    }

    /// <summary>
    /// <para>Convert some rows of samples to the output colorspace.</para>
    /// <para>
    /// Note that we change from noninterleaved, one-plane-per-component format
    /// to interleaved-pixel format.  The output buffer is therefore three times
    /// as wide as the input buffer.
    /// A starting row offset is provided only for the input buffer.  The caller
    /// can easily adjust the passed output_buf value to accommodate any row
    /// offset required on that side.
    /// </para>
    /// </summary>
    public void ColorConvert(ComponentBuffer[] input_buf, int[] perComponentOffsets, int input_row, byte[][] output_buf, int output_row, int num_rows)
    {
        m_perComponentOffsets = perComponentOffsets;
        m_converter(input_buf, input_row, output_buf, output_row, num_rows);
    }

    /**************** YCbCr -> RGB conversion: most common case **************/
    /*************** BG_YCC -> RGB conversion: less common case **************/
    /***************    RGB -> Y   conversion: less common case **************/

    /*
     * YCbCr is defined per Recommendation ITU-R BT.601-7 (03/2011),
     * previously known as Recommendation CCIR 601-1, except that Cb and Cr
     * are normalized to the range 0..MAXJSAMPLE rather than -0.5 .. 0.5.
     * sRGB (standard RGB color space) is defined per IEC 61966-2-1:1999.
     * sYCC (standard luma-chroma-chroma color space with extended gamut)
     * is defined per IEC 61966-2-1:1999 Amendment A1:2003 Annex F.
     * bg-sRGB and bg-sYCC (big gamut standard color spaces)
     * are defined per IEC 61966-2-1:1999 Amendment A1:2003 Annex G.
     * Note that the derived conversion coefficients given in some of these
     * documents are imprecise.  The general conversion equations are
     *
     *  R = Y + K * (1 - Kr) * Cr
     *  G = Y - K * (Kb * (1 - Kb) * Cb + Kr * (1 - Kr) * Cr) / (1 - Kr - Kb)
     *  B = Y + K * (1 - Kb) * Cb
     *
     *  Y = Kr * R + (1 - Kr - Kb) * G + Kb * B
     *
     * With Kr = 0.299 and Kb = 0.114 (derived according to SMPTE RP 177-1993
     * from the 1953 FCC NTSC primaries and CIE Illuminant C), K = 2 for sYCC,
     * the conversion equations to be implemented are therefore
     *
     *  R = Y + 1.402 * Cr
     *  G = Y - 0.344136286 * Cb - 0.714136286 * Cr
     *  B = Y + 1.772 * Cb
     *
     *  Y = 0.299 * R + 0.587 * G + 0.114 * B
     *
     * where Cb and Cr represent the incoming values less CENTERJSAMPLE.
     * For bg-sYCC, with K = 4, the equations are
     *
     *  R = Y + 2.804 * Cr
     *  G = Y - 0.688272572 * Cb - 1.428272572 * Cr
     *  B = Y + 3.544 * Cb
     *
     * To avoid floating-point arithmetic, we represent the fractional constants
     * as integers scaled up by 2^16 (about 4 digits precision); we have to divide
     * the products by 2^16, with appropriate rounding, to get the correct answer.
     * Notice that Y, being an integral input, does not contribute any fraction
     * so it need not participate in the rounding.
     *
     * For even more speed, we avoid doing any multiplications in the inner loop
     * by precalculating the constants times Cb and Cr for all possible values.
     * For 8-bit JSAMPLEs this is very reasonable (only 256 entries per table);
     * for 9-bit to 12-bit samples it is still acceptable.  It's not very
     * reasonable for 16-bit samples, but if you want lossless storage you
     * shouldn't be changing colorspace anyway.
     * The Cr=>R and Cb=>B values can be rounded to integers in advance; the
     * values for the G calculation are left scaled up, since we must add them
     * together before rounding.
     */

    /// <summary>
    /// Initialize tables for YCbCr->RGB colorspace conversion.
    /// </summary>
    private void BuildYccRgbTable()
    {
        /* Normal case, sYCC */
        m_Cr_r_tab = new int[JpegConstants.MAXJSAMPLE + 1];
        m_Cb_b_tab = new int[JpegConstants.MAXJSAMPLE + 1];
        m_Cr_g_tab = new int[JpegConstants.MAXJSAMPLE + 1];
        m_Cb_g_tab = new int[JpegConstants.MAXJSAMPLE + 1];

        for (int i = 0, x = -JpegConstants.CENTERJSAMPLE; i <= JpegConstants.MAXJSAMPLE; i++, x++)
        {
            /* i is the actual input pixel value, in the range 0..MAXJSAMPLE */
            /* The Cb or Cr value we are thinking of is x = i - CENTERJSAMPLE */
            /* Cr=>R value is nearest int to 1.402 * x */
            m_Cr_r_tab[i] = JpegUtils.RIGHT_SHIFT((FIX(1.402) * x) + ONE_HALF, SCALEBITS);

            /* Cb=>B value is nearest int to 1.772 * x */
            m_Cb_b_tab[i] = JpegUtils.RIGHT_SHIFT((FIX(1.772) * x) + ONE_HALF, SCALEBITS);

            /* Cr=>G value is scaled-up -0.714136286 * x */
            m_Cr_g_tab[i] = -FIX(0.714136286) * x;

            /* Cb=>G value is scaled-up -0.344136286 * x */
            /* We also add in ONE_HALF so that need not do it in inner loop */
            m_Cb_g_tab[i] = (-FIX(0.344136286) * x) + ONE_HALF;
        }
    }

    /// <summary>
    /// Initialize tables for BG_YCC->RGB colorspace conversion.
    /// </summary>
    private void BuildBgYccRgbTable()
    {
        /* Wide gamut case, bg-sYCC */
        m_Cr_r_tab = new int[JpegConstants.MAXJSAMPLE + 1];
        m_Cb_b_tab = new int[JpegConstants.MAXJSAMPLE + 1];
        m_Cr_g_tab = new int[JpegConstants.MAXJSAMPLE + 1];
        m_Cb_g_tab = new int[JpegConstants.MAXJSAMPLE + 1];

        for (int i = 0, x = -JpegConstants.CENTERJSAMPLE; i <= JpegConstants.MAXJSAMPLE; i++, x++)
        {
            /* i is the actual input pixel value, in the range 0..MAXJSAMPLE */
            /* The Cb or Cr value we are thinking of is x = i - CENTERJSAMPLE */
            /* Cr=>R value is nearest int to 2.804 * x */
            m_Cr_r_tab[i] = JpegUtils.RIGHT_SHIFT((FIX(2.804) * x) + ONE_HALF, SCALEBITS);

            /* Cb=>B value is nearest int to 3.544 * x */
            m_Cb_b_tab[i] = JpegUtils.RIGHT_SHIFT((FIX(3.544) * x) + ONE_HALF, SCALEBITS);

            /* Cr=>G value is scaled-up -1.428272572 * x */
            m_Cr_g_tab[i] = -FIX(1.428272572) * x;

            /* Cb=>G value is scaled-up -0.688272572 * x */
            /* We also add in ONE_HALF so that need not do it in inner loop */
            m_Cb_g_tab[i] = (-FIX(0.688272572) * x) + ONE_HALF;
        }
    }

    private void YccRgbConvert(ComponentBuffer[] input_buf, int input_row, byte[][] output_buf, int output_row, int num_rows)
    {
        var component0RowOffset = m_perComponentOffsets[0];
        var component1RowOffset = m_perComponentOffsets[1];
        var component2RowOffset = m_perComponentOffsets[2];

        var limit = m_cinfo.m_sample_range_limit;
        var limitOffset = m_cinfo.m_sampleRangeLimitOffset;

        for (var row = 0; row < num_rows; row++)
        {
            var columnOffset = 0;
            for (var col = 0; col < m_cinfo.outputWidth; col++)
            {
                int y = input_buf[0][input_row + component0RowOffset][col];
                int cb = input_buf[1][input_row + component1RowOffset][col];
                int cr = input_buf[2][input_row + component2RowOffset][col];

                /* Range-limiting is essential due to noise introduced by DCT losses.
                 * for extended gamut (sYCC) and wide gamut (bg-sYCC) encodings.
                 */
                output_buf[output_row + row][columnOffset + JpegConstants.RGB_RED] = limit[limitOffset + y + m_Cr_r_tab[cr]];
                output_buf[output_row + row][columnOffset + JpegConstants.RGB_GREEN] = limit[limitOffset + y + JpegUtils.RIGHT_SHIFT(m_Cb_g_tab[cb] + m_Cr_g_tab[cr], SCALEBITS)];
                output_buf[output_row + row][columnOffset + JpegConstants.RGB_BLUE] = limit[limitOffset + y + m_Cb_b_tab[cb]];
                columnOffset += JpegConstants.RGB_PIXELSIZE;
            }

            input_row++;
        }
    }

    /**************** Cases other than YCC -> RGB **************/

    /*
     * Initialize for RGB->grayscale colorspace conversion.
     */
    private void BuildRgbYTable()
    {
        /* Allocate and fill in the conversion tables. */
        rgb_y_tab = new int[TABLE_SIZE];

        for (var i = 0; i <= JpegConstants.MAXJSAMPLE; i++)
        {
            rgb_y_tab[i + R_Y_OFF] = FIX(0.299) * i;
            rgb_y_tab[i + G_Y_OFF] = FIX(0.587) * i;
            rgb_y_tab[i + B_Y_OFF] = (FIX(0.114) * i) + ONE_HALF;
        }
    }

    /*
     * Convert RGB to grayscale.
     */
    private void RgbGrayConvert(ComponentBuffer[] input_buf, int input_row, byte[][] output_buf, int output_row, int num_rows)
    {
        var component0RowOffset = m_perComponentOffsets[0];
        var component1RowOffset = m_perComponentOffsets[1];
        var component2RowOffset = m_perComponentOffsets[2];

        var num_cols = m_cinfo.outputWidth;

        for (var row = 0; row < num_rows; row++)
        {
            var columnOffset = 0;
            for (var col = 0; col < num_cols; col++)
            {
                int r = input_buf[0][input_row + component0RowOffset][col];
                int g = input_buf[1][input_row + component1RowOffset][col];
                int b = input_buf[2][input_row + component2RowOffset][col];

                /* Y */
                output_buf[output_row + row][columnOffset++] = (byte)((rgb_y_tab[r + R_Y_OFF] + rgb_y_tab[g + G_Y_OFF] + rgb_y_tab[b + B_Y_OFF]) >> SCALEBITS);
            }
        }
    }

    /*
     * [R-G,G,B-G] to [R,G,B] conversion with modulo calculation
     * (inverse color transform).
     * This can be seen as an adaption of the general YCbCr->RGB
     * conversion equation with Kr = Kb = 0, while replacing the
     * normalization by modulo calculation.
     */
    private void Rgb1RgbConvert(ComponentBuffer[] input_buf, int input_row, byte[][] output_buf, int output_row, int num_rows)
    {
        var component0RowOffset = m_perComponentOffsets[0];
        var component1RowOffset = m_perComponentOffsets[1];
        var component2RowOffset = m_perComponentOffsets[2];

        var num_cols = m_cinfo.outputWidth;

        for (var row = 0; row < num_rows; row++)
        {
            var columnOffset = 0;
            for (var col = 0; col < num_cols; col++)
            {
                int r = input_buf[0][input_row + component0RowOffset][col];
                int g = input_buf[1][input_row + component1RowOffset][col];
                int b = input_buf[2][input_row + component2RowOffset][col];

                /* Assume that MAXJSAMPLE+1 is a power of 2, so that the MOD
                 * (modulo) operator is equivalent to the bitmask operator AND.
                 */
                output_buf[output_row + row][columnOffset + JpegConstants.RGB_RED] = (byte)((r + g - JpegConstants.CENTERJSAMPLE) & JpegConstants.MAXJSAMPLE);
                output_buf[output_row + row][columnOffset + JpegConstants.RGB_GREEN] = (byte)g;
                output_buf[output_row + row][columnOffset + JpegConstants.RGB_BLUE] = (byte)((b + g - JpegConstants.CENTERJSAMPLE) & JpegConstants.MAXJSAMPLE);
                columnOffset += JpegConstants.RGB_PIXELSIZE;
            }
        }
    }

    /*
     * [R-G,G,B-G] to grayscale conversion with modulo calculation
     * (inverse color transform).
     */
    private void Rgb1GrayConvert(ComponentBuffer[] input_buf, int input_row, byte[][] output_buf, int output_row, int num_rows)
    {
        var component0RowOffset = m_perComponentOffsets[0];
        var component1RowOffset = m_perComponentOffsets[1];
        var component2RowOffset = m_perComponentOffsets[2];

        var num_cols = m_cinfo.outputWidth;

        for (var row = 0; row < num_rows; row++)
        {
            var columnOffset = 0;
            for (var col = 0; col < num_cols; col++)
            {
                int r = input_buf[0][input_row + component0RowOffset][col];
                int g = input_buf[1][input_row + component1RowOffset][col];
                int b = input_buf[2][input_row + component2RowOffset][col];

                /* Assume that MAXJSAMPLE+1 is a power of 2, so that the MOD
                 * (modulo) operator is equivalent to the bitmask operator AND.
                 */
                r = (r + g - JpegConstants.CENTERJSAMPLE) & JpegConstants.MAXJSAMPLE;
                b = (b + g - JpegConstants.CENTERJSAMPLE) & JpegConstants.MAXJSAMPLE;

                /* Y */
                output_buf[output_row + row][columnOffset++] = (byte)((rgb_y_tab[r + R_Y_OFF] + rgb_y_tab[g + G_Y_OFF] + rgb_y_tab[b + B_Y_OFF]) >> SCALEBITS);
            }
        }
    }

    /*
     * No colorspace change, but conversion from separate-planes
     * to interleaved representation.
     */
    private void RgbConvert(ComponentBuffer[] input_buf, int input_row, byte[][] output_buf, int output_row, int num_rows)
    {
        var component0RowOffset = m_perComponentOffsets[0];
        var component1RowOffset = m_perComponentOffsets[1];
        var component2RowOffset = m_perComponentOffsets[2];

        var num_cols = m_cinfo.outputWidth;

        for (var row = 0; row < num_rows; row++)
        {
            var columnOffset = 0;
            for (var col = 0; col < num_cols; col++)
            {
                int r = input_buf[0][input_row + component0RowOffset][col];
                int g = input_buf[1][input_row + component1RowOffset][col];
                int b = input_buf[2][input_row + component2RowOffset][col];

                /* We can dispense with GETJSAMPLE() here */
                output_buf[output_row + row][columnOffset + JpegConstants.RGB_RED] = (byte)r;
                output_buf[output_row + row][columnOffset + JpegConstants.RGB_GREEN] = (byte)g;
                output_buf[output_row + row][columnOffset + JpegConstants.RGB_BLUE] = (byte)b;
                columnOffset += JpegConstants.RGB_PIXELSIZE;
            }
        }
    }

    /// <summary>
    /// Adobe-style YCCK->CMYK conversion.
    /// We convert YCbCr to R=1-C, G=1-M, and B=1-Y using the same
    /// conversion as above, while passing K (black) unchanged.
    /// We assume build_ycc_rgb_table has been called.
    /// </summary>
    private void YcckCmykConvert(ComponentBuffer[] input_buf, int input_row, byte[][] output_buf, int output_row, int num_rows)
    {
        var component0RowOffset = m_perComponentOffsets[0];
        var component1RowOffset = m_perComponentOffsets[1];
        var component2RowOffset = m_perComponentOffsets[2];
        var component3RowOffset = m_perComponentOffsets[3];

        var limit = m_cinfo.m_sample_range_limit;
        var limitOffset = m_cinfo.m_sampleRangeLimitOffset;

        var num_cols = m_cinfo.outputWidth;
        for (var row = 0; row < num_rows; row++)
        {
            var columnOffset = 0;
            for (var col = 0; col < num_cols; col++)
            {
                int y = input_buf[0][input_row + component0RowOffset][col];
                int cb = input_buf[1][input_row + component1RowOffset][col];
                int cr = input_buf[2][input_row + component2RowOffset][col];

                /* Range-limiting is essential due to noise introduced by DCT losses,
                 * and for extended gamut encodings (sYCC).
                 */
                output_buf[output_row + row][columnOffset] = limit[limitOffset + JpegConstants.MAXJSAMPLE - (y + m_Cr_r_tab[cr])]; /* red */
                output_buf[output_row + row][columnOffset + 1] = limit[limitOffset + JpegConstants.MAXJSAMPLE - (y + JpegUtils.RIGHT_SHIFT(m_Cb_g_tab[cb] + m_Cr_g_tab[cr], SCALEBITS))]; /* green */
                output_buf[output_row + row][columnOffset + 2] = limit[limitOffset + JpegConstants.MAXJSAMPLE - (y + m_Cb_b_tab[cb])]; /* blue */

                /* K passes through unchanged */
                /* don't need GETJSAMPLE here */
                output_buf[output_row + row][columnOffset + 3] = input_buf[3][input_row + component3RowOffset][col];
                columnOffset += 4;
            }

            input_row++;
        }
    }

    /// <summary>
    /// Convert grayscale to RGB: just duplicate the graylevel three times.
    /// This is provided to support applications that don't want to cope
    /// with grayscale as a separate case.
    /// </summary>
    private void GrayRgbConvert(ComponentBuffer[] input_buf, int input_row, byte[][] output_buf, int output_row, int num_rows)
    {
        var component0RowOffset = m_perComponentOffsets[0];
        var component1RowOffset = m_perComponentOffsets[1];
        var component2RowOffset = m_perComponentOffsets[2];

        var num_cols = m_cinfo.outputWidth;
        for (var row = 0; row < num_rows; row++)
        {
            var columnOffset = 0;
            for (var col = 0; col < num_cols; col++)
            {
                /* We can dispense with GETJSAMPLE() here */
                output_buf[output_row + row][columnOffset + JpegConstants.RGB_RED] = input_buf[0][input_row + component0RowOffset][col];
                output_buf[output_row + row][columnOffset + JpegConstants.RGB_GREEN] = input_buf[0][input_row + component1RowOffset][col];
                output_buf[output_row + row][columnOffset + JpegConstants.RGB_BLUE] = input_buf[0][input_row + component2RowOffset][col];
                columnOffset += JpegConstants.RGB_PIXELSIZE;
            }

            input_row++;
        }
    }

    /// <summary>
    /// Color conversion for grayscale: just copy the data.
    /// This also works for YCC -> grayscale conversion, in which
    /// we just copy the Y (luminance) component and ignore chrominance.
    /// </summary>
    private void GrayscaleConvert(ComponentBuffer[] input_buf, int input_row, byte[][] output_buf, int output_row, int num_rows)
    {
        JpegUtils.jcopy_sample_rows(input_buf[0], input_row + m_perComponentOffsets[0], output_buf, output_row, num_rows, m_cinfo.outputWidth);
    }

    /// <summary>
    /// Color conversion for CMYK -> RGB
    /// </summary>
    private void CmykRgbConvert(ComponentBuffer[] input_buf, int input_row, byte[][] output_buf, int output_row, int num_rows)
    {
        var component0RowOffset = m_perComponentOffsets[0];
        var component1RowOffset = m_perComponentOffsets[1];
        var component2RowOffset = m_perComponentOffsets[2];
        var component3RowOffset = m_perComponentOffsets[3];

        for (var row = 0; row < num_rows; row++)
        {
            var columnOffset = 0;
            for (var col = 0; col < m_cinfo.outputWidth; col++)
            {
                int c = input_buf[0][input_row + component0RowOffset][col];
                int m = input_buf[1][input_row + component1RowOffset][col];
                int y = input_buf[2][input_row + component2RowOffset][col];
                int k = input_buf[3][input_row + component3RowOffset][col];

                output_buf[output_row + row][columnOffset + JpegConstants.RGB_RED] = (byte)((c * k) / 255);
                output_buf[output_row + row][columnOffset + JpegConstants.RGB_GREEN] = (byte)((m * k) / 255);
                output_buf[output_row + row][columnOffset + JpegConstants.RGB_BLUE] = (byte)((y * k) / 255);
                columnOffset += JpegConstants.RGB_PIXELSIZE;
            }

            input_row++;
        }
    }

    /// <summary>
    /// Color conversion for YCCK -> RGB
    /// it's just a gybrid of YCCK -> CMYK and CMYK -> RGB conversions
    /// </summary>
    private void YcckRgbConvert(ComponentBuffer[] input_buf, int input_row, byte[][] output_buf, int output_row, int num_rows)
    {
        var component0RowOffset = m_perComponentOffsets[0];
        var component1RowOffset = m_perComponentOffsets[1];
        var component2RowOffset = m_perComponentOffsets[2];
        var component3RowOffset = m_perComponentOffsets[3];

        var limit = m_cinfo.m_sample_range_limit;
        var limitOffset = m_cinfo.m_sampleRangeLimitOffset;

        var num_cols = m_cinfo.outputWidth;
        for (var row = 0; row < num_rows; row++)
        {
            var columnOffset = 0;
            for (var col = 0; col < num_cols; col++)
            {
                int y = input_buf[0][input_row + component0RowOffset][col];
                int cb = input_buf[1][input_row + component1RowOffset][col];
                int cr = input_buf[2][input_row + component2RowOffset][col];

                int cmyk_c = limit[limitOffset + JpegConstants.MAXJSAMPLE - (y + m_Cr_r_tab[cr])];
                int cmyk_m = limit[limitOffset + JpegConstants.MAXJSAMPLE - (y + JpegUtils.RIGHT_SHIFT(m_Cb_g_tab[cb] + m_Cr_g_tab[cr], SCALEBITS))];
                int cmyk_y = limit[limitOffset + JpegConstants.MAXJSAMPLE - (y + m_Cb_b_tab[cb])];
                int cmyk_k = input_buf[3][input_row + component3RowOffset][col];

                output_buf[output_row + row][columnOffset + JpegConstants.RGB_RED] = (byte)((cmyk_c * cmyk_k) / 255);
                output_buf[output_row + row][columnOffset + JpegConstants.RGB_GREEN] = (byte)((cmyk_m * cmyk_k) / 255);
                output_buf[output_row + row][columnOffset + JpegConstants.RGB_BLUE] = (byte)((cmyk_y * cmyk_k) / 255);
                columnOffset += JpegConstants.RGB_PIXELSIZE;
            }

            input_row++;
        }
    }

    /// <summary>
    /// Color conversion for no colorspace change: just copy the data,
    /// converting from separate-planes to interleaved representation.
    /// </summary>
    private void NullConvert(ComponentBuffer[] input_buf, int input_row, byte[][] output_buf, int output_row, int num_rows)
    {
        for (var row = 0; row < num_rows; row++)
        {
            for (var ci = 0; ci < m_cinfo.numComponents; ci++)
            {
                var columnIndex = 0;
                var componentOffset = 0;
                var perComponentOffset = m_perComponentOffsets[ci];

                for (var col = 0; col < m_cinfo.outputWidth; col++)
                {
                    /* needn't bother with GETJSAMPLE() here */
                    output_buf[output_row + row][ci + componentOffset] = input_buf[ci][input_row + perComponentOffset][columnIndex];
                    componentOffset += m_cinfo.numComponents;
                    columnIndex++;
                }
            }

            input_row++;
        }
    }

    private static int FIX(double x)
    {
        return (int)((x * (1L << SCALEBITS)) + 0.5);
    }
}
