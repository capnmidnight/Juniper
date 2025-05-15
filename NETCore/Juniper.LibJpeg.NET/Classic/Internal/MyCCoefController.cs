/*
 * This file contains the coefficient buffer controller for compression.
 * This controller is the top level of the JPEG compressor proper.
 * The coefficient buffer lies between forward-DCT and entropy encoding steps.
 */

/* We use a full-image coefficient buffer when doing Huffman optimization,
 * and also for writing multiple-scan JPEG files.  In all cases, the DCT
 * step is run during the first pass, and subsequent passes need only read
 * the buffered coefficients.
 */

using System;

namespace BitMiracle.LibJpeg.Classic.Internal
{
    internal class MyCCoefController : JpegCCoefController
    {
        private JBufMode m_passModeSetByLastStartPass;
        private readonly JpegCompressStruct m_cinfo;

        private int m_iMCU_row_num;    /* iMCU row # within image */
        private int m_mcu_ctr;     /* counts MCUs processed in current row */
        private int m_MCU_vert_offset;        /* counts MCU rows within iMCU row */
        private int m_MCU_rows_per_iMCU_row;  /* number of such rows needed */

        /* For single-pass compression, it's sufficient to buffer just one MCU
        * (although this may prove a bit slow in practice).  We allocate a
        * workspace of C_MAX_BLOCKS_IN_MCU coefficient blocks, and reuse it for each
        * MCU constructed and sent.  (On 80x86, the workspace is FAR even though
        * it's not really very big; this is to keep the module interfaces unchanged
        * when a large coefficient buffer is necessary.)
        * In multi-pass modes, this array points to the current MCU's blocks
        * within the virtual arrays.
        */
        private readonly JBlock[][] m_MCU_buffer = new JBlock[JpegConstants.C_MAX_BLOCKS_IN_MCU][];

        /* In multi-pass modes, we need a virtual block array for each component. */
        private readonly JVirtArray<JBlock>[] m_whole_image = new JVirtArray<JBlock>[JpegConstants.MAX_COMPONENTS];

        public MyCCoefController(JpegCompressStruct cinfo, bool need_full_buffer)
        {
            m_cinfo = cinfo;

            /* Create the coefficient buffer. */
            if (need_full_buffer)
            {
                /* Allocate a full-image virtual array for each component, */
                /* padded to a multiple of samp_factor DCT blocks in each direction. */
                for (var ci = 0; ci < cinfo.m_num_components; ci++)
                {
                    m_whole_image[ci] = JpegCommonStruct.CreateBlocksArray(
                        JpegUtils.jround_up(cinfo.Component_info[ci].Width_in_blocks, cinfo.Component_info[ci].H_samp_factor),
                        JpegUtils.jround_up(cinfo.Component_info[ci].height_in_blocks, cinfo.Component_info[ci].V_samp_factor));
                    m_whole_image[ci].ErrorProcessor = cinfo;
                }
            }
            else
            {
                /* We only need a single-MCU buffer. */
                var buffer = new JBlock[JpegConstants.C_MAX_BLOCKS_IN_MCU];
                for (var i = 0; i < JpegConstants.C_MAX_BLOCKS_IN_MCU; i++)
                {
                    buffer[i] = new JBlock();
                }

                for (var i = 0; i < JpegConstants.C_MAX_BLOCKS_IN_MCU; i++)
                {
                    m_MCU_buffer[i] = new JBlock[JpegConstants.C_MAX_BLOCKS_IN_MCU - i];
                    for (var j = i; j < JpegConstants.C_MAX_BLOCKS_IN_MCU; j++)
                    {
                        m_MCU_buffer[i][j - i] = buffer[j];
                    }
                }

                /* flag for no virtual arrays */
                m_whole_image[0] = null;
            }
        }

        // Initialize for a processing pass.
        public virtual void StartPass(JBufMode pass_mode)
        {
            m_iMCU_row_num = 0;
            StartIMcuRow();

            switch (pass_mode)
            {
                case JBufMode.PassThrough:
                if (m_whole_image[0] is object)
                {
                    m_cinfo.ErrExit(JMessageCode.JERR_BAD_BUFFER_MODE);
                }

                break;

                case JBufMode.SaveAndPass:
                case JBufMode.CrankDest:
                if (m_whole_image[0] is null)
                {
                    m_cinfo.ErrExit(JMessageCode.JERR_BAD_BUFFER_MODE);
                }

                break;

                default:
                m_cinfo.ErrExit(JMessageCode.JERR_BAD_BUFFER_MODE);
                break;
            }

            m_passModeSetByLastStartPass = pass_mode;
        }

        public virtual bool CompressData(byte[][][] input_buf)
        {
            return m_passModeSetByLastStartPass switch
            {
                JBufMode.PassThrough => CompressDataImpl(input_buf),
                JBufMode.SaveAndPass => CompressFirstPass(input_buf),
                JBufMode.CrankDest => CompressOutput(),
                _ => false,
            };
        }

        /// <summary>
        /// <para>
        /// Process some data in the single-pass case.
        /// We process the equivalent of one fully interleaved MCU row ("iMCU" row)
        /// per call, ie, v_samp_factor block rows for each component in the image.
        /// Returns true if the iMCU row is completed, false if suspended.
        /// </para>
        /// <para>
        /// NB: input_buf contains a plane for each component in image,
        /// which we index according to the component's SOF position.
        /// </para>
        /// </summary>
        private bool CompressDataImpl(byte[][][] input_buf)
        {
            var last_MCU_col = m_cinfo.m_MCUs_per_row - 1;
            var last_iMCU_row = m_cinfo.m_total_iMCU_rows - 1;

            /* Loop to write as much as one whole iMCU row */
            for (var yoffset = m_MCU_vert_offset; yoffset < m_MCU_rows_per_iMCU_row; yoffset++)
            {
                for (var MCU_col_num = m_mcu_ctr; MCU_col_num <= last_MCU_col; MCU_col_num++)
                {
                    /* Determine where data comes from in input_buf and do the DCT thing.
                     * Each call on forward_DCT processes a horizontal row of DCT blocks
                     * as wide as an MCU; we rely on having allocated the MCU_buffer[] blocks
                     * sequentially.  Dummy blocks at the right or bottom edge are filled in
                     * specially.  The data in them does not matter for image reconstruction,
                     * so we fill them with values that will encode to the smallest amount of
                     * data, viz: all zeroes in the AC entries, DC entries equal to previous
                     * block's DC value.  (Thanks to Thomas Kinsman for this idea.)
                     */
                    var blkn = 0;
                    for (var ci = 0; ci < m_cinfo.m_comps_in_scan; ci++)
                    {
                        var componentInfo = m_cinfo.Component_info[m_cinfo.m_cur_comp_info[ci]];
                        var forward_DCT = m_cinfo.m_fdct.forward_DCT[componentInfo.Component_index];

                        var blockcnt = (MCU_col_num < last_MCU_col) ? componentInfo.MCU_width : componentInfo.last_col_width;
                        var xpos = MCU_col_num * componentInfo.MCU_sample_width;
                        var ypos = yoffset * componentInfo.DCT_v_scaled_size;

                        for (var yindex = 0; yindex < componentInfo.MCU_height; yindex++)
                        {
                            if (m_iMCU_row_num < last_iMCU_row || yoffset + yindex < componentInfo.last_row_height)
                            {
                                forward_DCT(componentInfo, input_buf[componentInfo.Component_index],
                                    m_MCU_buffer[blkn], ypos, xpos, blockcnt);

                                if (blockcnt < componentInfo.MCU_width)
                                {
                                    /* Create some dummy blocks at the right edge of the image. */
                                    for (var i = 0; i < (componentInfo.MCU_width - blockcnt); i++)
                                    {
                                        Array.Clear(m_MCU_buffer[blkn + blockcnt][i].data, 0, m_MCU_buffer[blkn + blockcnt][i].data.Length);
                                    }

                                    for (var bi = blockcnt; bi < componentInfo.MCU_width; bi++)
                                    {
                                        m_MCU_buffer[blkn + bi][0][0] = m_MCU_buffer[blkn + bi - 1][0][0];
                                    }
                                }
                            }
                            else
                            {
                                /* Create a row of dummy blocks at the bottom of the image. */
                                for (var i = 0; i < componentInfo.MCU_width; i++)
                                {
                                    Array.Clear(m_MCU_buffer[blkn][i].data, 0, m_MCU_buffer[blkn][i].data.Length);
                                }

                                for (var bi = 0; bi < componentInfo.MCU_width; bi++)
                                {
                                    m_MCU_buffer[blkn + bi][0][0] = m_MCU_buffer[blkn - 1][0][0];
                                }
                            }

                            blkn += componentInfo.MCU_width;
                            ypos += componentInfo.DCT_v_scaled_size;
                        }
                    }

                    /* Try to write the MCU.  In event of a suspension failure, we will
                     * re-DCT the MCU on restart (a bit inefficient, could be fixed...)
                     */
                    if (!m_cinfo.m_entropy.encodeMcu(m_MCU_buffer))
                    {
                        /* Suspension forced; update state counters and exit */
                        m_MCU_vert_offset = yoffset;
                        m_mcu_ctr = MCU_col_num;
                        return false;
                    }
                }

                /* Completed an MCU row, but perhaps not an iMCU row */
                m_mcu_ctr = 0;
            }

            /* Completed the iMCU row, advance counters for next one */
            m_iMCU_row_num++;
            StartIMcuRow();
            return true;
        }

        /// <summary>
        /// <para>
        /// Process some data in the first pass of a multi-pass case.
        /// We process the equivalent of one fully interleaved MCU row ("iMCU" row)
        /// per call, ie, v_samp_factor block rows for each component in the image.
        /// This amount of data is read from the source buffer, DCT'd and quantized,
        /// and saved into the virtual arrays.  We also generate suitable dummy blocks
        /// as needed at the right and lower edges.  (The dummy blocks are constructed
        /// in the virtual arrays, which have been padded appropriately.)  This makes
        /// it possible for subsequent passes not to worry about real vs. dummy blocks.
        /// </para>
        /// <para>
        /// We must also emit the data to the entropy encoder.  This is conveniently
        /// done by calling compress_output() after we've loaded the current strip
        /// of the virtual arrays.
        /// </para>
        /// <para>
        /// NB: input_buf contains a plane for each component in image.  All
        /// components are DCT'd and loaded into the virtual arrays in this pass.
        /// However, it may be that only a subset of the components are emitted to
        /// the entropy encoder during this first pass; be careful about looking
        /// at the scan-dependent variables (MCU dimensions, etc).
        /// </para>
        /// </summary>
        private bool CompressFirstPass(byte[][][] input_buf)
        {
            var last_iMCU_row = m_cinfo.m_total_iMCU_rows - 1;

            for (var ci = 0; ci < m_cinfo.m_num_components; ci++)
            {
                var componentInfo = m_cinfo.Component_info[ci];

                /* Align the virtual buffer for this component. */
                var buffer = m_whole_image[ci].Access(m_iMCU_row_num * componentInfo.V_samp_factor,
                    componentInfo.V_samp_factor);

                /* Count non-dummy DCT block rows in this iMCU row. */
                int block_rows;
                if (m_iMCU_row_num < last_iMCU_row)
                {
                    block_rows = componentInfo.V_samp_factor;
                }
                else
                {
                    /* NB: can't use last_row_height here, since may not be set! */
                    block_rows = componentInfo.height_in_blocks % componentInfo.V_samp_factor;
                    if (block_rows == 0)
                    {
                        block_rows = componentInfo.V_samp_factor;
                    }
                }

                var blocks_across = componentInfo.Width_in_blocks;
                var h_samp_factor = componentInfo.H_samp_factor;

                /* Count number of dummy blocks to be added at the right margin. */
                var ndummy = blocks_across % h_samp_factor;
                if (ndummy > 0)
                {
                    ndummy = h_samp_factor - ndummy;
                }

                var forward_DCT = m_cinfo.m_fdct.forward_DCT[ci];
                /* Perform DCT for all non-dummy blocks in this iMCU row.  Each call
                 * on forward_DCT processes a complete horizontal row of DCT blocks.
                 */
                for (var block_row = 0; block_row < block_rows; block_row++)
                {
                    forward_DCT(componentInfo, input_buf[ci],
                        buffer[block_row], block_row * componentInfo.DCT_v_scaled_size, 0, blocks_across);

                    if (ndummy > 0)
                    {
                        /* Create dummy blocks at the right edge of the image. */
                        Array.Clear(buffer[block_row][blocks_across].data, 0, buffer[block_row][blocks_across].data.Length);

                        var lastDC = buffer[block_row][blocks_across - 1][0];
                        for (var bi = 0; bi < ndummy; bi++)
                        {
                            buffer[block_row][blocks_across + bi][0] = lastDC;
                        }
                    }
                }

                /* If at end of image, create dummy block rows as needed.
                 * The tricky part here is that within each MCU, we want the DC values
                 * of the dummy blocks to match the last real block's DC value.
                 * This squeezes a few more bytes out of the resulting file...
                 */
                if (m_iMCU_row_num == last_iMCU_row)
                {
                    blocks_across += ndummy;    /* include lower right corner */
                    var MCUs_across = blocks_across / h_samp_factor;
                    for (var block_row = block_rows; block_row < componentInfo.V_samp_factor; block_row++)
                    {
                        for (var i = 0; i < blocks_across; i++)
                        {
                            Array.Clear(buffer[block_row][i].data, 0, buffer[block_row][i].data.Length);
                        }

                        var thisOffset = 0;
                        var lastOffset = 0;
                        for (var MCUindex = 0; MCUindex < MCUs_across; MCUindex++)
                        {
                            var lastDC = buffer[block_row - 1][lastOffset + h_samp_factor - 1][0];
                            for (var bi = 0; bi < h_samp_factor; bi++)
                            {
                                buffer[block_row][thisOffset + bi][0] = lastDC;
                            }

                            thisOffset += h_samp_factor; /* advance to next MCU in row */
                            lastOffset += h_samp_factor;
                        }
                    }
                }
            }

            /* NB: compress_output will increment iMCU_row_num if successful.
             * A suspension return will result in redoing all the work above next time.
             */

            /* Emit data to the entropy encoder, sharing code with subsequent passes */
            return CompressOutput();
        }

        /// <summary>
        /// Process some data in subsequent passes of a multi-pass case.
        /// We process the equivalent of one fully interleaved MCU row ("iMCU" row)
        /// per call, ie, v_samp_factor block rows for each component in the scan.
        /// The data is obtained from the virtual arrays and fed to the entropy coder.
        /// Returns true if the iMCU row is completed, false if suspended.
        /// </summary>
        private bool CompressOutput()
        {
            /* Align the virtual buffers for the components used in this scan.
             */
            var buffer = new JBlock[JpegConstants.MAX_COMPS_IN_SCAN][][];
            for (var ci = 0; ci < m_cinfo.m_comps_in_scan; ci++)
            {
                var componentInfo = m_cinfo.Component_info[m_cinfo.m_cur_comp_info[ci]];
                buffer[ci] = m_whole_image[componentInfo.Component_index].Access(
                    m_iMCU_row_num * componentInfo.V_samp_factor, componentInfo.V_samp_factor);
            }

            /* Loop to process one whole iMCU row */
            for (var yoffset = m_MCU_vert_offset; yoffset < m_MCU_rows_per_iMCU_row; yoffset++)
            {
                for (var MCU_col_num = m_mcu_ctr; MCU_col_num < m_cinfo.m_MCUs_per_row; MCU_col_num++)
                {
                    /* Construct list of pointers to DCT blocks belonging to this MCU */
                    var blkn = 0;           /* index of current DCT block within MCU */
                    for (var ci = 0; ci < m_cinfo.m_comps_in_scan; ci++)
                    {
                        var componentInfo = m_cinfo.Component_info[m_cinfo.m_cur_comp_info[ci]];
                        var start_col = MCU_col_num * componentInfo.MCU_width;
                        for (var yindex = 0; yindex < componentInfo.MCU_height; yindex++)
                        {
                            for (var xindex = 0; xindex < componentInfo.MCU_width; xindex++)
                            {
                                var bufLength = buffer[ci][yindex + yoffset].Length;
                                var start = start_col + xindex;
                                m_MCU_buffer[blkn] = new JBlock[bufLength - start];
                                for (var j = start; j < bufLength; j++)
                                {
                                    m_MCU_buffer[blkn][j - start] = buffer[ci][yindex + yoffset][j];
                                }

                                blkn++;
                            }
                        }
                    }

                    /* Try to write the MCU. */
                    if (!m_cinfo.m_entropy.encodeMcu(m_MCU_buffer))
                    {
                        /* Suspension forced; update state counters and exit */
                        m_MCU_vert_offset = yoffset;
                        m_mcu_ctr = MCU_col_num;
                        return false;
                    }
                }

                /* Completed an MCU row, but perhaps not an iMCU row */
                m_mcu_ctr = 0;
            }

            /* Completed the iMCU row, advance counters for next one */
            m_iMCU_row_num++;
            StartIMcuRow();
            return true;
        }

        // Reset within-iMCU-row counters for a new row
        private void StartIMcuRow()
        {
            /* In an interleaved scan, an MCU row is the same as an iMCU row.
             * In a noninterleaved scan, an iMCU row has v_samp_factor MCU rows.
             * But at the bottom of the image, process only what's left.
             */
            if (m_cinfo.m_comps_in_scan > 1)
            {
                m_MCU_rows_per_iMCU_row = 1;
            }
            else
            {
                if (m_iMCU_row_num < (m_cinfo.m_total_iMCU_rows - 1))
                {
                    m_MCU_rows_per_iMCU_row = m_cinfo.Component_info[m_cinfo.m_cur_comp_info[0]].V_samp_factor;
                }
                else
                {
                    m_MCU_rows_per_iMCU_row = m_cinfo.Component_info[m_cinfo.m_cur_comp_info[0]].last_row_height;
                }
            }

            m_mcu_ctr = 0;
            m_MCU_vert_offset = 0;
        }
    }
}
