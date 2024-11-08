/*
 * This file contains the decompression postprocessing controller.
 * This controller manages the upsampling, color conversion, and color
 * quantization/reduction steps; specifically, it controls the buffering
 * between upsample/color conversion and color quantization/reduction.
 *
 * If no color quantization/reduction is required, then this module has no
 * work to do, and it just hands off to the upsample/color conversion code.
 * An integrated upsample/convert/quantize process would replace this module
 * entirely.
 */

namespace BitMiracle.LibJpeg.Classic.Internal;

/// <summary>
/// Decompression postprocessing (color quantization buffer control)
/// </summary>
internal class JpegDPostController
{
    private enum ProcessorType
    {
        OnePass,
        PrePass,
        Upsample,
        SecondPass
    }

    private ProcessorType m_processor;

    private readonly JpegDecompressStruct m_cinfo;

    /* Color quantization source buffer: this holds output data from
    * the upsample/color conversion step to be passed to the quantizer.
    * For two-pass color quantization, we need a full-image buffer;
    * for one-pass operation, a strip buffer is sufficient.
    */
    private readonly JVirtArray<byte> m_whole_image;  /* virtual array, or null if one-pass */
    private byte[][] m_buffer;       /* strip buffer, or current strip of virtual */
    private readonly int m_strip_height;    /* buffer size in rows */
    /* for two-pass mode only: */
    private int m_starting_row;    /* row # of first row in current strip */
    private int m_next_row;        /* index of next row to fill/empty in strip */

    /// <summary>
    /// Initialize postprocessing controller.
    /// </summary>
    public JpegDPostController(JpegDecompressStruct cinfo, bool need_full_buffer)
    {
        m_cinfo = cinfo;

        /* Create the quantization buffer, if needed */
        if (cinfo.quantizeColors)
        {
            /* The buffer strip height is max_v_samp_factor, which is typically
            * an efficient number of rows for upsampling to return.
            * (In the presence of output rescaling, we might want to be smarter?)
            */
            m_strip_height = cinfo.m_maxVSampleFactor;

            if (need_full_buffer)
            {
                /* Two-pass color quantization: need full-image storage. */
                /* We round up the number of rows to a multiple of the strip height. */
                m_whole_image = JpegCommonStruct.CreateSamplesArray(
                    cinfo.outputWidth * cinfo.outColorComponents,
                    JpegUtils.jround_up(cinfo.outputHeight, m_strip_height));
                m_whole_image.ErrorProcessor = cinfo;
            }
            else
            {
                /* One-pass color quantization: just make a strip buffer. */
                m_buffer = JpegCommonStruct.AllocJpegSamples(
                    cinfo.outputWidth * cinfo.outColorComponents, m_strip_height);
            }
        }
    }

    /// <summary>
    /// Initialize for a processing pass.
    /// </summary>
    public void StartPass(JBufMode pass_mode)
    {
        switch (pass_mode)
        {
            case JBufMode.PassThrough:
            if (m_cinfo.quantizeColors)
            {
                /* Single-pass processing with color quantization. */
                m_processor = ProcessorType.OnePass;
                /* We could be doing buffered-image output before starting a 2-pass
                 * color quantization; in that case, jinit_d_post_controller did not
                 * allocate a strip buffer.  Use the virtual-array buffer as workspace.
                 */
                if (m_buffer is null)
                {
                    m_buffer = m_whole_image.Access(0, m_strip_height);
                }
            }
            else
            {
                /* For single-pass processing without color quantization,
                 * I have no work to do; just call the upsampler directly.
                 */
                m_processor = ProcessorType.Upsample;
            }
            break;
            case JBufMode.SaveAndPass:
            /* First pass of 2-pass quantization */
            if (m_whole_image is null)
            {
                m_cinfo.ErrExit(JMessageCode.JERR_BAD_BUFFER_MODE);
            }

            m_processor = ProcessorType.PrePass;
            break;
            case JBufMode.CrankDest:
            /* Second pass of 2-pass quantization */
            if (m_whole_image is null)
            {
                m_cinfo.ErrExit(JMessageCode.JERR_BAD_BUFFER_MODE);
            }

            m_processor = ProcessorType.SecondPass;
            break;
            default:
            m_cinfo.ErrExit(JMessageCode.JERR_BAD_BUFFER_MODE);
            break;
        }

        m_starting_row = m_next_row = 0;
    }

    public void PostProcessData(ComponentBuffer[] input_buf, ref int in_row_group_ctr, int in_row_groups_avail, byte[][] output_buf, ref int out_row_ctr, int out_rows_avail)
    {
        switch (m_processor)
        {
            case ProcessorType.OnePass:
            PostProcess1Pass(input_buf, ref in_row_group_ctr, in_row_groups_avail, output_buf, ref out_row_ctr, out_rows_avail);
            break;
            case ProcessorType.PrePass:
            PostProcessPrepass(input_buf, ref in_row_group_ctr, in_row_groups_avail, ref out_row_ctr);
            break;
            case ProcessorType.Upsample:
            m_cinfo.m_upsample.UpSample(input_buf, ref in_row_group_ctr, in_row_groups_avail, output_buf, ref out_row_ctr, out_rows_avail);
            break;
            case ProcessorType.SecondPass:
            PostProcess2Pass(output_buf, ref out_row_ctr, out_rows_avail);
            break;
            default:
            m_cinfo.ErrExit(JMessageCode.JERR_NOTIMPL);
            break;
        }
    }

    /// <summary>
    /// Process some data in the one-pass (strip buffer) case.
    /// This is used for color precision reduction as well as one-pass quantization.
    /// </summary>
    private void PostProcess1Pass(ComponentBuffer[] input_buf, ref int in_row_group_ctr, int in_row_groups_avail, byte[][] output_buf, ref int out_row_ctr, int out_rows_avail)
    {
        /* Fill the buffer, but not more than what we can dump out in one go. */
        /* Note we rely on the upsampler to detect bottom of image. */
        var max_rows = out_rows_avail - out_row_ctr;
        if (max_rows > m_strip_height)
        {
            max_rows = m_strip_height;
        }

        var num_rows = 0;
        m_cinfo.m_upsample.UpSample(input_buf, ref in_row_group_ctr, in_row_groups_avail, m_buffer, ref num_rows, max_rows);

        /* Quantize and emit data. */
        m_cinfo.m_cquantize.ColorQuantize(m_buffer, 0, output_buf, out_row_ctr, num_rows);
        out_row_ctr += num_rows;
    }

    /// <summary>
    /// Process some data in the first pass of 2-pass quantization.
    /// </summary>
    private void PostProcessPrepass(ComponentBuffer[] input_buf, ref int in_row_group_ctr, int in_row_groups_avail, ref int out_row_ctr)
    {
        int old_next_row, num_rows;

        /* Reposition virtual buffer if at start of strip. */
        if (m_next_row == 0)
        {
            m_buffer = m_whole_image.Access(m_starting_row, m_strip_height);
        }

        /* Upsample some data (up to a strip height's worth). */
        old_next_row = m_next_row;
        m_cinfo.m_upsample.UpSample(input_buf, ref in_row_group_ctr, in_row_groups_avail, m_buffer, ref m_next_row, m_strip_height);

        /* Allow quantizer to scan new data.  No data is emitted, */
        /* but we advance out_row_ctr so outer loop can tell when we're done. */
        if (m_next_row > old_next_row)
        {
            num_rows = m_next_row - old_next_row;
            m_cinfo.m_cquantize.ColorQuantize(m_buffer, old_next_row, null, 0, num_rows);
            out_row_ctr += num_rows;
        }

        /* Advance if we filled the strip. */
        if (m_next_row >= m_strip_height)
        {
            m_starting_row += m_strip_height;
            m_next_row = 0;
        }
    }

    /// <summary>
    /// Process some data in the second pass of 2-pass quantization.
    /// </summary>
    private void PostProcess2Pass(byte[][] output_buf, ref int out_row_ctr, int out_rows_avail)
    {
        int num_rows, max_rows;

        /* Reposition virtual buffer if at start of strip. */
        if (m_next_row == 0)
        {
            m_buffer = m_whole_image.Access(m_starting_row, m_strip_height);
        }

        /* Determine number of rows to emit. */
        num_rows = m_strip_height - m_next_row; /* available in strip */
        max_rows = out_rows_avail - out_row_ctr; /* available in output area */
        if (num_rows > max_rows)
        {
            num_rows = max_rows;
        }

        /* We have to check bottom of image here, can't depend on upsampler. */
        max_rows = m_cinfo.outputHeight - m_starting_row;
        if (num_rows > max_rows)
        {
            num_rows = max_rows;
        }

        /* Quantize and emit data. */
        m_cinfo.m_cquantize.ColorQuantize(m_buffer, m_next_row, output_buf, out_row_ctr, num_rows);
        out_row_ctr += num_rows;

        /* Advance if we filled the strip. */
        m_next_row += num_rows;
        if (m_next_row >= m_strip_height)
        {
            m_starting_row += m_strip_height;
            m_next_row = 0;
        }
    }
}
