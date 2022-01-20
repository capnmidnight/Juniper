/*
 * This file contains the main buffer controller for compression.
 * The main buffer lies between the pre-processor and the JPEG
 * compressor proper; it holds downsampled data in the JPEG colorspace.
 */

namespace BitMiracle.LibJpeg.Classic.Internal
{
    /// <summary>
    /// Main buffer control (downsampled-data buffer)
    /// </summary>
    internal class JpegCMainController
    {
        private readonly JpegCompressStruct m_cinfo;

        private int m_cur_iMCU_row;    /* number of current iMCU row */
        private int m_rowgroup_ctr;    /* counts row groups received in iMCU row */
        private bool m_suspended;     /* remember if we suspended output */

        /* If using just a strip buffer, this points to the entire set of buffers
        * (we allocate one for each component).  In the full-image case, this
        * points to the currently accessible strips of the virtual arrays.
        */
        private readonly byte[][][] m_buffer = new byte[JpegConstants.MAX_COMPONENTS][][];

        public JpegCMainController(JpegCompressStruct cinfo)
        {
            m_cinfo = cinfo;

            /* Allocate a strip buffer for each component */
            for (var ci = 0; ci < cinfo.m_num_components; ci++)
            {
                var compptr = cinfo.Component_info[ci];
                m_buffer[ci] = JpegCommonStruct.AllocJpegSamples(
                    compptr.Width_in_blocks * compptr.DCT_h_scaled_size,
                    compptr.V_samp_factor * compptr.DCT_v_scaled_size);
            }
        }

        // Initialize for a processing pass.
#pragma warning disable IDE1006 // Naming Styles
        public void start_pass(JBufMode pass_mode)
#pragma warning restore IDE1006 // Naming Styles
        {
            /* Do nothing in raw-data mode. */
            if (m_cinfo.m_raw_data_in)
            {
                return;
            }

            m_cur_iMCU_row = 0; /* initialize counters */
            m_rowgroup_ctr = 0;
            m_suspended = false;

            if (pass_mode != JBufMode.PassThrough)
            {
                m_cinfo.ErrExit(JMessageCode.JERR_BAD_BUFFER_MODE);
            }
        }

        /// <summary>
        /// Process some data.
        /// This routine handles the simple pass-through mode,
        /// where we have only a strip buffer.
        /// </summary>
#pragma warning disable IDE1006 // Naming Styles
        public void process_data(byte[][] input_buf, ref int in_row_ctr, int in_rows_avail)
#pragma warning restore IDE1006 // Naming Styles
        {
            while (m_cur_iMCU_row < m_cinfo.m_total_iMCU_rows)
            {
                /* Read input data if we haven't filled the main buffer yet */
                if (m_rowgroup_ctr < m_cinfo.min_DCT_v_scaled_size)
                {
                    m_cinfo.m_prep.PreProcessData(input_buf, ref in_row_ctr, in_rows_avail, m_buffer,
                        ref m_rowgroup_ctr, m_cinfo.min_DCT_v_scaled_size);
                }

                /* If we don't have a full iMCU row buffered, return to application for
                 * more data.  Note that preprocessor will always pad to fill the iMCU row
                 * at the bottom of the image.
                 */
                if (m_rowgroup_ctr != m_cinfo.min_DCT_v_scaled_size)
                {
                    return;
                }

                /* Send the completed row to the compressor */
                if (!m_cinfo.m_coef.CompressData(m_buffer))
                {
                    /* If compressor did not consume the whole row, then we must need to
                     * suspend processing and return to the application.  In this situation
                     * we pretend we didn't yet consume the last input row; otherwise, if
                     * it happened to be the last row of the image, the application would
                     * think we were done.
                     */
                    if (!m_suspended)
                    {
                        in_row_ctr--;
                        m_suspended = true;
                    }

                    return;
                }

                /* We did finish the row.  Undo our little suspension hack if a previous
                 * call suspended; then mark the main buffer empty.
                 */
                if (m_suspended)
                {
                    in_row_ctr++;
                    m_suspended = false;
                }

                m_rowgroup_ctr = 0;
                m_cur_iMCU_row++;
            }
        }
    }
}
