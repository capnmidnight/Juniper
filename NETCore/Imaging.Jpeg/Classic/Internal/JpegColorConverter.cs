/*
 * This file contains input colorspace conversion routines.
 */

namespace BitMiracle.LibJpeg.Classic.Internal;

/// <summary>
/// Colorspace conversion
/// </summary>
internal class JpegColorConverter
{
    /**************** RGB -> YCbCr conversion: most common case **************/

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
     *  Y  = Kr * R + (1 - Kr - Kb) * G + Kb * B
     *  Cb = 0.5 * (B - Y) / (1 - Kb)
     *  Cr = 0.5 * (R - Y) / (1 - Kr)
     * With Kr = 0.299 and Kb = 0.114 (derived according to SMPTE RP 177-1993
     * from the 1953 FCC NTSC primaries and CIE Illuminant C),
     * the conversion equations to be implemented are therefore
     *  Y  =  0.299 * R + 0.587 * G + 0.114 * B
     *  Cb = -0.168735892 * R - 0.331264108 * G + 0.5 * B + CENTERJSAMPLE
     *  Cr =  0.5 * R - 0.418687589 * G - 0.081312411 * B + CENTERJSAMPLE
     * Note: older versions of the IJG code used a zero offset of MAXJSAMPLE/2,
     * rather than CENTERJSAMPLE, for Cb and Cr.  This gave equal positive and
     * negative swings for Cb/Cr, but meant that grayscale values (Cb=Cr=0)
     * were not represented exactly.  Now we sacrifice exact representation of
     * maximum red and maximum blue in order to get exact grayscales.
     *
     * To avoid floating-point arithmetic, we represent the fractional constants
     * as integers scaled up by 2^16 (about 4 digits precision); we have to divide
     * the products by 2^16, with appropriate rounding, to get the correct answer.
     *
     * For even more speed, we avoid doing any multiplications in the inner loop
     * by precalculating the constants times R,G,B for all possible values.
     * For 8-bit JSAMPLEs this is very reasonable (only 256 entries per table);
     * for 9-bit to 12-bit samples it is still acceptable.  It's not very
     * reasonable for 16-bit samples, but if you want lossless storage you
     * shouldn't be changing colorspace anyway.
     * The CENTERJSAMPLE offsets and the rounding fudge-factor of 0.5 are included
     * in the tables to save adding them separately in the inner loop.
     */

    private const int SCALEBITS = 16;  /* speediest right-shift on some machines */
    private const int CBCR_OFFSET = JpegConstants.CENTERJSAMPLE << SCALEBITS;
    private const int ONE_HALF = 1 << (SCALEBITS - 1);

    // We allocate one big table and divide it up into eight parts, instead of
    // doing eight alloc_small requests.  This lets us use a single table base
    // address, which can be held in a register in the inner loops on many
    // machines (more than can hold all eight addresses, anyway).
    private const int R_Y_OFF = 0;           /* offset to R => Y section */
    private const int G_Y_OFF = (1 * (JpegConstants.MAXJSAMPLE + 1));  /* offset to G => Y section */
    private const int B_Y_OFF = (2 * (JpegConstants.MAXJSAMPLE + 1));  /* etc. */
    private const int R_CB_OFF = (3 * (JpegConstants.MAXJSAMPLE + 1));
    private const int G_CB_OFF = (4 * (JpegConstants.MAXJSAMPLE + 1));
    private const int B_CB_OFF = (5 * (JpegConstants.MAXJSAMPLE + 1));
    private const int R_CR_OFF = B_CB_OFF;        /* B=>Cb, R=>Cr are the same */
    private const int G_CR_OFF = (6 * (JpegConstants.MAXJSAMPLE + 1));
    private const int B_CR_OFF = (7 * (JpegConstants.MAXJSAMPLE + 1));
    private const int TABLE_SIZE = (8 * (JpegConstants.MAXJSAMPLE + 1));

    private readonly JpegCompressStruct m_cinfo;

    private readonly bool m_useNullStart;

    private int[] m_rgb_ycc_tab;     /* => table for RGB to YCbCr conversion */

    internal delegate void ConvertMethod(byte[][] input_buf, int input_row, byte[][][] output_buf, int output_row, int num_rows);

    /// <summary>
    /// <para>Convert some rows of samples to the JPEG colorspace.</para>
    /// <para>
    /// Note that we change from the application's interleaved-pixel format
    /// to our internal noninterleaved, one-plane-per-component format.
    /// The input buffer is therefore three times as wide as the output buffer.
    /// </para>
    /// <para>
    /// A starting row offset is provided only for the output buffer.  The caller
    /// can easily adjust the passed input_buf value to accommodate any row
    /// offset required on that side.
    /// </para>
    /// </summary>
    internal ConvertMethod color_convert;

    public JpegColorConverter(JpegCompressStruct cinfo)
    {
        m_cinfo = cinfo;

        /* set start_pass to null method until we find out differently */
        m_useNullStart = true;

        /* Make sure input_components agrees with in_color_space */
        switch (cinfo.m_in_color_space)
        {
            case JColorSpace.JCS_GRAYSCALE:
            if (cinfo.m_input_components != 1)
            {
                cinfo.ErrExit(JMessageCode.JERR_BAD_IN_COLORSPACE);
            }

            break;

            case JColorSpace.JCS_RGB:
            case JColorSpace.JCS_BG_RGB:
            if (cinfo.m_input_components != JpegConstants.RGB_PIXELSIZE)
            {
                cinfo.ErrExit(JMessageCode.JERR_BAD_IN_COLORSPACE);
            }

            break;

            case JColorSpace.JCS_YCbCr:
            case JColorSpace.JCS_BG_YCC:
            if (cinfo.m_input_components != 3)
            {
                cinfo.ErrExit(JMessageCode.JERR_BAD_IN_COLORSPACE);
            }

            break;

            case JColorSpace.JCS_CMYK:
            case JColorSpace.JCS_YCCK:
            if (cinfo.m_input_components != 4)
            {
                cinfo.ErrExit(JMessageCode.JERR_BAD_IN_COLORSPACE);
            }

            break;

            default:
            /* JCS_UNKNOWN can be anything */
            if (cinfo.m_input_components < 1)
            {
                cinfo.ErrExit(JMessageCode.JERR_BAD_IN_COLORSPACE);
            }

            break;
        }

        /* Support color transform only for RGB colorspaces */
        if (cinfo.ColorTransform != JColorTransform.JCT_NONE &&
            cinfo.Jpeg_color_space != JColorSpace.JCS_RGB &&
            cinfo.Jpeg_color_space != JColorSpace.JCS_BG_RGB)
        {
            cinfo.ErrExit(JMessageCode.JERR_CONVERSION_NOTIMPL);
        }

        /* Check num_components, set conversion method based on requested space */
        switch (cinfo.m_jpeg_color_space)
        {
            case JColorSpace.JCS_GRAYSCALE:
            if (cinfo.m_num_components != 1)
            {
                cinfo.ErrExit(JMessageCode.JERR_BAD_J_COLORSPACE);
            }

            switch (cinfo.m_in_color_space)
            {
                case JColorSpace.JCS_GRAYSCALE:
                case JColorSpace.JCS_YCbCr:
                case JColorSpace.JCS_BG_YCC:
                color_convert = GrayscaleConvert;
                break;

                case JColorSpace.JCS_RGB:
                m_useNullStart = false; // use rgb_ycc_start
                color_convert = RgbGrayConvert;
                break;

                default:
                cinfo.ErrExit(JMessageCode.JERR_CONVERSION_NOTIMPL);
                break;
            }
            break;

            case JColorSpace.JCS_RGB:
            case JColorSpace.JCS_BG_RGB:
            if (cinfo.m_num_components != 3)
            {
                cinfo.ErrExit(JMessageCode.JERR_BAD_J_COLORSPACE);
            }

            if (cinfo.m_in_color_space == cinfo.Jpeg_color_space)
            {
                switch (cinfo.ColorTransform)
                {
                    case JColorTransform.JCT_NONE:
                    color_convert = RgbConvert;
                    break;
                    case JColorTransform.JCT_SUBTRACT_GREEN:
                    color_convert = RgbRgb1Convert;
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

            case JColorSpace.JCS_YCbCr:
            if (cinfo.m_num_components != 3)
            {
                cinfo.ErrExit(JMessageCode.JERR_BAD_J_COLORSPACE);
            }

            switch (cinfo.m_in_color_space)
            {
                case JColorSpace.JCS_RGB:
                m_useNullStart = false; // use rgb_ycc_start
                color_convert = RgbYccConvert;
                break;

                case JColorSpace.JCS_YCbCr:
                color_convert = NullConvert;
                break;

                default:
                cinfo.ErrExit(JMessageCode.JERR_CONVERSION_NOTIMPL);
                break;
            }
            break;

            case JColorSpace.JCS_BG_YCC:
            if (cinfo.m_num_components != 3)
            {
                cinfo.ErrExit(JMessageCode.JERR_BAD_J_COLORSPACE);
            }

            switch (cinfo.m_in_color_space)
            {
                case JColorSpace.JCS_RGB:
                /* For conversion from normal RGB input to BG_YCC representation,
                 * the Cb/Cr values are first computed as usual, and then
                 * quantized further after DCT processing by a factor of
                 * 2 in reference to the nominal quantization factor.
                 */
                /* need quantization scale by factor of 2 after DCT */
                cinfo.Component_info[1].component_needed = true;
                cinfo.Component_info[2].component_needed = true;
                /* compute normal YCC first */
                m_useNullStart = false; // use rgb_ycc_start
                color_convert = RgbYccConvert;
                break;
                case JColorSpace.JCS_YCbCr:
                /* need quantization scale by factor of 2 after DCT */
                cinfo.Component_info[1].component_needed = true;
                cinfo.Component_info[2].component_needed = true;
                color_convert = NullConvert;
                break;
                case JColorSpace.JCS_BG_YCC:
                /* Pass through for BG_YCC input */
                color_convert = NullConvert;
                break;
                default:
                cinfo.ErrExit(JMessageCode.JERR_BAD_J_COLORSPACE);
                break;
            }
            break;

            case JColorSpace.JCS_CMYK:
            if (cinfo.m_num_components != 4)
            {
                cinfo.ErrExit(JMessageCode.JERR_BAD_J_COLORSPACE);
            }

            if (cinfo.m_in_color_space == JColorSpace.JCS_CMYK)
            {
                color_convert = NullConvert;
            }
            else
            {
                cinfo.ErrExit(JMessageCode.JERR_CONVERSION_NOTIMPL);
            }

            break;

            case JColorSpace.JCS_YCCK:
            if (cinfo.m_num_components != 4)
            {
                cinfo.ErrExit(JMessageCode.JERR_BAD_J_COLORSPACE);
            }

            switch (cinfo.m_in_color_space)
            {
                case JColorSpace.JCS_CMYK:
                m_useNullStart = false; // use rgb_ycc_start
                color_convert = CmykYcckConvert;
                break;

                case JColorSpace.JCS_YCCK:
                color_convert = NullConvert;
                break;

                default:
                cinfo.ErrExit(JMessageCode.JERR_CONVERSION_NOTIMPL);
                break;
            }
            break;

            default:
            /* allow null conversion of JCS_UNKNOWN */
            if (cinfo.m_jpeg_color_space != cinfo.m_in_color_space || cinfo.m_num_components != cinfo.m_input_components)
            {
                cinfo.ErrExit(JMessageCode.JERR_CONVERSION_NOTIMPL);
            }

            color_convert = NullConvert;
            break;
        }
    }

    public void StartPass()
    {
        if (!m_useNullStart)
        {
            RgbYccStart();
        }
    }

    /// <summary>
    /// Initialize for RGB->YCC colorspace conversion.
    /// </summary>
    private void RgbYccStart()
    {
        /* Allocate and fill in the conversion tables. */
        m_rgb_ycc_tab = new int[TABLE_SIZE];

        for (var i = 0; i <= JpegConstants.MAXJSAMPLE; i++)
        {
            m_rgb_ycc_tab[i + R_Y_OFF] = Fix(0.299) * i;
            m_rgb_ycc_tab[i + G_Y_OFF] = Fix(0.587) * i;
            m_rgb_ycc_tab[i + B_Y_OFF] = (Fix(0.114) * i) + ONE_HALF;
            m_rgb_ycc_tab[i + R_CB_OFF] = (-Fix(0.168735892)) * i;
            m_rgb_ycc_tab[i + G_CB_OFF] = (-Fix(0.331264108)) * i;

            /* We use a rounding fudge-factor of 0.5-epsilon for Cb and Cr.
             * This ensures that the maximum output will round to MAXJSAMPLE
             * not MAXJSAMPLE+1, and thus that we don't have to range-limit.
             */
            m_rgb_ycc_tab[i + B_CB_OFF] = (Fix(0.5) * i) + CBCR_OFFSET + ONE_HALF - 1;

            /*  B=>Cb and R=>Cr tables are the same
                m_rgb_ycc_tab[i + R_CR_OFF] = FIX(0.5) * i + CBCR_OFFSET + ONE_HALF - 1;
            */
            m_rgb_ycc_tab[i + G_CR_OFF] = (-Fix(0.418687589)) * i;
            m_rgb_ycc_tab[i + B_CR_OFF] = (-Fix(0.081312411)) * i;
        }
    }

    private static int Fix(double x)
    {
        return (int)((x * (1L << SCALEBITS)) + 0.5);
    }

    /// <summary>
    /// RGB -&gt; YCbCr conversion: most common case
    /// YCbCr is defined per CCIR 601-1, except that Cb and Cr are
    /// normalized to the range 0..MAXJSAMPLE rather than -0.5 .. 0.5.
    /// The conversion equations to be implemented are therefore
    /// Y  =  0.29900 * R + 0.58700 * G + 0.11400 * B
    /// Cb = -0.16874 * R - 0.33126 * G + 0.50000 * B  + CENTERJSAMPLE
    /// Cr =  0.50000 * R - 0.41869 * G - 0.08131 * B  + CENTERJSAMPLE
    /// (These numbers are derived from TIFF 6.0 section 21, dated 3-June-92.)
    /// To avoid floating-point arithmetic, we represent the fractional constants
    /// as integers scaled up by 2^16 (about 4 digits precision); we have to divide
    /// the products by 2^16, with appropriate rounding, to get the correct answer.
    /// For even more speed, we avoid doing any multiplications in the inner loop
    /// by precalculating the constants times R,G,B for all possible values.
    /// For 8-bit JSAMPLEs this is very reasonable (only 256 entries per table);
    /// for 12-bit samples it is still acceptable.  It's not very reasonable for
    /// 16-bit samples, but if you want lossless storage you shouldn't be changing
    /// colorspace anyway.
    /// The CENTERJSAMPLE offsets and the rounding fudge-factor of 0.5 are included
    /// in the tables to save adding them separately in the inner loop.
    /// </summary>
    private void RgbYccConvert(byte[][] input_buf, int input_row, byte[][][] output_buf, int output_row, int num_rows)
    {
        var num_cols = m_cinfo.m_image_width;
        for (var row = 0; row < num_rows; row++)
        {
            var columnOffset = 0;
            for (var col = 0; col < num_cols; col++)
            {
                int r = input_buf[input_row + row][columnOffset + JpegConstants.RGB_RED];
                int g = input_buf[input_row + row][columnOffset + JpegConstants.RGB_GREEN];
                int b = input_buf[input_row + row][columnOffset + JpegConstants.RGB_BLUE];
                columnOffset += JpegConstants.RGB_PIXELSIZE;

                /* If the inputs are 0..MAXJSAMPLE, the outputs of these equations
                 * must be too; we do not need an explicit range-limiting operation.
                 * Hence the value being shifted is never negative, and we don't
                 * need the general RIGHT_SHIFT macro.
                 */
                /* Y */
                output_buf[0][output_row][col] = (byte)((m_rgb_ycc_tab[r + R_Y_OFF] + m_rgb_ycc_tab[g + G_Y_OFF] + m_rgb_ycc_tab[b + B_Y_OFF]) >> SCALEBITS);
                /* Cb */
                output_buf[1][output_row][col] = (byte)((m_rgb_ycc_tab[r + R_CB_OFF] + m_rgb_ycc_tab[g + G_CB_OFF] + m_rgb_ycc_tab[b + B_CB_OFF]) >> SCALEBITS);
                /* Cr */
                output_buf[2][output_row][col] = (byte)((m_rgb_ycc_tab[r + R_CR_OFF] + m_rgb_ycc_tab[g + G_CR_OFF] + m_rgb_ycc_tab[b + B_CR_OFF]) >> SCALEBITS);
            }

            output_row++;
        }
    }

    /// <summary>
    /// Convert some rows of samples to the JPEG colorspace.
    /// This version handles RGB->grayscale conversion, which is the same
    /// as the RGB->Y portion of RGB->YCbCr.
    /// We assume rgb_ycc_start has been called (we only use the Y tables).
    /// </summary>
    private void RgbGrayConvert(byte[][] input_buf, int input_row, byte[][][] output_buf, int output_row, int num_rows)
    {
        var num_cols = m_cinfo.m_image_width;
        for (var row = 0; row < num_rows; row++)
        {
            var columnOffset = 0;
            for (var col = 0; col < num_cols; col++)
            {
                int r = input_buf[input_row + row][columnOffset + JpegConstants.RGB_RED];
                int g = input_buf[input_row + row][columnOffset + JpegConstants.RGB_GREEN];
                int b = input_buf[input_row + row][columnOffset + JpegConstants.RGB_BLUE];
                columnOffset += JpegConstants.RGB_PIXELSIZE;

                /* Y */
                output_buf[0][output_row][col] = (byte)((m_rgb_ycc_tab[r + R_Y_OFF] + m_rgb_ycc_tab[g + G_Y_OFF] + m_rgb_ycc_tab[b + B_Y_OFF]) >> SCALEBITS);
            }

            output_row++;
        }
    }

    /// <summary>
    /// Convert some rows of samples to the JPEG colorspace.
    /// This version handles Adobe-style CMYK->YCCK conversion,
    /// where we convert R=1-C, G=1-M, and B=1-Y to YCbCr using the same
    /// conversion as above, while passing K (black) unchanged.
    /// We assume rgb_ycc_start has been called.
    /// </summary>
    private void CmykYcckConvert(byte[][] input_buf, int input_row, byte[][][] output_buf, int output_row, int num_rows)
    {
        var num_cols = m_cinfo.m_image_width;
        for (var row = 0; row < num_rows; row++)
        {
            var columnOffset = 0;
            for (var col = 0; col < num_cols; col++)
            {
                var r = JpegConstants.MAXJSAMPLE - input_buf[input_row + row][columnOffset];
                var g = JpegConstants.MAXJSAMPLE - input_buf[input_row + row][columnOffset + 1];
                var b = JpegConstants.MAXJSAMPLE - input_buf[input_row + row][columnOffset + 2];

                /* K passes through as-is */
                /* don't need GETJSAMPLE here */
                output_buf[3][output_row][col] = input_buf[input_row + row][columnOffset + 3];
                columnOffset += 4;

                /* If the inputs are 0..MAXJSAMPLE, the outputs of these equations
                 * must be too; we do not need an explicit range-limiting operation.
                 * Hence the value being shifted is never negative, and we don't
                 * need the general RIGHT_SHIFT macro.
                 */
                /* Y */
                output_buf[0][output_row][col] = (byte)((m_rgb_ycc_tab[r + R_Y_OFF] + m_rgb_ycc_tab[g + G_Y_OFF] + m_rgb_ycc_tab[b + B_Y_OFF]) >> SCALEBITS);
                /* Cb */
                output_buf[1][output_row][col] = (byte)((m_rgb_ycc_tab[r + R_CB_OFF] + m_rgb_ycc_tab[g + G_CB_OFF] + m_rgb_ycc_tab[b + B_CB_OFF]) >> SCALEBITS);
                /* Cr */
                output_buf[2][output_row][col] = (byte)((m_rgb_ycc_tab[r + R_CR_OFF] + m_rgb_ycc_tab[g + G_CR_OFF] + m_rgb_ycc_tab[b + B_CR_OFF]) >> SCALEBITS);
            }

            output_row++;
        }
    }

    /*
     * Convert some rows of samples to the JPEG colorspace.
     * [R,G,B] to [R-G,G,B-G] conversion with modulo calculation
     * (forward reversible color transform).
     * This can be seen as an adaption of the general RGB->YCbCr
     * conversion equation with Kr = Kb = 0, while replacing the
     * normalization by modulo calculation.
     */
    private void RgbRgb1Convert(byte[][] input_buf, int input_row, byte[][][] output_buf, int output_row, int num_rows)
    {
        var num_cols = m_cinfo.m_image_width;

        for (var row = 0; row < num_rows; row++)
        {
            var columnOffset = 0;
            for (var col = 0; col < num_cols; col++)
            {
                int r = input_buf[input_row + row][columnOffset];
                int g = input_buf[input_row + row][columnOffset + 1];
                int b = input_buf[input_row + row][columnOffset + 2];
                /* Assume that MAXJSAMPLE+1 is a power of 2, so that the MOD
                 * (modulo) operator is equivalent to the bitmask operator AND.
                 */
                output_buf[0][output_row][col] = (byte)((r - g + JpegConstants.CENTERJSAMPLE) & JpegConstants.MAXJSAMPLE);
                output_buf[1][output_row][col] = (byte)g;
                output_buf[2][output_row][col] = (byte)((b - g + JpegConstants.CENTERJSAMPLE) & JpegConstants.MAXJSAMPLE);
                columnOffset += JpegConstants.RGB_PIXELSIZE;
            }

            output_row++;
        }
    }

    /// <summary>
    /// Convert some rows of samples to the JPEG colorspace.
    /// This version handles grayscale output with no conversion.
    /// The source can be either plain grayscale or YCC (since Y == gray).
    /// </summary>
    private void GrayscaleConvert(byte[][] input_buf, int input_row, byte[][][] output_buf, int output_row, int num_rows)
    {
        var num_cols = m_cinfo.m_image_width;
        var instride = m_cinfo.m_input_components;

        for (var row = 0; row < num_rows; row++)
        {
            var columnOffset = 0;
            for (var col = 0; col < num_cols; col++)
            {
                /* don't need GETJSAMPLE() here */
                output_buf[0][output_row][col] = input_buf[input_row + row][columnOffset];
                columnOffset += instride;
            }

            output_row++;
        }
    }

    /*
     * Convert some rows of samples to the JPEG colorspace.
     * No colorspace conversion, but change from interleaved
     * to separate-planes representation.
     */
    private void RgbConvert(byte[][] input_buf, int input_row, byte[][][] output_buf, int output_row, int num_rows)
    {
        var num_cols = m_cinfo.m_image_width;

        for (var row = 0; row < num_rows; row++)
        {
            var columnOffset = 0;
            for (var col = 0; col < num_cols; col++)
            {
                /* We can dispense with GETJSAMPLE() here */
                output_buf[0][output_row][col] = input_buf[input_row + row][columnOffset];
                output_buf[1][output_row][col] = input_buf[input_row + row][columnOffset + 1];
                output_buf[2][output_row][col] = input_buf[input_row + row][columnOffset + 2];

                columnOffset += JpegConstants.RGB_PIXELSIZE;
            }

            output_row++;
        }
    }

    /// <summary>
    /// Convert some rows of samples to the JPEG colorspace.
    /// This version handles multi-component colorspaces without conversion.
    /// We assume input_components == num_components.
    /// </summary>
    private void NullConvert(byte[][] input_buf, int input_row, byte[][][] output_buf, int output_row, int num_rows)
    {
        var nc = m_cinfo.m_num_components;
        var num_cols = m_cinfo.m_image_width;

        for (var row = 0; row < num_rows; row++)
        {
            /* It seems fastest to make a separate pass for each component. */
            for (var ci = 0; ci < nc; ci++)
            {
                var columnOffset = 0;
                for (var col = 0; col < num_cols; col++)
                {
                    /* don't need GETJSAMPLE() here */
                    output_buf[ci][output_row][col] = input_buf[input_row + row][columnOffset + ci];
                    columnOffset += nc;
                }
            }

            output_row++;
        }
    }
}
