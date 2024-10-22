namespace BitMiracle.LibJpeg.Classic.Internal;

/// <summary>
/// Master control module
/// </summary>
internal class JpegCompMaster
{
    private enum PassType
    {
        Main,      /* input data, also do first output step */
        HuffmanOptimization,  /* Huffman code optimization pass */
        Output     /* data output pass */
    }

    private readonly JpegCompressStruct m_cinfo;

    private bool m_call_pass_startup; /* True if pass_startup must be called */
    private bool m_is_last_pass;      /* True during last pass */

    private PassType m_pass_type;  /* the type of the current pass */

    private int m_pass_number;        /* # of passes completed */
    private readonly int m_total_passes;       /* total # of passes needed */

    private int m_scan_number;        /* current index in scan_info[] */

    public JpegCompMaster(JpegCompressStruct cinfo, bool transcode_only)
    {
        m_cinfo = cinfo;

        if (transcode_only)
        {
            /* no main pass in transcoding */
            if (cinfo.optimizeEntropyCoding)
            {
                m_pass_type = PassType.HuffmanOptimization;
            }
            else
            {
                m_pass_type = PassType.Output;
            }
        }
        else
        {
            /* for normal compression, first pass is always this type: */
            m_pass_type = PassType.Main;
        }

        if (cinfo.optimizeEntropyCoding)
        {
            m_total_passes = cinfo.m_num_scans * 2;
        }
        else
        {
            m_total_passes = cinfo.m_num_scans;
        }
    }

    /// <summary>
    /// <para>Per-pass setup.</para>
    /// <para>
    /// This is called at the beginning of each pass.  We determine which 
    /// modules will be active during this pass and give them appropriate 
    /// start_pass calls. 
    /// We also set is_last_pass to indicate whether any more passes will 
    /// be required.
    /// </para>
    /// </summary>
    public void PrepareForPass()
    {
        switch (m_pass_type)
        {
            case PassType.Main:
            PrepareForMainPass();
            break;
            case PassType.HuffmanOptimization:
            if (!PrepareForHuffmanOptimizationPass())
            {
                break;
            }

            PrepareForOutputPass();
            break;
            case PassType.Output:
            PrepareForOutputPass();
            break;
            default:
            m_cinfo.ErrExit(JMessageCode.JERR_NOT_COMPILED);
            break;
        }

        m_is_last_pass = (m_pass_number == m_total_passes - 1);

        /* Set up progress monitor's pass info if present */
        if (m_cinfo.prog is object)
        {
            m_cinfo.prog.CompletedPasses = m_pass_number;
            m_cinfo.prog.TotalPasses = m_total_passes;
        }
    }

    /// <summary>
    /// <para>Special start-of-pass hook.</para>
    /// <para>
    /// This is called by jpeg_write_scanlines if call_pass_startup is true.
    /// In single-pass processing, we need this hook because we don't want to
    /// write frame/scan headers during jpeg_start_compress; we want to let the
    /// application write COM markers etc. between jpeg_start_compress and the
    /// jpeg_write_scanlines loop.
    /// In multi-pass processing, this routine is not used.
    /// </para>
    /// </summary>
    public void PassStartup()
    {
        m_cinfo.m_master.m_call_pass_startup = false; /* reset flag so call only once */

        m_cinfo.m_marker.WriteFrameHeader();
        m_cinfo.m_marker.WriteScanHeader();
    }

    /// <summary>
    /// Finish up at end of pass.
    /// </summary>
    public void FinishPass()
    {
        /* The entropy coder always needs an end-of-pass call,
        * either to analyze statistics or to flush its output buffer.
        */
        m_cinfo.m_entropy.finishPass();

        /* Update state for next pass */
        switch (m_pass_type)
        {
            case PassType.Main:
            /* next pass is either output of scan 0 (after optimization)
            * or output of scan 1 (if no optimization).
            */
            m_pass_type = PassType.Output;
            if (!m_cinfo.optimizeEntropyCoding)
            {
                m_scan_number++;
            }

            break;
            case PassType.HuffmanOptimization:
            /* next pass is always output of current scan */
            m_pass_type = PassType.Output;
            break;
            case PassType.Output:
            /* next pass is either optimization or output of next scan */
            if (m_cinfo.optimizeEntropyCoding)
            {
                m_pass_type = PassType.HuffmanOptimization;
            }

            m_scan_number++;
            break;
        }

        m_pass_number++;
    }

    public bool IsLastPass()
    {
        return m_is_last_pass;
    }

    public bool MustCallPassStartup()
    {
        return m_call_pass_startup;
    }

    private void PrepareForMainPass()
    {
        /* Initial pass: will collect input data, and do either Huffman
        * optimization or data output for the first scan.
        */
        SelectScanParameters();
        PerScanSetup();

        if (!m_cinfo.m_raw_data_in)
        {
            m_cinfo.m_cconvert.StartPass();
            m_cinfo.m_prep.StartPass(JBufMode.PassThrough);
        }

        m_cinfo.m_fdct.StartPass();
        m_cinfo.m_entropy.StartPass(m_cinfo.optimizeEntropyCoding);
        m_cinfo.m_coef.StartPass(m_total_passes > 1 ? JBufMode.SaveAndPass : JBufMode.PassThrough);
        m_cinfo.m_main.start_pass(JBufMode.PassThrough);

        if (m_cinfo.optimizeEntropyCoding)
        {
            /* No immediate data output; postpone writing frame/scan headers */
            m_call_pass_startup = false;
        }
        else
        {
            /* Will write frame/scan headers at first jpeg_write_scanlines call */
            m_call_pass_startup = true;
        }
    }

    private bool PrepareForHuffmanOptimizationPass()
    {
        /* Do Huffman optimization for a scan after the first one. */
        SelectScanParameters();
        PerScanSetup();

        if (m_cinfo.m_Ss != 0 || m_cinfo.m_Ah == 0)
        {
            m_cinfo.m_entropy.StartPass(true);
            m_cinfo.m_coef.StartPass(JBufMode.CrankDest);
            m_call_pass_startup = false;
            return false;
        }

        /* Special case: Huffman DC refinement scans need no Huffman table
        * and therefore we can skip the optimization pass for them.
        */
        m_pass_type = PassType.Output;
        m_pass_number++;
        return true;
    }

    private void PrepareForOutputPass()
    {
        /* Do a data-output pass. */
        /* We need not repeat per-scan setup if prior optimization pass did it. */
        if (!m_cinfo.optimizeEntropyCoding)
        {
            SelectScanParameters();
            PerScanSetup();
        }

        m_cinfo.m_entropy.StartPass(false);
        m_cinfo.m_coef.StartPass(JBufMode.CrankDest);

        /* We emit frame/scan headers now */
        if (m_scan_number == 0)
        {
            m_cinfo.m_marker.WriteFrameHeader();
        }

        m_cinfo.m_marker.WriteScanHeader();
        m_call_pass_startup = false;
    }

    // Set up the scan parameters for the current scan
    private void SelectScanParameters()
    {
        if (m_cinfo.m_scan_info is object)
        {
            /* Prepare for current scan --- the script is already validated */
            var scanInfo = m_cinfo.m_scan_info[m_scan_number];

            m_cinfo.m_comps_in_scan = scanInfo.comps_in_scan;
            for (var ci = 0; ci < scanInfo.comps_in_scan; ci++)
            {
                m_cinfo.m_cur_comp_info[ci] = scanInfo.component_index[ci];
            }

            if (m_cinfo.m_progressive_mode)
            {
                m_cinfo.m_Ss = scanInfo.Ss;
                m_cinfo.m_Se = scanInfo.Se;
                m_cinfo.m_Ah = scanInfo.Ah;
                m_cinfo.m_Al = scanInfo.Al;
                return;
            }
        }
        else
        {
            /* Prepare for single sequential-JPEG scan containing all components */
            if (m_cinfo.m_num_components > JpegConstants.MAX_COMPS_IN_SCAN)
            {
                m_cinfo.ErrExit(JMessageCode.JERR_COMPONENT_COUNT, m_cinfo.m_num_components, JpegConstants.MAX_COMPS_IN_SCAN);
            }

            m_cinfo.m_comps_in_scan = m_cinfo.m_num_components;
            for (var ci = 0; ci < m_cinfo.m_num_components; ci++)
            {
                m_cinfo.m_cur_comp_info[ci] = ci;
            }
        }

        m_cinfo.m_Ss = 0;
        m_cinfo.m_Se = (m_cinfo.BlockSize * m_cinfo.BlockSize) - 1;
        m_cinfo.m_Ah = 0;
        m_cinfo.m_Al = 0;
    }

    /// <summary>
    /// Do computations that are needed before processing a JPEG scan
    /// cinfo.comps_in_scan and cinfo.cur_comp_info[] are already set
    /// </summary>
    private void PerScanSetup()
    {
        if (m_cinfo.m_comps_in_scan == 1)
        {
            /* Noninterleaved (single-component) scan */
            var compIndex = m_cinfo.m_cur_comp_info[0];
            var compptr = m_cinfo.Component_info[compIndex];

            /* Overall image size in MCUs */
            m_cinfo.m_MCUs_per_row = compptr.Width_in_blocks;
            m_cinfo.m_MCU_rows_in_scan = compptr.height_in_blocks;

            /* For noninterleaved scan, always one block per MCU */
            compptr.MCU_width = 1;
            compptr.MCU_height = 1;
            compptr.MCU_blocks = 1;
            compptr.MCU_sample_width = compptr.DCT_h_scaled_size;
            compptr.last_col_width = 1;

            /* For noninterleaved scans, it is convenient to define last_row_height
            * as the number of block rows present in the last iMCU row.
            */
            var tmp = compptr.height_in_blocks % compptr.V_samp_factor;
            if (tmp == 0)
            {
                tmp = compptr.V_samp_factor;
            }

            compptr.last_row_height = tmp;

            /* Prepare array describing MCU composition */
            m_cinfo.m_blocks_in_MCU = 1;
            m_cinfo.m_MCU_membership[0] = 0;
        }
        else
        {
            /* Interleaved (multi-component) scan */
            if (m_cinfo.m_comps_in_scan <= 0 || m_cinfo.m_comps_in_scan > JpegConstants.MAX_COMPS_IN_SCAN)
            {
                m_cinfo.ErrExit(JMessageCode.JERR_COMPONENT_COUNT, m_cinfo.m_comps_in_scan, JpegConstants.MAX_COMPS_IN_SCAN);
            }

            /* Overall image size in MCUs */
            m_cinfo.m_MCUs_per_row = (int)JpegUtils.jdiv_round_up(
                m_cinfo.jpeg_width, m_cinfo.m_max_h_samp_factor * m_cinfo.BlockSize);

            m_cinfo.m_MCU_rows_in_scan = (int)JpegUtils.jdiv_round_up(
                m_cinfo.jpeg_height, m_cinfo.m_max_v_samp_factor * m_cinfo.BlockSize);

            m_cinfo.m_blocks_in_MCU = 0;

            for (var ci = 0; ci < m_cinfo.m_comps_in_scan; ci++)
            {
                var compIndex = m_cinfo.m_cur_comp_info[ci];
                var compptr = m_cinfo.Component_info[compIndex];

                /* Sampling factors give # of blocks of component in each MCU */
                compptr.MCU_width = compptr.H_samp_factor;
                compptr.MCU_height = compptr.V_samp_factor;
                compptr.MCU_blocks = compptr.MCU_width * compptr.MCU_height;
                compptr.MCU_sample_width = compptr.MCU_width * compptr.DCT_h_scaled_size;

                /* Figure number of non-dummy blocks in last MCU column & row */
                var tmp = compptr.Width_in_blocks % compptr.MCU_width;
                if (tmp == 0)
                {
                    tmp = compptr.MCU_width;
                }

                compptr.last_col_width = tmp;

                tmp = compptr.height_in_blocks % compptr.MCU_height;
                if (tmp == 0)
                {
                    tmp = compptr.MCU_height;
                }

                compptr.last_row_height = tmp;

                /* Prepare array describing MCU composition */
                var mcublks = compptr.MCU_blocks;
                if (m_cinfo.m_blocks_in_MCU + mcublks > JpegConstants.C_MAX_BLOCKS_IN_MCU)
                {
                    m_cinfo.ErrExit(JMessageCode.JERR_BAD_MCU_SIZE);
                }

                while (mcublks-- > 0)
                {
                    m_cinfo.m_MCU_membership[m_cinfo.m_blocks_in_MCU++] = ci;
                }
            }
        }

        /* Convert restart specified in rows to actual MCU count. */
        /* Note that count must fit in 16 bits, so we provide limiting. */
        if (m_cinfo.m_restart_in_rows > 0)
        {
            var nominal = m_cinfo.m_restart_in_rows * m_cinfo.m_MCUs_per_row;
            m_cinfo.m_restart_interval = Math.Min(nominal, 65535);
        }
    }
}
