/*
 * This file contains the forward-DCT management logic.
 * This code selects a particular DCT implementation to be used,
 * and it performs related housekeeping chores including coefficient
 * quantization.
 */

namespace BitMiracle.LibJpeg.Classic.Internal;

/// <summary>
/// <para>Forward DCT (also controls coefficient quantization)</para>
/// <para>
/// A forward DCT routine is given a pointer to an input sample array and
/// a pointer to a work area of type DCTELEM[]; the DCT is to be performed
/// in-place in that buffer.  Type DCTELEM is int for 8-bit samples, INT32
/// for 12-bit samples.  (NOTE: Floating-point DCT implementations use an
/// array of type FAST_FLOAT, instead.)
/// The input data is to be fetched from the sample array starting at a
/// specified column.  (Any row offset needed will be applied to the array
/// pointer before it is passed to the FDCT code.)
/// Note that the number of samples fetched by the FDCT routine is
/// DCT_h_scaled_size * DCT_v_scaled_size.
/// The DCT outputs are returned scaled up by a factor of 8; they therefore
/// have a range of +-8K for 8-bit data, +-128K for 12-bit data.  This
/// convention improves accuracy in integer implementations and saves some
/// work in floating-point ones.
/// </para>
/// <para>Each IDCT routine has its own ideas about the best dct_table element type.</para>
/// </summary>
internal class JpegForwardDct
{
    private const int FAST_INTEGER_CONST_BITS = 8;

    /* We use the following pre-calculated constants.
    * If you change FAST_INTEGER_CONST_BITS you may want to add appropriate values.
    * 
    * Convert a positive real constant to an integer scaled by CONST_SCALE.
    * static int FAST_INTEGER_FIX(double x)
    *{
    *    return ((int) ((x) * (((int) 1) << FAST_INTEGER_CONST_BITS) + 0.5));
    *}
    */
    private const int FAST_INTEGER_FIX_0_382683433 = 98;        /* FIX(0.382683433) */
    private const int FAST_INTEGER_FIX_0_541196100 = 139;       /* FIX(0.541196100) */
    private const int FAST_INTEGER_FIX_0_707106781 = 181;       /* FIX(0.707106781) */
    private const int FAST_INTEGER_FIX_1_306562965 = 334;       /* FIX(1.306562965) */

    private const int SLOW_INTEGER_CONST_BITS = 13;
    private const int SLOW_INTEGER_PASS1_BITS = 2;

    /* We use the following pre-calculated constants.
    * If you change SLOW_INTEGER_CONST_BITS you may want to add appropriate values.
    * 
    * Convert a positive real constant to an integer scaled by CONST_SCALE.
    *
    * static int SLOW_INTEGER_FIX(double x)
    * {
    *     return ((int) ((x) * (((int) 1) << SLOW_INTEGER_CONST_BITS) + 0.5));
    * }
    */
    private const int SLOW_INTEGER_FIX_0_298631336 = 2446;   /* FIX(0.298631336) */
    private const int SLOW_INTEGER_FIX_0_390180644 = 3196;   /* FIX(0.390180644) */
    private const int SLOW_INTEGER_FIX_0_541196100 = 4433;   /* FIX(0.541196100) */
    private const int SLOW_INTEGER_FIX_0_765366865 = 6270;   /* FIX(0.765366865) */
    private const int SLOW_INTEGER_FIX_0_899976223 = 7373;   /* FIX(0.899976223) */
    private const int SLOW_INTEGER_FIX_1_175875602 = 9633;   /* FIX(1.175875602) */
    private const int SLOW_INTEGER_FIX_1_501321110 = 12299;  /* FIX(1.501321110) */
    private const int SLOW_INTEGER_FIX_1_847759065 = 15137;  /* FIX(1.847759065) */
    private const int SLOW_INTEGER_FIX_1_961570560 = 16069;  /* FIX(1.961570560) */
    private const int SLOW_INTEGER_FIX_2_053119869 = 16819;  /* FIX(2.053119869) */
    private const int SLOW_INTEGER_FIX_2_562915447 = 20995;  /* FIX(2.562915447) */
    private const int SLOW_INTEGER_FIX_3_072711026 = 25172;  /* FIX(3.072711026) */

    /* For AA&N IDCT method, divisors are equal to quantization
     * coefficients scaled by scalefactor[row]*scalefactor[col], where
     *   scalefactor[0] = 1
     *   scalefactor[k] = cos(k*PI/16) * sqrt(2)    for k=1..7
     * We apply a further scale factor of 8.
     */
    private const int CONST_BITS = 14;

    /* precomputed values scaled up by 14 bits */
    private static readonly short[] aanscales = [
        16384, 22725, 21407, 19266, 16384, 12873, 8867, 4520, 22725, 31521, 29692, 26722, 22725, 17855,
        12299, 6270, 21407, 29692, 27969, 25172, 21407, 16819, 11585,
        5906, 19266, 26722, 25172, 22654, 19266, 15137, 10426, 5315,
        16384, 22725, 21407, 19266, 16384, 12873, 8867, 4520, 12873,
        17855, 16819, 15137, 12873, 10114, 6967, 3552, 8867, 12299,
        11585, 10426, 8867, 6967, 4799, 2446, 4520, 6270, 5906, 5315,
        4520, 3552, 2446, 1247 ];

    /* For float AA&N IDCT method, divisors are equal to quantization
     * coefficients scaled by scalefactor[row]*scalefactor[col], where
     *   scalefactor[0] = 1
     *   scalefactor[k] = cos(k*PI/16) * sqrt(2)    for k=1..7
     * We apply a further scale factor of 8.
     * What's actually stored is 1/divisor so that the inner loop can
     * use a multiplication rather than a division.
     */
    private static readonly double[] aanscalefactor = [
        1.0, 1.387039845, 1.306562965, 1.175875602, 1.0,
        0.785694958, 0.541196100, 0.275899379 ];

    private readonly JpegCompressStruct m_cinfo;

    private delegate void ForwardDctMethodDelegate(int[] data, byte[][] sample_data, int start_row, int start_col);
    private readonly ForwardDctMethodDelegate[] do_dct = new ForwardDctMethodDelegate[JpegConstants.MAX_COMPONENTS];

    /* Same as above for the floating-point case. */
    private delegate void FloatDctMethodDelegate(float[] data, byte[][] sample_data, int start_row, int start_col);
    private readonly FloatDctMethodDelegate[] do_float_dct = new FloatDctMethodDelegate[JpegConstants.MAX_COMPONENTS];

    /// <summary>
    /// <para>Perform forward DCT on one or more blocks of a component.</para>
    /// <para>
    /// The input samples are taken from the sample_data[] array starting at
    /// position start_row/start_col, and moving to the right for any additional
    /// blocks. The quantized coefficients are returned in coef_blocks[].
    /// </para>
    /// </summary>
    public delegate void ForwardDctDelegate(JpegComponentInfo compptr, byte[][] sample_data, JBlock[] coef_blocks, int start_row, int start_col, int num_blocks);

    /* It is useful to allow each component to have a separate FDCT method. */
    public ForwardDctDelegate[] forward_DCT = new ForwardDctDelegate[JpegConstants.MAX_COMPONENTS];

    /* The allocated post-DCT divisor tables - big enough for any supported variant and not
       identical to the quant table entries, because of scaling (especially for an
       unnormalized DCT) - are pointed to by dct_table in the per-component comp_info
       structures.  Each table is given in normal array order.
    */
    private class DivisorTable
    {
        public int[] int_array = new int[JpegConstants.DCTSIZE2];
        public float[] float_array = new float[JpegConstants.DCTSIZE2];
    };

    private readonly DivisorTable[] m_dctTables;

    public JpegForwardDct(JpegCompressStruct cinfo)
    {
        m_cinfo = cinfo;
        m_dctTables = new DivisorTable[m_cinfo.m_num_components];

        for (var ci = 0; ci < m_cinfo.m_num_components; ci++)
        {
            /* Allocate a divisor table for each component */
            m_dctTables[ci] = new DivisorTable();
        }
    }

    /// <summary>
    /// Initialize for a processing pass.
    /// Verify that all referenced Q-tables are present, and set up
    /// the divisor table for each one.
    /// In the current implementation, DCT of all components is done during
    /// the first pass, even if only some components will be output in the
    /// first scan.  Hence all components should be examined here.
    /// </summary>
    public virtual void StartPass()
    {
        JDctMethod method = 0;
        for (var ci = 0; ci < m_cinfo.m_num_components; ci++)
        {
            /* Select the proper DCT routine for this component's scaling */
            var compptr = m_cinfo.Component_info[ci];
            switch ((compptr.DCT_h_scaled_size << 8) + compptr.DCT_v_scaled_size)
            {
                case ((1 << 8) + 1):
                do_dct[ci] = JpegForwardDct1x1;
                method = JDctMethod.JDCT_ISLOW;    /* jfdctint uses islow-style table */
                break;
                case ((2 << 8) + 2):
                do_dct[ci] = JpegForwardDct2x2;
                method = JDctMethod.JDCT_ISLOW;    /* jfdctint uses islow-style table */
                break;
                case ((3 << 8) + 3):
                do_dct[ci] = JpegForwardDct3x3;
                method = JDctMethod.JDCT_ISLOW;    /* jfdctint uses islow-style table */
                break;
                case ((4 << 8) + 4):
                do_dct[ci] = JpegForwardDct4x4;
                method = JDctMethod.JDCT_ISLOW;    /* jfdctint uses islow-style table */
                break;
                case ((5 << 8) + 5):
                do_dct[ci] = JpegForwardDct5x5;
                method = JDctMethod.JDCT_ISLOW;    /* jfdctint uses islow-style table */
                break;
                case ((6 << 8) + 6):
                do_dct[ci] = JpegForwardDct6x6;
                method = JDctMethod.JDCT_ISLOW;    /* jfdctint uses islow-style table */
                break;
                case ((7 << 8) + 7):
                do_dct[ci] = JpegForwardDct7x7;
                method = JDctMethod.JDCT_ISLOW;    /* jfdctint uses islow-style table */
                break;
                case ((9 << 8) + 9):
                do_dct[ci] = JpegForwardDct9x9;
                method = JDctMethod.JDCT_ISLOW;    /* jfdctint uses islow-style table */
                break;
                case ((10 << 8) + 10):
                do_dct[ci] = JpegForwardDct10x10;
                method = JDctMethod.JDCT_ISLOW;    /* jfdctint uses islow-style table */
                break;
                case ((11 << 8) + 11):
                do_dct[ci] = JpegForwardDct11x11;
                method = JDctMethod.JDCT_ISLOW;    /* jfdctint uses islow-style table */
                break;
                case ((12 << 8) + 12):
                do_dct[ci] = JpegForwardDct12x12;
                method = JDctMethod.JDCT_ISLOW;    /* jfdctint uses islow-style table */
                break;
                case ((13 << 8) + 13):
                do_dct[ci] = JpegForwardDct13x13;
                method = JDctMethod.JDCT_ISLOW;    /* jfdctint uses islow-style table */
                break;
                case ((14 << 8) + 14):
                do_dct[ci] = JpegForwardDct14x14;
                method = JDctMethod.JDCT_ISLOW;    /* jfdctint uses islow-style table */
                break;
                case ((15 << 8) + 15):
                do_dct[ci] = JpegForwardDct15x15;
                method = JDctMethod.JDCT_ISLOW;    /* jfdctint uses islow-style table */
                break;
                case ((16 << 8) + 16):
                do_dct[ci] = JpegForwardDct16x16;
                method = JDctMethod.JDCT_ISLOW;    /* jfdctint uses islow-style table */
                break;
                case ((16 << 8) + 8):
                do_dct[ci] = JpegForwardDct16x8;
                method = JDctMethod.JDCT_ISLOW;    /* jfdctint uses islow-style table */
                break;
                case ((14 << 8) + 7):
                do_dct[ci] = JpegForwardDct14x7;
                method = JDctMethod.JDCT_ISLOW;    /* jfdctint uses islow-style table */
                break;
                case ((12 << 8) + 6):
                do_dct[ci] = JpegForwardDct12x6;
                method = JDctMethod.JDCT_ISLOW;    /* jfdctint uses islow-style table */
                break;
                case ((10 << 8) + 5):
                do_dct[ci] = JpegForwardDct10x5;
                method = JDctMethod.JDCT_ISLOW;    /* jfdctint uses islow-style table */
                break;
                case ((8 << 8) + 4):
                do_dct[ci] = JpegForwardDct8x4;
                method = JDctMethod.JDCT_ISLOW;    /* jfdctint uses islow-style table */
                break;
                case ((6 << 8) + 3):
                do_dct[ci] = JpegForwardDct6x3;
                method = JDctMethod.JDCT_ISLOW;    /* jfdctint uses islow-style table */
                break;
                case ((4 << 8) + 2):
                do_dct[ci] = JpegForwardDct4x2;
                method = JDctMethod.JDCT_ISLOW;    /* jfdctint uses islow-style table */
                break;
                case ((2 << 8) + 1):
                do_dct[ci] = JpegForwardDct2x1;
                method = JDctMethod.JDCT_ISLOW;    /* jfdctint uses islow-style table */
                break;
                case ((8 << 8) + 16):
                do_dct[ci] = JpegForwardDct8x16;
                method = JDctMethod.JDCT_ISLOW;    /* jfdctint uses islow-style table */
                break;
                case ((7 << 8) + 14):
                do_dct[ci] = JpegForwardDct7x14;
                method = JDctMethod.JDCT_ISLOW;    /* jfdctint uses islow-style table */
                break;
                case ((6 << 8) + 12):
                do_dct[ci] = JpegForwardDct6x12;
                method = JDctMethod.JDCT_ISLOW;    /* jfdctint uses islow-style table */
                break;
                case ((5 << 8) + 10):
                do_dct[ci] = JpegForwardDct5x10;
                method = JDctMethod.JDCT_ISLOW;    /* jfdctint uses islow-style table */
                break;
                case ((4 << 8) + 8):
                do_dct[ci] = JpegForwardDct4x8;
                method = JDctMethod.JDCT_ISLOW;    /* jfdctint uses islow-style table */
                break;
                case ((3 << 8) + 6):
                do_dct[ci] = JpegForwardDct3x6;
                method = JDctMethod.JDCT_ISLOW;    /* jfdctint uses islow-style table */
                break;
                case ((2 << 8) + 4):
                do_dct[ci] = JpegForwardDct2x4;
                method = JDctMethod.JDCT_ISLOW;    /* jfdctint uses islow-style table */
                break;
                case ((1 << 8) + 2):
                do_dct[ci] = JpegForwardDct1x2;
                method = JDctMethod.JDCT_ISLOW;    /* jfdctint uses islow-style table */
                break;
                case ((JpegConstants.DCTSIZE << 8) + JpegConstants.DCTSIZE):
                switch (m_cinfo.m_dct_method)
                {
                    case JDctMethod.JDCT_ISLOW:
                    do_dct[ci] = JpegForwardDctISlow;
                    method = JDctMethod.JDCT_ISLOW;
                    break;
                    case JDctMethod.JDCT_IFAST:
                    do_dct[ci] = JpegForwardDctIFast;
                    method = JDctMethod.JDCT_IFAST;
                    break;
                    case JDctMethod.JDCT_FLOAT:
                    do_float_dct[ci] = JpegForwardDctFloat;
                    method = JDctMethod.JDCT_FLOAT;
                    break;
                    default:
                    m_cinfo.ErrExit(JMessageCode.JERR_NOT_COMPILED);
                    break;
                }
                break;
                default:
                m_cinfo.ErrExit(JMessageCode.JERR_BAD_DCTSIZE, compptr.DCT_h_scaled_size, compptr.DCT_v_scaled_size);
                break;
            }

            var qtblno = m_cinfo.Component_info[ci].Quant_tbl_no;

            /* Make sure specified quantization table is present */
            if (qtblno < 0 || qtblno >= JpegConstants.NUM_QUANT_TBLS || m_cinfo.m_quant_tbl_ptrs[qtblno] is null)
            {
                m_cinfo.ErrExit(JMessageCode.JERR_NO_QUANT_TABLE, qtblno);
            }

            var qtbl = m_cinfo.m_quant_tbl_ptrs[qtblno];
            int[] dtbl;

            /* Create divisor table from quant table */
            var i = 0;
            switch (method)
            {
                case JDctMethod.JDCT_ISLOW:
                /* For LL&M IDCT method, divisors are equal to raw quantization
                 * coefficients multiplied by 8 (to counteract scaling).
                 */
                dtbl = m_dctTables[ci].int_array;
                for (i = 0; i < JpegConstants.DCTSIZE2; i++)
                {
                    dtbl[i] = qtbl.quantBal[i] << (compptr.component_needed ? 4 : 3);
                }

                forward_DCT[ci] = ForwardDctImpl;
                break;

                case JDctMethod.JDCT_IFAST:
                dtbl = m_dctTables[ci].int_array;
                for (i = 0; i < JpegConstants.DCTSIZE2; i++)
                {
                    dtbl[i] = JpegUtils.DESCALE(qtbl.quantBal[i] * aanscales[i], compptr.component_needed ? CONST_BITS - 4 : CONST_BITS - 3);
                }

                forward_DCT[ci] = ForwardDctImpl;
                break;

                case JDctMethod.JDCT_FLOAT:
                var fdtbl = m_dctTables[ci].float_array;
                i = 0;
                for (var row = 0; row < JpegConstants.DCTSIZE; row++)
                {
                    for (var col = 0; col < JpegConstants.DCTSIZE; col++)
                    {
                        fdtbl[i] = (float)(1.0 / (qtbl.quantBal[i] * aanscalefactor[row] * aanscalefactor[col] * (compptr.component_needed ? 16.0 : 8.0)));
                        i++;
                    }
                }
                forward_DCT[ci] = ForwardDctFloatImpl;
                break;

                default:
                m_cinfo.ErrExit(JMessageCode.JERR_NOT_COMPILED);
                break;
            }
        }
    }

    // This version is used for integer DCT implementations.
    private void ForwardDctImpl(JpegComponentInfo compptr, byte[][] sample_data, JBlock[] coef_blocks, int start_row, int start_col, int num_blocks)
    {
        /* This routine is heavily used, so it's worth coding it tightly. */
        var do_dct = this.do_dct[compptr.Component_index];
        var divisors = m_dctTables[compptr.Component_index].int_array;
        var workspace = new int[JpegConstants.DCTSIZE2];    /* work area for FDCT subroutine */
        for (var bi = 0; bi < num_blocks; bi++, start_col += compptr.DCT_h_scaled_size)
        {
            /* Perform the DCT */
            do_dct(workspace, sample_data, start_row, start_col);

            /* Quantize/descale the coefficients, and store into coef_blocks[] */
            for (var i = 0; i < JpegConstants.DCTSIZE2; i++)
            {
                var qval = divisors[i];
                var temp = workspace[i];

                if (temp < 0)
                {
                    temp = -temp;
                    temp += qval >> 1;  /* for rounding */

                    if (temp >= qval)
                    {
                        temp /= qval;
                    }
                    else
                    {
                        temp = 0;
                    }

                    temp = -temp;
                }
                else
                {
                    temp += qval >> 1;  /* for rounding */

                    if (temp >= qval)
                    {
                        temp /= qval;
                    }
                    else
                    {
                        temp = 0;
                    }
                }

                coef_blocks[bi][i] = (short)temp;
            }
        }
    }

    // This version is used for floating-point DCT implementations.
    private void ForwardDctFloatImpl(JpegComponentInfo compptr, byte[][] sample_data, JBlock[] coef_blocks, int start_row, int start_col, int num_blocks)
    {
        /* This routine is heavily used, so it's worth coding it tightly. */
        var do_dct = do_float_dct[compptr.Component_index];
        var divisors = m_dctTables[compptr.Component_index].float_array;
        var workspace = new float[JpegConstants.DCTSIZE2]; /* work area for FDCT subroutine */
        for (var bi = 0; bi < num_blocks; bi++, start_col += compptr.DCT_h_scaled_size)
        {
            /* Perform the DCT */
            do_dct(workspace, sample_data, start_row, start_col);

            /* Quantize/descale the coefficients, and store into coef_blocks[] */
            for (var i = 0; i < JpegConstants.DCTSIZE2; i++)
            {
                /* Apply the quantization and scaling factor */
                var temp = workspace[i] * divisors[i];

                /* Round to nearest integer.
                 * Since C does not specify the direction of rounding for negative
                 * quotients, we have to force the dividend positive for portability.
                 * The maximum coefficient size is +-16K (for 12-bit data), so this
                 * code should work for either 16-bit or 32-bit ints.
                 */
                coef_blocks[bi][i] = (short)((int)(temp + (float)16384.5) - 16384);
            }
        }
    }

    /// <summary>
    /// <para>
    /// Perform the forward DCT on one block of samples.
    /// NOTE: this code only copes with 8x8 DCTs.
    /// </para>
    /// <para>
    /// A floating-point implementation of the 
    /// forward DCT (Discrete Cosine Transform).
    /// </para>
    /// <para>
    /// This implementation should be more accurate than either of the integer
    /// DCT implementations.  However, it may not give the same results on all
    /// machines because of differences in roundoff behavior.  Speed will depend
    /// on the hardware's floating point capacity.
    /// </para>
    /// <para>
    /// A 2-D DCT can be done by 1-D DCT on each row followed by 1-D DCT
    /// on each column.  Direct algorithms are also available, but they are
    /// much more complex and seem not to be any faster when reduced to code.
    /// </para>
    /// <para>
    /// This implementation is based on Arai, Agui, and Nakajima's algorithm for
    /// scaled DCT.  Their original paper (Trans. IEICE E-71(11):1095) is in
    /// Japanese, but the algorithm is described in the Pennebaker &amp; Mitchell
    /// JPEG textbook (see REFERENCES section in file README).  The following code
    /// is based directly on figure 4-8 in P&amp;M.
    /// While an 8-point DCT cannot be done in less than 11 multiplies, it is
    /// possible to arrange the computation so that many of the multiplies are
    /// simple scalings of the final outputs.  These multiplies can then be
    /// folded into the multiplications or divisions by the JPEG quantization
    /// table entries.  The AA&amp;N method leaves only 5 multiplies and 29 adds
    /// to be done in the DCT itself.
    /// The primary disadvantage of this method is that with a fixed-point
    /// implementation, accuracy is lost due to imprecise representation of the
    /// scaled quantization values.  However, that problem does not arise if
    /// we use floating point arithmetic.
    /// </para>
    /// </summary>
    private static void JpegForwardDctFloat(float[] data, byte[][] sample_data, int start_row, int start_col)
    {
        /* Pass 1: process rows. */
        var dataIndex = 0;
        for (var ctr = 0; ctr < JpegConstants.DCTSIZE; ctr++)
        {
            var elem = sample_data[start_row + ctr];
            var elemIndex = start_col;

            /* Load data into workspace */
            float tmp0 = elem[elemIndex + 0] + elem[elemIndex + 7];
            float tmp7 = elem[elemIndex + 0] - elem[elemIndex + 7];
            float tmp1 = elem[elemIndex + 1] + elem[elemIndex + 6];
            float tmp6 = elem[elemIndex + 1] - elem[elemIndex + 6];
            float tmp2 = elem[elemIndex + 2] + elem[elemIndex + 5];
            float tmp5 = elem[elemIndex + 2] - elem[elemIndex + 5];
            float tmp3 = elem[elemIndex + 3] + elem[elemIndex + 4];
            float tmp4 = elem[elemIndex + 3] - elem[elemIndex + 4];

            /* Even part */

            var tmp10 = tmp0 + tmp3;    /* phase 2 */
            var tmp13 = tmp0 - tmp3;
            var tmp11 = tmp1 + tmp2;
            var tmp12 = tmp1 - tmp2;

            /* Apply unsigned->signed conversion. */
            data[dataIndex + 0] = tmp10 + tmp11 - (8 * JpegConstants.CENTERJSAMPLE); /* phase 3 */
            data[dataIndex + 4] = tmp10 - tmp11;

            var z1 = (tmp12 + tmp13) * 0.707106781f; /* c4 */
            data[dataIndex + 2] = tmp13 + z1;    /* phase 5 */
            data[dataIndex + 6] = tmp13 - z1;

            /* Odd part */

            tmp10 = tmp4 + tmp5;    /* phase 2 */
            tmp11 = tmp5 + tmp6;
            tmp12 = tmp6 + tmp7;

            /* The rotator is modified from fig 4-8 to avoid extra negations. */
            var z5 = (tmp10 - tmp12) * 0.382683433f; /* c6 */
            var z2 = (0.541196100f * tmp10) + z5; /* c2-c6 */
            var z4 = (1.306562965f * tmp12) + z5; /* c2+c6 */
            var z3 = tmp11 * 0.707106781f; /* c4 */

            var z11 = tmp7 + z3;        /* phase 5 */
            var z13 = tmp7 - z3;

            data[dataIndex + 5] = z13 + z2;  /* phase 6 */
            data[dataIndex + 3] = z13 - z2;
            data[dataIndex + 1] = z11 + z4;
            data[dataIndex + 7] = z11 - z4;

            dataIndex += JpegConstants.DCTSIZE;     /* advance pointer to next row */
        }

        /* Pass 2: process columns. */

        dataIndex = 0;
        for (var ctr = JpegConstants.DCTSIZE - 1; ctr >= 0; ctr--)
        {
            var tmp0 = data[dataIndex + (JpegConstants.DCTSIZE * 0)] + data[dataIndex + (JpegConstants.DCTSIZE * 7)];
            var tmp7 = data[dataIndex + (JpegConstants.DCTSIZE * 0)] - data[dataIndex + (JpegConstants.DCTSIZE * 7)];
            var tmp1 = data[dataIndex + (JpegConstants.DCTSIZE * 1)] + data[dataIndex + (JpegConstants.DCTSIZE * 6)];
            var tmp6 = data[dataIndex + (JpegConstants.DCTSIZE * 1)] - data[dataIndex + (JpegConstants.DCTSIZE * 6)];
            var tmp2 = data[dataIndex + (JpegConstants.DCTSIZE * 2)] + data[dataIndex + (JpegConstants.DCTSIZE * 5)];
            var tmp5 = data[dataIndex + (JpegConstants.DCTSIZE * 2)] - data[dataIndex + (JpegConstants.DCTSIZE * 5)];
            var tmp3 = data[dataIndex + (JpegConstants.DCTSIZE * 3)] + data[dataIndex + (JpegConstants.DCTSIZE * 4)];
            var tmp4 = data[dataIndex + (JpegConstants.DCTSIZE * 3)] - data[dataIndex + (JpegConstants.DCTSIZE * 4)];

            /* Even part */

            var tmp10 = tmp0 + tmp3;    /* phase 2 */
            var tmp13 = tmp0 - tmp3;
            var tmp11 = tmp1 + tmp2;
            var tmp12 = tmp1 - tmp2;

            data[dataIndex + (JpegConstants.DCTSIZE * 0)] = tmp10 + tmp11; /* phase 3 */
            data[dataIndex + (JpegConstants.DCTSIZE * 4)] = tmp10 - tmp11;

            var z1 = (tmp12 + tmp13) * 0.707106781f; /* c4 */
            data[dataIndex + (JpegConstants.DCTSIZE * 2)] = tmp13 + z1; /* phase 5 */
            data[dataIndex + (JpegConstants.DCTSIZE * 6)] = tmp13 - z1;

            /* Odd part */

            tmp10 = tmp4 + tmp5;    /* phase 2 */
            tmp11 = tmp5 + tmp6;
            tmp12 = tmp6 + tmp7;

            /* The rotator is modified from fig 4-8 to avoid extra negations. */
            var z5 = (tmp10 - tmp12) * 0.382683433f; /* c6 */
            var z2 = (0.541196100f * tmp10) + z5; /* c2-c6 */
            var z4 = (1.306562965f * tmp12) + z5; /* c2+c6 */
            var z3 = tmp11 * 0.707106781f; /* c4 */

            var z11 = tmp7 + z3;        /* phase 5 */
            var z13 = tmp7 - z3;

            data[dataIndex + (JpegConstants.DCTSIZE * 5)] = z13 + z2; /* phase 6 */
            data[dataIndex + (JpegConstants.DCTSIZE * 3)] = z13 - z2;
            data[dataIndex + (JpegConstants.DCTSIZE * 1)] = z11 + z4;
            data[dataIndex + (JpegConstants.DCTSIZE * 7)] = z11 - z4;

            dataIndex++;          /* advance pointer to next column */
        }
    }

    /// <summary>
    /// <para>
    /// Perform the forward DCT on one block of samples.
    /// NOTE: this code only copes with 8x8 DCTs.
    /// This file contains a fast, not so accurate integer implementation of the
    /// forward DCT (Discrete Cosine Transform).
    /// </para>
    /// <para>
    /// A 2-D DCT can be done by 1-D DCT on each row followed by 1-D DCT
    /// on each column.  Direct algorithms are also available, but they are
    /// much more complex and seem not to be any faster when reduced to code.
    /// </para>
    /// <para>
    /// This implementation is based on Arai, Agui, and Nakajima's algorithm for
    /// scaled DCT.  Their original paper (Trans. IEICE E-71(11):1095) is in
    /// Japanese, but the algorithm is described in the Pennebaker &amp; Mitchell
    /// JPEG textbook (see REFERENCES section in file README).  The following code
    /// is based directly on figure 4-8 in P&amp;M.
    /// While an 8-point DCT cannot be done in less than 11 multiplies, it is
    /// possible to arrange the computation so that many of the multiplies are
    /// simple scalings of the final outputs.  These multiplies can then be
    /// folded into the multiplications or divisions by the JPEG quantization
    /// table entries.  The AA&amp;N method leaves only 5 multiplies and 29 adds
    /// to be done in the DCT itself.
    /// The primary disadvantage of this method is that with fixed-point math,
    /// accuracy is lost due to imprecise representation of the scaled
    /// quantization values.  The smaller the quantization table entry, the less
    /// precise the scaled value, so this implementation does worse with high-
    /// quality-setting files than with low-quality ones.
    /// </para>
    /// <para>
    /// Scaling decisions are generally the same as in the LL&amp;M algorithm;
    /// see JpegForwardDctISlow for more details.  However, we choose to descale
    /// (right shift) multiplication products as soon as they are formed,
    /// rather than carrying additional fractional bits into subsequent additions.
    /// This compromises accuracy slightly, but it lets us save a few shifts.
    /// More importantly, 16-bit arithmetic is then adequate (for 8-bit samples)
    /// everywhere except in the multiplications proper; this saves a good deal
    /// of work on 16-bit-int machines.
    /// </para>
    /// <para>
    /// Again to save a few shifts, the intermediate results between pass 1 and
    /// pass 2 are not upscaled, but are represented only to integral precision.
    /// </para>
    /// <para>
    /// A final compromise is to represent the multiplicative constants to only
    /// 8 fractional bits, rather than 13.  This saves some shifting work on some
    /// machines, and may also reduce the cost of multiplication (since there
    /// are fewer one-bits in the constants).
    /// </para>
    /// </summary>
    private static void JpegForwardDctIFast(int[] data, byte[][] sample_data, int start_row, int start_col)
    {
        /* Pass 1: process rows. */
        var dataIndex = 0;
        for (var ctr = 0; ctr < JpegConstants.DCTSIZE; ctr++)
        {
            var elem = sample_data[start_row + ctr];
            var elemIndex = start_col;

            /* Load data into workspace */
            var tmp0 = elem[elemIndex + 0] + elem[elemIndex + 7];
            var tmp7 = elem[elemIndex + 0] - elem[elemIndex + 7];
            var tmp1 = elem[elemIndex + 1] + elem[elemIndex + 6];
            var tmp6 = elem[elemIndex + 1] - elem[elemIndex + 6];
            var tmp2 = elem[elemIndex + 2] + elem[elemIndex + 5];
            var tmp5 = elem[elemIndex + 2] - elem[elemIndex + 5];
            var tmp3 = elem[elemIndex + 3] + elem[elemIndex + 4];
            var tmp4 = elem[elemIndex + 3] - elem[elemIndex + 4];

            /* Even part */

            var tmp10 = tmp0 + tmp3;    /* phase 2 */
            var tmp13 = tmp0 - tmp3;
            var tmp11 = tmp1 + tmp2;
            var tmp12 = tmp1 - tmp2;

            /* Apply unsigned->signed conversion. */
            data[dataIndex + 0] = tmp10 + tmp11 - (8 * JpegConstants.CENTERJSAMPLE); /* phase 3 */
            data[dataIndex + 4] = tmp10 - tmp11;

            var z1 = FastIntegerMultiply(tmp12 + tmp13, FAST_INTEGER_FIX_0_707106781); /* c4 */
            data[dataIndex + 2] = tmp13 + z1;    /* phase 5 */
            data[dataIndex + 6] = tmp13 - z1;

            /* Odd part */

            tmp10 = tmp4 + tmp5;    /* phase 2 */
            tmp11 = tmp5 + tmp6;
            tmp12 = tmp6 + tmp7;

            /* The rotator is modified from fig 4-8 to avoid extra negations. */
            var z5 = FastIntegerMultiply(tmp10 - tmp12, FAST_INTEGER_FIX_0_382683433); /* c6 */
            var z2 = FastIntegerMultiply(tmp10, FAST_INTEGER_FIX_0_541196100) + z5; /* c2-c6 */
            var z4 = FastIntegerMultiply(tmp12, FAST_INTEGER_FIX_1_306562965) + z5; /* c2+c6 */
            var z3 = FastIntegerMultiply(tmp11, FAST_INTEGER_FIX_0_707106781); /* c4 */

            var z11 = tmp7 + z3;        /* phase 5 */
            var z13 = tmp7 - z3;

            data[dataIndex + 5] = z13 + z2;  /* phase 6 */
            data[dataIndex + 3] = z13 - z2;
            data[dataIndex + 1] = z11 + z4;
            data[dataIndex + 7] = z11 - z4;

            dataIndex += JpegConstants.DCTSIZE;     /* advance pointer to next row */
        }

        /* Pass 2: process columns. */

        dataIndex = 0;
        for (var ctr = JpegConstants.DCTSIZE - 1; ctr >= 0; ctr--)
        {
            var tmp0 = data[dataIndex + (JpegConstants.DCTSIZE * 0)] + data[dataIndex + (JpegConstants.DCTSIZE * 7)];
            var tmp7 = data[dataIndex + (JpegConstants.DCTSIZE * 0)] - data[dataIndex + (JpegConstants.DCTSIZE * 7)];
            var tmp1 = data[dataIndex + (JpegConstants.DCTSIZE * 1)] + data[dataIndex + (JpegConstants.DCTSIZE * 6)];
            var tmp6 = data[dataIndex + (JpegConstants.DCTSIZE * 1)] - data[dataIndex + (JpegConstants.DCTSIZE * 6)];
            var tmp2 = data[dataIndex + (JpegConstants.DCTSIZE * 2)] + data[dataIndex + (JpegConstants.DCTSIZE * 5)];
            var tmp5 = data[dataIndex + (JpegConstants.DCTSIZE * 2)] - data[dataIndex + (JpegConstants.DCTSIZE * 5)];
            var tmp3 = data[dataIndex + (JpegConstants.DCTSIZE * 3)] + data[dataIndex + (JpegConstants.DCTSIZE * 4)];
            var tmp4 = data[dataIndex + (JpegConstants.DCTSIZE * 3)] - data[dataIndex + (JpegConstants.DCTSIZE * 4)];

            /* Even part */

            var tmp10 = tmp0 + tmp3;    /* phase 2 */
            var tmp13 = tmp0 - tmp3;
            var tmp11 = tmp1 + tmp2;
            var tmp12 = tmp1 - tmp2;

            data[dataIndex + (JpegConstants.DCTSIZE * 0)] = tmp10 + tmp11; /* phase 3 */
            data[dataIndex + (JpegConstants.DCTSIZE * 4)] = tmp10 - tmp11;

            var z1 = FastIntegerMultiply(tmp12 + tmp13, FAST_INTEGER_FIX_0_707106781); /* c4 */
            data[dataIndex + (JpegConstants.DCTSIZE * 2)] = tmp13 + z1; /* phase 5 */
            data[dataIndex + (JpegConstants.DCTSIZE * 6)] = tmp13 - z1;

            /* Odd part */

            tmp10 = tmp4 + tmp5;    /* phase 2 */
            tmp11 = tmp5 + tmp6;
            tmp12 = tmp6 + tmp7;

            /* The rotator is modified from fig 4-8 to avoid extra negations. */
            var z5 = FastIntegerMultiply(tmp10 - tmp12, FAST_INTEGER_FIX_0_382683433); /* c6 */
            var z2 = FastIntegerMultiply(tmp10, FAST_INTEGER_FIX_0_541196100) + z5; /* c2-c6 */
            var z4 = FastIntegerMultiply(tmp12, FAST_INTEGER_FIX_1_306562965) + z5; /* c2+c6 */
            var z3 = FastIntegerMultiply(tmp11, FAST_INTEGER_FIX_0_707106781); /* c4 */

            var z11 = tmp7 + z3;        /* phase 5 */
            var z13 = tmp7 - z3;

            data[dataIndex + (JpegConstants.DCTSIZE * 5)] = z13 + z2; /* phase 6 */
            data[dataIndex + (JpegConstants.DCTSIZE * 3)] = z13 - z2;
            data[dataIndex + (JpegConstants.DCTSIZE * 1)] = z11 + z4;
            data[dataIndex + (JpegConstants.DCTSIZE * 7)] = z11 - z4;

            dataIndex++;          /* advance pointer to next column */
        }
    }

    /// <summary>
    /// <para>
    /// Perform the forward DCT on one block of samples.
    /// NOTE: this code only copes with 8x8 DCTs.
    /// </para>
    /// <para>
    /// A slow-but-accurate integer implementation of the
    /// forward DCT (Discrete Cosine Transform).
    /// </para>
    /// <para>
    /// A 2-D DCT can be done by 1-D DCT on each row followed by 1-D DCT
    /// on each column.  Direct algorithms are also available, but they are
    /// much more complex and seem not to be any faster when reduced to code.
    /// </para>
    /// <para>
    /// This implementation is based on an algorithm described in
    /// C. Loeffler, A. Ligtenberg and G. Moschytz, "Practical Fast 1-D DCT
    /// Algorithms with 11 Multiplications", Proc. Int'l. Conf. on Acoustics,
    /// Speech, and Signal Processing 1989 (ICASSP '89), pp. 988-991.
    /// The primary algorithm described there uses 11 multiplies and 29 adds.
    /// We use their alternate method with 12 multiplies and 32 adds.
    /// The advantage of this method is that no data path contains more than one
    /// multiplication; this allows a very simple and accurate implementation in
    /// scaled fixed-point arithmetic, with a minimal number of shifts.
    /// </para>
    /// <para>The poop on this scaling stuff is as follows:</para>
    /// <para>
    /// Each 1-D DCT step produces outputs which are a factor of sqrt(N)
    /// larger than the true DCT outputs.  The final outputs are therefore
    /// a factor of N larger than desired; since N=8 this can be cured by
    /// a simple right shift at the end of the algorithm.  The advantage of
    /// this arrangement is that we save two multiplications per 1-D DCT,
    /// because the y0 and y4 outputs need not be divided by sqrt(N).
    /// In the IJG code, this factor of 8 is removed by the quantization 
    /// step, NOT here.
    /// </para>
    /// <para>
    /// We have to do addition and subtraction of the integer inputs, which
    /// is no problem, and multiplication by fractional constants, which is
    /// a problem to do in integer arithmetic.  We multiply all the constants
    /// by CONST_SCALE and convert them to integer constants (thus retaining
    /// SLOW_INTEGER_CONST_BITS bits of precision in the constants).  After doing a
    /// multiplication we have to divide the product by CONST_SCALE, with proper
    /// rounding, to produce the correct output.  This division can be done
    /// cheaply as a right shift of SLOW_INTEGER_CONST_BITS bits.  We postpone shifting
    /// as long as possible so that partial sums can be added together with
    /// full fractional precision.
    /// </para>
    /// <para>
    /// The outputs of the first pass are scaled up by SLOW_INTEGER_PASS1_BITS bits so that
    /// they are represented to better-than-integral precision.  These outputs
    /// require BITS_IN_JSAMPLE + SLOW_INTEGER_PASS1_BITS + 3 bits; this fits in a 16-bit word
    /// with the recommended scaling.  (For 12-bit sample data, the intermediate
    /// array is int anyway.)
    /// </para>
    /// <para>
    /// To avoid overflow of the 32-bit intermediate results in pass 2, we must
    /// have BITS_IN_JSAMPLE + SLOW_INTEGER_CONST_BITS + SLOW_INTEGER_PASS1_BITS &lt;= 26.  Error analysis
    /// shows that the values given below are the most effective.
    /// </para>
    /// </summary>
    private static void JpegForwardDctISlow(int[] data, byte[][] sample_data, int start_row, int start_col)
    {
        /* Pass 1: process rows. */
        /* Note results are scaled up by sqrt(8) compared to a true DCT; */
        /* furthermore, we scale the results by 2**SLOW_INTEGER_PASS1_BITS. */
        var dataIndex = 0;
        for (var ctr = 0; ctr < JpegConstants.DCTSIZE; ctr++)
        {
            var elem = sample_data[start_row + ctr];
            var elemIndex = start_col;

            var tmp0 = elem[elemIndex + 0] + elem[elemIndex + 7];
            var tmp1 = elem[elemIndex + 1] + elem[elemIndex + 6];
            var tmp2 = elem[elemIndex + 2] + elem[elemIndex + 5];
            var tmp3 = elem[elemIndex + 3] + elem[elemIndex + 4];

            /* Even part per LL&M figure 1 --- note that published figure is faulty;
            * rotator "sqrt(2)*c1" should be "sqrt(2)*c6".
            */

            var tmp10 = tmp0 + tmp3;
            var tmp12 = tmp0 - tmp3;
            var tmp11 = tmp1 + tmp2;
            var tmp13 = tmp1 - tmp2;

            tmp0 = elem[elemIndex + 0] - elem[elemIndex + 7];
            tmp1 = elem[elemIndex + 1] - elem[elemIndex + 6];
            tmp2 = elem[elemIndex + 2] - elem[elemIndex + 5];
            tmp3 = elem[elemIndex + 3] - elem[elemIndex + 4];

            data[dataIndex + 0] = (tmp10 + tmp11 - (8 * JpegConstants.CENTERJSAMPLE)) << SLOW_INTEGER_PASS1_BITS;
            data[dataIndex + 4] = (tmp10 - tmp11) << SLOW_INTEGER_PASS1_BITS;

            var z1 = (tmp12 + tmp13) * SLOW_INTEGER_FIX_0_541196100;
            /* Add fudge factor here for final descale. */
            z1 += 1 << (SLOW_INTEGER_CONST_BITS - SLOW_INTEGER_PASS1_BITS - 1);

            data[dataIndex + 2] = JpegUtils.RIGHT_SHIFT(z1 + (tmp12 * SLOW_INTEGER_FIX_0_765366865), /* c2-c6 */
                                            SLOW_INTEGER_CONST_BITS - SLOW_INTEGER_PASS1_BITS);
            data[dataIndex + 6] = JpegUtils.DESCALE(z1 - (tmp13 * SLOW_INTEGER_FIX_1_847759065),
                                            SLOW_INTEGER_CONST_BITS - SLOW_INTEGER_PASS1_BITS);

            /* Odd part per figure 8 --- note paper omits factor of sqrt(2).
            * cK represents cos(K*pi/16).
            * i0..i3 in the paper are tmp4..tmp7 here.
            */

            tmp12 = tmp0 + tmp2;
            tmp13 = tmp1 + tmp3;

            z1 = (tmp12 + tmp13) * SLOW_INTEGER_FIX_1_175875602; /*  c3 */
            /* Add fudge factor here for final descale. */
            z1 += 1 << (SLOW_INTEGER_CONST_BITS - SLOW_INTEGER_PASS1_BITS - 1);

            tmp12 *= -SLOW_INTEGER_FIX_0_390180644;          /* -c3+c5 */
            tmp13 *= -SLOW_INTEGER_FIX_1_961570560;          /* -c3-c5 */
            tmp12 += z1;
            tmp13 += z1;

            z1 = (tmp0 + tmp3) * -SLOW_INTEGER_FIX_0_899976223;       /* -c3+c7 */
            tmp0 *= SLOW_INTEGER_FIX_1_501321110;              /*  c1+c3-c5-c7 */
            tmp3 *= SLOW_INTEGER_FIX_0_298631336;              /* -c1+c3+c5-c7 */
            tmp0 += z1 + tmp12;
            tmp3 += z1 + tmp13;

            z1 = (tmp1 + tmp2) * -SLOW_INTEGER_FIX_2_562915447;       /* -c1-c3 */
            tmp1 *= SLOW_INTEGER_FIX_3_072711026;              /*  c1+c3+c5-c7 */
            tmp2 *= SLOW_INTEGER_FIX_2_053119869;              /*  c1+c3-c5+c7 */
            tmp1 += z1 + tmp13;
            tmp2 += z1 + tmp12;

            data[dataIndex + 1] = JpegUtils.RIGHT_SHIFT(tmp0, SLOW_INTEGER_CONST_BITS - SLOW_INTEGER_PASS1_BITS);
            data[dataIndex + 3] = JpegUtils.RIGHT_SHIFT(tmp1, SLOW_INTEGER_CONST_BITS - SLOW_INTEGER_PASS1_BITS);
            data[dataIndex + 5] = JpegUtils.RIGHT_SHIFT(tmp2, SLOW_INTEGER_CONST_BITS - SLOW_INTEGER_PASS1_BITS);
            data[dataIndex + 7] = JpegUtils.RIGHT_SHIFT(tmp3, SLOW_INTEGER_CONST_BITS - SLOW_INTEGER_PASS1_BITS);

            dataIndex += JpegConstants.DCTSIZE;     /* advance pointer to next row */
        }

        /* Pass 2: process columns.
        * We remove the SLOW_INTEGER_PASS1_BITS scaling, but leave the results scaled up
        * by an overall factor of 8.
        * cK represents sqrt(2) * cos(K*pi/16).
        */

        dataIndex = 0;
        for (var ctr = JpegConstants.DCTSIZE - 1; ctr >= 0; ctr--)
        {
            /* Even part per LL&M figure 1 --- note that published figure is faulty;
             * rotator "sqrt(2)*c1" should be "sqrt(2)*c6".
             */
            var tmp0 = data[dataIndex + (JpegConstants.DCTSIZE * 0)] + data[dataIndex + (JpegConstants.DCTSIZE * 7)];
            var tmp1 = data[dataIndex + (JpegConstants.DCTSIZE * 1)] + data[dataIndex + (JpegConstants.DCTSIZE * 6)];
            var tmp2 = data[dataIndex + (JpegConstants.DCTSIZE * 2)] + data[dataIndex + (JpegConstants.DCTSIZE * 5)];
            var tmp3 = data[dataIndex + (JpegConstants.DCTSIZE * 3)] + data[dataIndex + (JpegConstants.DCTSIZE * 4)];

            /* Add fudge factor here for final descale. */
            var tmp10 = tmp0 + tmp3 + (1 << (SLOW_INTEGER_PASS1_BITS - 1));
            var tmp12 = tmp0 - tmp3;
            var tmp11 = tmp1 + tmp2;
            var tmp13 = tmp1 - tmp2;

            tmp0 = data[dataIndex + (JpegConstants.DCTSIZE * 0)] - data[dataIndex + (JpegConstants.DCTSIZE * 7)];
            tmp1 = data[dataIndex + (JpegConstants.DCTSIZE * 1)] - data[dataIndex + (JpegConstants.DCTSIZE * 6)];
            tmp2 = data[dataIndex + (JpegConstants.DCTSIZE * 2)] - data[dataIndex + (JpegConstants.DCTSIZE * 5)];
            tmp3 = data[dataIndex + (JpegConstants.DCTSIZE * 3)] - data[dataIndex + (JpegConstants.DCTSIZE * 4)];

            data[dataIndex + (JpegConstants.DCTSIZE * 0)] = JpegUtils.RIGHT_SHIFT(tmp10 + tmp11, SLOW_INTEGER_PASS1_BITS);
            data[dataIndex + (JpegConstants.DCTSIZE * 4)] = JpegUtils.RIGHT_SHIFT(tmp10 - tmp11, SLOW_INTEGER_PASS1_BITS);

            var z1 = (tmp12 + tmp13) * SLOW_INTEGER_FIX_0_541196100;       /* c6 */
            /* Add fudge factor here for final descale. */
            z1 += 1 << (SLOW_INTEGER_CONST_BITS + SLOW_INTEGER_PASS1_BITS - 1);

            data[dataIndex + (JpegConstants.DCTSIZE * 2)] = JpegUtils.RIGHT_SHIFT(
                z1 + (tmp12 * SLOW_INTEGER_FIX_0_765366865),
                SLOW_INTEGER_CONST_BITS + SLOW_INTEGER_PASS1_BITS);
            data[dataIndex + (JpegConstants.DCTSIZE * 6)] = JpegUtils.RIGHT_SHIFT(
                z1 - (tmp13 * SLOW_INTEGER_FIX_1_847759065),
                SLOW_INTEGER_CONST_BITS + SLOW_INTEGER_PASS1_BITS);

            /* Odd part per figure 8 --- note paper omits factor of sqrt(2).
            * i0..i3 in the paper are tmp4..tmp7 here.
            */

            tmp12 = tmp0 + tmp2;
            tmp13 = tmp1 + tmp3;

            z1 = (tmp12 + tmp13) * SLOW_INTEGER_FIX_1_175875602; /*  c3 */
            /* Add fudge factor here for final descale. */
            z1 += 1 << (SLOW_INTEGER_CONST_BITS + SLOW_INTEGER_PASS1_BITS - 1);

            tmp12 *= -SLOW_INTEGER_FIX_0_390180644;          /* -c3+c5 */
            tmp13 *= -SLOW_INTEGER_FIX_1_961570560;          /* -c3-c5 */
            tmp12 += z1;
            tmp13 += z1;

            z1 = (tmp0 + tmp3) * -SLOW_INTEGER_FIX_0_899976223;       /* -c3+c7 */
            tmp0 *= SLOW_INTEGER_FIX_1_501321110;              /*  c1+c3-c5-c7 */
            tmp3 *= SLOW_INTEGER_FIX_0_298631336;              /* -c1+c3+c5-c7 */
            tmp0 += z1 + tmp12;
            tmp3 += z1 + tmp13;

            z1 = (tmp1 + tmp2) * -SLOW_INTEGER_FIX_2_562915447;       /* -c1-c3 */
            tmp1 *= SLOW_INTEGER_FIX_3_072711026;              /*  c1+c3+c5-c7 */
            tmp2 *= SLOW_INTEGER_FIX_2_053119869;              /*  c1+c3-c5+c7 */
            tmp1 += z1 + tmp13;
            tmp2 += z1 + tmp12;

            data[dataIndex + (JpegConstants.DCTSIZE * 1)] = JpegUtils.RIGHT_SHIFT(tmp0, SLOW_INTEGER_CONST_BITS + SLOW_INTEGER_PASS1_BITS);
            data[dataIndex + (JpegConstants.DCTSIZE * 3)] = JpegUtils.RIGHT_SHIFT(tmp1, SLOW_INTEGER_CONST_BITS + SLOW_INTEGER_PASS1_BITS);
            data[dataIndex + (JpegConstants.DCTSIZE * 5)] = JpegUtils.RIGHT_SHIFT(tmp2, SLOW_INTEGER_CONST_BITS + SLOW_INTEGER_PASS1_BITS);
            data[dataIndex + (JpegConstants.DCTSIZE * 7)] = JpegUtils.RIGHT_SHIFT(tmp3, SLOW_INTEGER_CONST_BITS + SLOW_INTEGER_PASS1_BITS);

            dataIndex++;          /* advance pointer to next column */
        }
    }

    /// <summary>
    /// Multiply a DCTELEM variable by an int constant, and immediately
    /// descale to yield a DCTELEM result.
    /// </summary>
    private static int FastIntegerMultiply(int var, int c)
    {
#if !USE_ACCURATE_ROUNDING
        return JpegUtils.RIGHT_SHIFT(var * c, FAST_INTEGER_CONST_BITS);
#else
        return JpegUtils.DESCALE(var * c, FAST_INTEGER_CONST_BITS);
#endif
    }

    private static int SlowIntegerFix(double x)
    {
        return (int)((x * (1 << SLOW_INTEGER_CONST_BITS)) + 0.5);
    }

    private void JpegForwardDct1x1(int[] data, byte[][] sample_data, int start_row, int start_col)
    {
        m_cinfo.ErrExit(JMessageCode.JERR_NOTIMPL);
    }

    private void JpegForwardDct2x2(int[] data, byte[][] sample_data, int start_row, int start_col)
    {
        m_cinfo.ErrExit(JMessageCode.JERR_NOTIMPL);
    }

    private void JpegForwardDct3x3(int[] data, byte[][] sample_data, int start_row, int start_col)
    {
        m_cinfo.ErrExit(JMessageCode.JERR_NOTIMPL);
    }

    private void JpegForwardDct4x4(int[] data, byte[][] sample_data, int start_row, int start_col)
    {
        m_cinfo.ErrExit(JMessageCode.JERR_NOTIMPL);
    }

    private void JpegForwardDct5x5(int[] data, byte[][] sample_data, int start_row, int start_col)
    {
        m_cinfo.ErrExit(JMessageCode.JERR_NOTIMPL);
    }

    private void JpegForwardDct6x6(int[] data, byte[][] sample_data, int start_row, int start_col)
    {
        m_cinfo.ErrExit(JMessageCode.JERR_NOTIMPL);
    }

    private void JpegForwardDct7x7(int[] data, byte[][] sample_data, int start_row, int start_col)
    {
        m_cinfo.ErrExit(JMessageCode.JERR_NOTIMPL);
    }

    private void JpegForwardDct9x9(int[] data, byte[][] sample_data, int start_row, int start_col)
    {
        m_cinfo.ErrExit(JMessageCode.JERR_NOTIMPL);
    }

    private void JpegForwardDct10x10(int[] data, byte[][] sample_data, int start_row, int start_col)
    {
        m_cinfo.ErrExit(JMessageCode.JERR_NOTIMPL);
    }

    private void JpegForwardDct11x11(int[] data, byte[][] sample_data, int start_row, int start_col)
    {
        m_cinfo.ErrExit(JMessageCode.JERR_NOTIMPL);
    }

    private void JpegForwardDct12x12(int[] data, byte[][] sample_data, int start_row, int start_col)
    {
        m_cinfo.ErrExit(JMessageCode.JERR_NOTIMPL);
    }

    private void JpegForwardDct13x13(int[] data, byte[][] sample_data, int start_row, int start_col)
    {
        m_cinfo.ErrExit(JMessageCode.JERR_NOTIMPL);
    }

    private void JpegForwardDct14x14(int[] data, byte[][] sample_data, int start_row, int start_col)
    {
        m_cinfo.ErrExit(JMessageCode.JERR_NOTIMPL);
    }

    private void JpegForwardDct15x15(int[] data, byte[][] sample_data, int start_row, int start_col)
    {
        m_cinfo.ErrExit(JMessageCode.JERR_NOTIMPL);
    }

    /*
     * Perform the forward DCT on a 16x16 sample block.
     */
    private void JpegForwardDct16x16(int[] data1, byte[][] sample_data, int start_row, int start_col)
    {
        /* Pass 1: process rows.
         * Note results are scaled up by sqrt(8) compared to a true DCT;
         * furthermore, we scale the results by 2**PASS1_BITS.
         * cK represents sqrt(2) * cos(K*pi/32).
         */
        var workspace = new int[JpegConstants.DCTSIZE2];

        var dataIndex = 0;
        var data = data1;

        var ctr = 0;
        for (; ; )
        {
            var elem = sample_data[start_row + ctr];
            var elemIndex = start_col;

            /* Even part */

            var tmp0 = elem[elemIndex + 0] + elem[elemIndex + 15];
            var tmp1 = elem[elemIndex + 1] + elem[elemIndex + 14];
            var tmp2 = elem[elemIndex + 2] + elem[elemIndex + 13];
            var tmp3 = elem[elemIndex + 3] + elem[elemIndex + 12];
            var tmp4 = elem[elemIndex + 4] + elem[elemIndex + 11];
            var tmp5 = elem[elemIndex + 5] + elem[elemIndex + 10];
            var tmp6 = elem[elemIndex + 6] + elem[elemIndex + 9];
            var tmp7 = elem[elemIndex + 7] + elem[elemIndex + 8];

            var tmp10 = tmp0 + tmp7;
            var tmp14 = tmp0 - tmp7;
            var tmp11 = tmp1 + tmp6;
            var tmp15 = tmp1 - tmp6;
            var tmp12 = tmp2 + tmp5;
            var tmp16 = tmp2 - tmp5;
            var tmp13 = tmp3 + tmp4;
            var tmp17 = tmp3 - tmp4;

            tmp0 = elem[elemIndex + 0] - elem[elemIndex + 15];
            tmp1 = elem[elemIndex + 1] - elem[elemIndex + 14];
            tmp2 = elem[elemIndex + 2] - elem[elemIndex + 13];
            tmp3 = elem[elemIndex + 3] - elem[elemIndex + 12];
            tmp4 = elem[elemIndex + 4] - elem[elemIndex + 11];
            tmp5 = elem[elemIndex + 5] - elem[elemIndex + 10];
            tmp6 = elem[elemIndex + 6] - elem[elemIndex + 9];
            tmp7 = elem[elemIndex + 7] - elem[elemIndex + 8];

            /* Apply unsigned->signed conversion. */
            data[dataIndex + 0] =
              ((tmp10 + tmp11 + tmp12 + tmp13 - (16 * JpegConstants.CENTERJSAMPLE)) << SLOW_INTEGER_PASS1_BITS);
            data[dataIndex + 4] =
              JpegUtils.DESCALE(
                  ((tmp10 - tmp13) * SlowIntegerFix(1.306562965)) + /* c4[16] = c2[8] */
                  ((tmp11 - tmp12) * SLOW_INTEGER_FIX_0_541196100),   /* c12[16] = c6[8] */
                  SLOW_INTEGER_CONST_BITS - SLOW_INTEGER_PASS1_BITS);

            tmp10 = ((tmp17 - tmp15) * SlowIntegerFix(0.275899379)) +   /* c14[16] = c7[8] */
                ((tmp14 - tmp16) * SlowIntegerFix(1.387039845));    /* c2[16] = c1[8] */

            data[dataIndex + 2] = JpegUtils.DESCALE(
                tmp10 + (tmp15 * SlowIntegerFix(1.451774982))   /* c6+c14 */
                + (tmp16 * SlowIntegerFix(2.172734804)),        /* c2+c10 */
                SLOW_INTEGER_CONST_BITS - SLOW_INTEGER_PASS1_BITS);
            data[dataIndex + 6] = JpegUtils.DESCALE(
                tmp10 - (tmp14 * SlowIntegerFix(0.211164243))   /* c2-c6 */
                - (tmp17 * SlowIntegerFix(1.061594338)),        /* c10+c14 */
                SLOW_INTEGER_CONST_BITS - SLOW_INTEGER_PASS1_BITS);

            /* Odd part */

            tmp11 = ((tmp0 + tmp1) * SlowIntegerFix(1.353318001)) +         /* c3 */
                ((tmp6 - tmp7) * SlowIntegerFix(0.410524528));          /* c13 */
            tmp12 = ((tmp0 + tmp2) * SlowIntegerFix(1.247225013)) +         /* c5 */
                ((tmp5 + tmp7) * SlowIntegerFix(0.666655658));          /* c11 */
            tmp13 = ((tmp0 + tmp3) * SlowIntegerFix(1.093201867)) +         /* c7 */
                ((tmp4 - tmp7) * SlowIntegerFix(0.897167586));          /* c9 */
            tmp14 = ((tmp1 + tmp2) * SlowIntegerFix(0.138617169)) +         /* c15 */
                ((tmp6 - tmp5) * SlowIntegerFix(1.407403738));          /* c1 */
            tmp15 = ((tmp1 + tmp3) * -SlowIntegerFix(0.666655658)) +       /* -c11 */
                ((tmp4 + tmp6) * -SlowIntegerFix(1.247225013));        /* -c5 */
            tmp16 = ((tmp2 + tmp3) * -SlowIntegerFix(1.353318001)) +       /* -c3 */
                ((tmp5 - tmp4) * SlowIntegerFix(0.410524528));          /* c13 */
            tmp10 = tmp11 + tmp12 + tmp13 -
                (tmp0 * SlowIntegerFix(2.286341144)) +                /* c7+c5+c3-c1 */
                (tmp7 * SlowIntegerFix(0.779653625));                 /* c15+c13-c11+c9 */
            tmp11 += tmp14 + tmp15 + (tmp1 * SlowIntegerFix(0.071888074)) /* c9-c3-c15+c11 */
                 - (tmp6 * SlowIntegerFix(1.663905119));              /* c7+c13+c1-c5 */
            tmp12 += tmp14 + tmp16 - (tmp2 * SlowIntegerFix(1.125726048)) /* c7+c5+c15-c3 */
                 + (tmp5 * SlowIntegerFix(1.227391138));              /* c9-c11+c1-c13 */
            tmp13 += tmp15 + tmp16 + (tmp3 * SlowIntegerFix(1.065388962)) /* c15+c3+c11-c7 */
                 + (tmp4 * SlowIntegerFix(2.167985692));              /* c1+c13+c5-c9 */

            data[dataIndex + 1] = JpegUtils.DESCALE(tmp10, SLOW_INTEGER_CONST_BITS - SLOW_INTEGER_PASS1_BITS);
            data[dataIndex + 3] = JpegUtils.DESCALE(tmp11, SLOW_INTEGER_CONST_BITS - SLOW_INTEGER_PASS1_BITS);
            data[dataIndex + 5] = JpegUtils.DESCALE(tmp12, SLOW_INTEGER_CONST_BITS - SLOW_INTEGER_PASS1_BITS);
            data[dataIndex + 7] = JpegUtils.DESCALE(tmp13, SLOW_INTEGER_CONST_BITS - SLOW_INTEGER_PASS1_BITS);

            ctr++;

            if (ctr != JpegConstants.DCTSIZE)
            {
                if (ctr == JpegConstants.DCTSIZE * 2)
                {
                    /* Done. */
                    break;
                }

                /* advance pointer to next row */
                dataIndex += JpegConstants.DCTSIZE;
            }
            else
            {
                /* switch pointer to extended workspace */
                data = workspace;
                dataIndex = 0;
            }
        }

        /* Pass 2: process columns.
         * We remove the PASS1_BITS scaling, but leave the results scaled up
         * by an overall factor of 8.
         * We must also scale the output by (8/16)**2 = 1/2**2.
         * cK represents sqrt(2) * cos(K*pi/32).
         */

        data = data1;
        dataIndex = 0;
        var workspaceIndex = 0;
        for (ctr = JpegConstants.DCTSIZE - 1; ctr >= 0; ctr--)
        {
            /* Even part */
            var tmp0 = data[dataIndex + (JpegConstants.DCTSIZE * 0)] + workspace[workspaceIndex + (JpegConstants.DCTSIZE * 7)];
            var tmp1 = data[dataIndex + (JpegConstants.DCTSIZE * 1)] + workspace[workspaceIndex + (JpegConstants.DCTSIZE * 6)];
            var tmp2 = data[dataIndex + (JpegConstants.DCTSIZE * 2)] + workspace[workspaceIndex + (JpegConstants.DCTSIZE * 5)];
            var tmp3 = data[dataIndex + (JpegConstants.DCTSIZE * 3)] + workspace[workspaceIndex + (JpegConstants.DCTSIZE * 4)];
            var tmp4 = data[dataIndex + (JpegConstants.DCTSIZE * 4)] + workspace[workspaceIndex + (JpegConstants.DCTSIZE * 3)];
            var tmp5 = data[dataIndex + (JpegConstants.DCTSIZE * 5)] + workspace[workspaceIndex + (JpegConstants.DCTSIZE * 2)];
            var tmp6 = data[dataIndex + (JpegConstants.DCTSIZE * 6)] + workspace[workspaceIndex + (JpegConstants.DCTSIZE * 1)];
            var tmp7 = data[dataIndex + (JpegConstants.DCTSIZE * 7)] + workspace[workspaceIndex + (JpegConstants.DCTSIZE * 0)];

            var tmp10 = tmp0 + tmp7;
            var tmp14 = tmp0 - tmp7;
            var tmp11 = tmp1 + tmp6;
            var tmp15 = tmp1 - tmp6;
            var tmp12 = tmp2 + tmp5;
            var tmp16 = tmp2 - tmp5;
            var tmp13 = tmp3 + tmp4;
            var tmp17 = tmp3 - tmp4;

            tmp0 = data[dataIndex + (JpegConstants.DCTSIZE * 0)] - workspace[workspaceIndex + (JpegConstants.DCTSIZE * 7)];
            tmp1 = data[dataIndex + (JpegConstants.DCTSIZE * 1)] - workspace[workspaceIndex + (JpegConstants.DCTSIZE * 6)];
            tmp2 = data[dataIndex + (JpegConstants.DCTSIZE * 2)] - workspace[workspaceIndex + (JpegConstants.DCTSIZE * 5)];
            tmp3 = data[dataIndex + (JpegConstants.DCTSIZE * 3)] - workspace[workspaceIndex + (JpegConstants.DCTSIZE * 4)];
            tmp4 = data[dataIndex + (JpegConstants.DCTSIZE * 4)] - workspace[workspaceIndex + (JpegConstants.DCTSIZE * 3)];
            tmp5 = data[dataIndex + (JpegConstants.DCTSIZE * 5)] - workspace[workspaceIndex + (JpegConstants.DCTSIZE * 2)];
            tmp6 = data[dataIndex + (JpegConstants.DCTSIZE * 6)] - workspace[workspaceIndex + (JpegConstants.DCTSIZE * 1)];
            tmp7 = data[dataIndex + (JpegConstants.DCTSIZE * 7)] - workspace[workspaceIndex + (JpegConstants.DCTSIZE * 0)];

            data[dataIndex + (JpegConstants.DCTSIZE * 0)] =
              JpegUtils.DESCALE(tmp10 + tmp11 + tmp12 + tmp13, SLOW_INTEGER_PASS1_BITS + 2);
            data[dataIndex + (JpegConstants.DCTSIZE * 4)] =
              JpegUtils.DESCALE(((tmp10 - tmp13) * SlowIntegerFix(1.306562965)) + /* c4[16] = c2[8] */
                  ((tmp11 - tmp12) * SLOW_INTEGER_FIX_0_541196100),   /* c12[16] = c6[8] */
                  SLOW_INTEGER_CONST_BITS + SLOW_INTEGER_PASS1_BITS + 2);

            tmp10 = ((tmp17 - tmp15) * SlowIntegerFix(0.275899379)) +   /* c14[16] = c7[8] */
                ((tmp14 - tmp16) * SlowIntegerFix(1.387039845));    /* c2[16] = c1[8] */

            data[dataIndex + (JpegConstants.DCTSIZE * 2)] =
              JpegUtils.DESCALE(tmp10 + (tmp15 * SlowIntegerFix(1.451774982))   /* c6+c14 */
                  + (tmp16 * SlowIntegerFix(2.172734804)),        /* c2+10 */
                  SLOW_INTEGER_CONST_BITS + SLOW_INTEGER_PASS1_BITS + 2);
            data[dataIndex + (JpegConstants.DCTSIZE * 6)] =
              JpegUtils.DESCALE(tmp10 - (tmp14 * SlowIntegerFix(0.211164243))   /* c2-c6 */
                  - (tmp17 * SlowIntegerFix(1.061594338)),        /* c10+c14 */
                  SLOW_INTEGER_CONST_BITS + SLOW_INTEGER_PASS1_BITS + 2);

            /* Odd part */

            tmp11 = ((tmp0 + tmp1) * SlowIntegerFix(1.353318001)) +         /* c3 */
                ((tmp6 - tmp7) * SlowIntegerFix(0.410524528));          /* c13 */
            tmp12 = ((tmp0 + tmp2) * SlowIntegerFix(1.247225013)) +         /* c5 */
                ((tmp5 + tmp7) * SlowIntegerFix(0.666655658));          /* c11 */
            tmp13 = ((tmp0 + tmp3) * SlowIntegerFix(1.093201867)) +         /* c7 */
                ((tmp4 - tmp7) * SlowIntegerFix(0.897167586));          /* c9 */
            tmp14 = ((tmp1 + tmp2) * SlowIntegerFix(0.138617169)) +         /* c15 */
                ((tmp6 - tmp5) * SlowIntegerFix(1.407403738));          /* c1 */
            tmp15 = ((tmp1 + tmp3) * -SlowIntegerFix(0.666655658)) +       /* -c11 */
                ((tmp4 + tmp6) * -SlowIntegerFix(1.247225013));        /* -c5 */
            tmp16 = ((tmp2 + tmp3) * -SlowIntegerFix(1.353318001)) +       /* -c3 */
                ((tmp5 - tmp4) * SlowIntegerFix(0.410524528));          /* c13 */
            tmp10 = tmp11 + tmp12 + tmp13 -
                (tmp0 * SlowIntegerFix(2.286341144)) +                /* c7+c5+c3-c1 */
                (tmp7 * SlowIntegerFix(0.779653625));                 /* c15+c13-c11+c9 */
            tmp11 += tmp14 + tmp15 + (tmp1 * SlowIntegerFix(0.071888074)) /* c9-c3-c15+c11 */
                 - (tmp6 * SlowIntegerFix(1.663905119));              /* c7+c13+c1-c5 */
            tmp12 += tmp14 + tmp16 - (tmp2 * SlowIntegerFix(1.125726048)) /* c7+c5+c15-c3 */
                 + (tmp5 * SlowIntegerFix(1.227391138));              /* c9-c11+c1-c13 */
            tmp13 += tmp15 + tmp16 + (tmp3 * SlowIntegerFix(1.065388962)) /* c15+c3+c11-c7 */
                 + (tmp4 * SlowIntegerFix(2.167985692));              /* c1+c13+c5-c9 */

            data[dataIndex + (JpegConstants.DCTSIZE * 1)] = JpegUtils.DESCALE(tmp10, SLOW_INTEGER_CONST_BITS + SLOW_INTEGER_PASS1_BITS + 2);
            data[dataIndex + (JpegConstants.DCTSIZE * 3)] = JpegUtils.DESCALE(tmp11, SLOW_INTEGER_CONST_BITS + SLOW_INTEGER_PASS1_BITS + 2);
            data[dataIndex + (JpegConstants.DCTSIZE * 5)] = JpegUtils.DESCALE(tmp12, SLOW_INTEGER_CONST_BITS + SLOW_INTEGER_PASS1_BITS + 2);
            data[dataIndex + (JpegConstants.DCTSIZE * 7)] = JpegUtils.DESCALE(tmp13, SLOW_INTEGER_CONST_BITS + SLOW_INTEGER_PASS1_BITS + 2);

            dataIndex++;            /* advance pointer to next column */
            workspaceIndex++;            /* advance pointer to next column */
        }
    }

    private void JpegForwardDct16x8(int[] data, byte[][] sample_data, int start_row, int start_col)
    {
        m_cinfo.ErrExit(JMessageCode.JERR_NOTIMPL);
    }

    private void JpegForwardDct14x7(int[] data, byte[][] sample_data, int start_row, int start_col)
    {
        m_cinfo.ErrExit(JMessageCode.JERR_NOTIMPL);
    }

    private void JpegForwardDct12x6(int[] data, byte[][] sample_data, int start_row, int start_col)
    {
        m_cinfo.ErrExit(JMessageCode.JERR_NOTIMPL);
    }

    private void JpegForwardDct10x5(int[] data, byte[][] sample_data, int start_row, int start_col)
    {
        m_cinfo.ErrExit(JMessageCode.JERR_NOTIMPL);
    }

    private void JpegForwardDct8x4(int[] data, byte[][] sample_data, int start_row, int start_col)
    {
        m_cinfo.ErrExit(JMessageCode.JERR_NOTIMPL);
    }

    private void JpegForwardDct6x3(int[] data, byte[][] sample_data, int start_row, int start_col)
    {
        m_cinfo.ErrExit(JMessageCode.JERR_NOTIMPL);
    }

    private void JpegForwardDct4x2(int[] data, byte[][] sample_data, int start_row, int start_col)
    {
        m_cinfo.ErrExit(JMessageCode.JERR_NOTIMPL);
    }

    private void JpegForwardDct2x1(int[] data, byte[][] sample_data, int start_row, int start_col)
    {
        m_cinfo.ErrExit(JMessageCode.JERR_NOTIMPL);
    }

    private void JpegForwardDct8x16(int[] data, byte[][] sample_data, int start_row, int start_col)
    {
        m_cinfo.ErrExit(JMessageCode.JERR_NOTIMPL);
    }

    private void JpegForwardDct7x14(int[] data, byte[][] sample_data, int start_row, int start_col)
    {
        m_cinfo.ErrExit(JMessageCode.JERR_NOTIMPL);
    }

    private void JpegForwardDct6x12(int[] data, byte[][] sample_data, int start_row, int start_col)
    {
        m_cinfo.ErrExit(JMessageCode.JERR_NOTIMPL);
    }

    private void JpegForwardDct5x10(int[] data, byte[][] sample_data, int start_row, int start_col)
    {
        m_cinfo.ErrExit(JMessageCode.JERR_NOTIMPL);
    }

    private void JpegForwardDct4x8(int[] data, byte[][] sample_data, int start_row, int start_col)
    {
        m_cinfo.ErrExit(JMessageCode.JERR_NOTIMPL);
    }

    private void JpegForwardDct3x6(int[] data, byte[][] sample_data, int start_row, int start_col)
    {
        m_cinfo.ErrExit(JMessageCode.JERR_NOTIMPL);
    }

    private void JpegForwardDct2x4(int[] data, byte[][] sample_data, int start_row, int start_col)
    {
        m_cinfo.ErrExit(JMessageCode.JERR_NOTIMPL);
    }

    private void JpegForwardDct1x2(int[] data, byte[][] sample_data, int start_row, int start_col)
    {
        m_cinfo.ErrExit(JMessageCode.JERR_NOTIMPL);
    }
}
