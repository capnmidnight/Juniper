/*
 * This file contains the compression preprocessing controller.
 * This controller manages the color conversion, downsampling,
 * and edge expansion steps.
 *
 * Most of the complexity here is associated with buffering input rows
 * as required by the downsampler.  See the comments at the head of
 * my_downsampler for the downsampler's needs.
 */

using System;

namespace BitMiracle.LibJpeg.Classic.Internal
{
    /// <summary>
    /// <para>Compression preprocessing (downsampling input buffer control).</para>
    /// <para>
    /// For the simple (no-context-row) case, we just need to buffer one
    /// row group's worth of pixels for the downsampling step.  At the bottom of
    /// the image, we pad to a full row group by replicating the last pixel row.
    /// The downsampler's last output row is then replicated if needed to pad
    /// out to a full iMCU row.
    /// </para>
    /// <para>
    /// When providing context rows, we must buffer three row groups' worth of
    /// pixels.  Three row groups are physically allocated, but the row pointer
    /// arrays are made five row groups high, with the extra pointers above and
    /// below "wrapping around" to point to the last and first real row groups.
    /// This allows the downsampler to access the proper context rows.
    /// At the top and bottom of the image, we create dummy context rows by
    /// copying the first or last real pixel row.  This copying could be avoided
    /// by pointer hacking as is done in jdmainct.c, but it doesn't seem worth the
    /// trouble on the compression side.
    /// </para>
    /// </summary>
    internal class JpegCPrepController
    {
        private readonly jpeg_compress_struct cinfo;

        /* Downsampling input buffer.  This buffer holds color-converted data
        * until we have enough to do a downsample step.
        */
        private readonly byte[][][] colorBuf = new byte[JpegConstants.MAX_COMPONENTS][][];
        private int colorBufRowsOffset;

        private int rowsToGo;  /* counts rows remaining in source image */
        private int nextBufRow;       /* index of next row to store in color_buf */

        private int thisRowGroup;     /* starting row index of group to process */
        private int nextBufStop;      /* downsample when we reach this index */

        public JpegCPrepController(jpeg_compress_struct cinfo)
        {
            this.cinfo = cinfo;

            /* Allocate the color conversion buffer.
            * We make the buffer wide enough to allow the downsampler to edge-expand
            * horizontally within the buffer, if it so chooses.
            */
            if (cinfo.m_downsample.NeedContextRows())
            {
                /* Set up to provide context rows */
                create_context_buffer();
            }
            else
            {
                /* No context, just make it tall enough for one row group */
                for (var ci = 0; ci < cinfo.m_num_components; ci++)
                {
                    colorBufRowsOffset = 0;
                    colorBuf[ci] = jpeg_common_struct.AllocJpegSamples(
                        (cinfo.Component_info[ci].Width_in_blocks *
                        cinfo.min_DCT_h_scaled_size * cinfo.m_max_h_samp_factor) /
                        cinfo.Component_info[ci].H_samp_factor,
                        cinfo.m_max_v_samp_factor);
                }
            }
        }

        /// <summary>
        /// Initialize for a processing pass.
        /// </summary>
        public void StartPass(JBufMode pass_mode)
        {
            if (pass_mode != JBufMode.PassThrough)
            {
                cinfo.ERREXIT(J_MESSAGE_CODE.JERR_BAD_BUFFER_MODE);
            }

            /* Initialize total-height counter for detecting bottom of image */
            rowsToGo = cinfo.m_image_height;

            /* Mark the conversion buffer empty */
            nextBufRow = 0;

            /* Preset additional state variables for context mode.
             * These aren't used in non-context mode, so we needn't test which mode.
             */
            thisRowGroup = 0;

            /* Set next_buf_stop to stop after two row groups have been read in. */
            nextBufStop = 2 * cinfo.m_max_v_samp_factor;
        }

        public void PreProcessData(byte[][] input_buf, ref int in_row_ctr, int in_rows_avail, byte[][][] output_buf, ref int out_row_group_ctr, int out_row_groups_avail)
        {
            if (cinfo.m_downsample.NeedContextRows())
            {
                pre_process_context(input_buf, ref in_row_ctr, in_rows_avail, output_buf, ref out_row_group_ctr, out_row_groups_avail);
            }
            else
            {
                pre_process_WithoutContext(input_buf, ref in_row_ctr, in_rows_avail, output_buf, ref out_row_group_ctr, out_row_groups_avail);
            }
        }

        /// <summary>
        /// Create the wrapped-around downsampling input buffer needed for context mode.
        /// </summary>
        private void create_context_buffer()
        {
            var rgroup_height = cinfo.m_max_v_samp_factor;
            for (var ci = 0; ci < cinfo.m_num_components; ci++)
            {
                var samplesPerRow = (cinfo.Component_info[ci].Width_in_blocks *
                    cinfo.min_DCT_h_scaled_size * cinfo.m_max_h_samp_factor) /
                    cinfo.Component_info[ci].H_samp_factor;

                var fake_buffer = new byte[5 * rgroup_height][];
                for (var i = 1; i < 4 * rgroup_height; i++)
                {
                    fake_buffer[i] = new byte[samplesPerRow];
                }

                /* Allocate the actual buffer space (3 row groups) for this component.
                 * We make the buffer wide enough to allow the downsampler to edge-expand
                 * horizontally within the buffer, if it so chooses.
                 */
                var true_buffer = jpeg_common_struct.AllocJpegSamples(samplesPerRow, 3 * rgroup_height);

                /* Copy true buffer row pointers into the middle of the fake row array */
                for (var i = 0; i < 3 * rgroup_height; i++)
                {
                    fake_buffer[rgroup_height + i] = true_buffer[i];
                }

                /* Fill in the above and below wraparound pointers */
                for (var i = 0; i < rgroup_height; i++)
                {
                    fake_buffer[i] = true_buffer[2 * rgroup_height + i];
                    fake_buffer[4 * rgroup_height + i] = true_buffer[i];
                }

                colorBuf[ci] = fake_buffer;
                colorBufRowsOffset = rgroup_height;
            }
        }

        /// <summary>
        /// <para>Process some data in the simple no-context case.</para>
        /// <para>
        /// Preprocessor output data is counted in "row groups".  A row group
        /// is defined to be v_samp_factor sample rows of each component.
        /// Downsampling will produce this much data from each max_v_samp_factor
        /// input rows.
        /// </para>
        /// </summary>
        private void pre_process_WithoutContext(byte[][] input_buf, ref int in_row_ctr, int in_rows_avail, byte[][][] output_buf, ref int out_row_group_ctr, int out_row_groups_avail)
        {
            while (in_row_ctr < in_rows_avail && out_row_group_ctr < out_row_groups_avail)
            {
                /* Do color conversion to fill the conversion buffer. */
                var inrows = in_rows_avail - in_row_ctr;
                var numrows = cinfo.m_max_v_samp_factor - nextBufRow;
                numrows = Math.Min(numrows, inrows);
                cinfo.m_cconvert.color_convert(input_buf, in_row_ctr, colorBuf, colorBufRowsOffset + nextBufRow, numrows);
                in_row_ctr += numrows;
                nextBufRow += numrows;
                rowsToGo -= numrows;

                /* If at bottom of image, pad to fill the conversion buffer. */
                if (rowsToGo == 0 && nextBufRow < cinfo.m_max_v_samp_factor)
                {
                    for (var ci = 0; ci < cinfo.m_num_components; ci++)
                    {
                        expand_bottom_edge(colorBuf[ci], colorBufRowsOffset, cinfo.m_image_width, nextBufRow, cinfo.m_max_v_samp_factor);
                    }

                    nextBufRow = cinfo.m_max_v_samp_factor;
                }

                /* If we've filled the conversion buffer, empty it. */
                if (nextBufRow == cinfo.m_max_v_samp_factor)
                {
                    cinfo.m_downsample.Downsample(colorBuf, colorBufRowsOffset, output_buf, out_row_group_ctr);
                    nextBufRow = 0;
                    out_row_group_ctr++;
                }

                /* If at bottom of image, pad the output to a full iMCU height.
                 * Note we assume the caller is providing a one-iMCU-height output buffer!
                 */
                if (rowsToGo == 0 && out_row_group_ctr < out_row_groups_avail)
                {
                    for (var ci = 0; ci < cinfo.m_num_components; ci++)
                    {
                        var componentInfo = cinfo.Component_info[ci];
                        numrows = (componentInfo.V_samp_factor * componentInfo.DCT_v_scaled_size) /
                            cinfo.min_DCT_v_scaled_size;

                        expand_bottom_edge(output_buf[ci], 0,
                            componentInfo.Width_in_blocks * componentInfo.DCT_h_scaled_size,
                            out_row_group_ctr * numrows,
                            out_row_groups_avail * numrows);
                    }

                    out_row_group_ctr = out_row_groups_avail;
                    break;          /* can exit outer loop without test */
                }
            }
        }

        /// <summary>
        /// Process some data in the context case.
        /// </summary>
        private void pre_process_context(byte[][] input_buf, ref int in_row_ctr, int in_rows_avail, byte[][][] output_buf, ref int out_row_group_ctr, int out_row_groups_avail)
        {
            while (out_row_group_ctr < out_row_groups_avail)
            {
                if (in_row_ctr < in_rows_avail)
                {
                    /* Do color conversion to fill the conversion buffer. */
                    var inrows = in_rows_avail - in_row_ctr;
                    var numrows = nextBufStop - nextBufRow;
                    numrows = Math.Min(numrows, inrows);
                    cinfo.m_cconvert.color_convert(input_buf, in_row_ctr, colorBuf, colorBufRowsOffset + nextBufRow, numrows);

                    /* Pad at top of image, if first time through */
                    if (rowsToGo == cinfo.m_image_height)
                    {
                        for (var ci = 0; ci < cinfo.m_num_components; ci++)
                        {
                            for (var row = 1; row <= cinfo.m_max_v_samp_factor; row++)
                            {
                                JpegUtils.jcopy_sample_rows(colorBuf[ci], colorBufRowsOffset, colorBuf[ci], colorBufRowsOffset - row, 1, cinfo.m_image_width);
                            }
                        }
                    }

                    in_row_ctr += numrows;
                    nextBufRow += numrows;
                    rowsToGo -= numrows;
                }
                else
                {
                    /* Return for more data, unless we are at the bottom of the image. */
                    if (rowsToGo != 0)
                    {
                        break;
                    }

                    /* When at bottom of image, pad to fill the conversion buffer. */
                    if (nextBufRow < nextBufStop)
                    {
                        for (var ci = 0; ci < cinfo.m_num_components; ci++)
                        {
                            expand_bottom_edge(colorBuf[ci], colorBufRowsOffset, cinfo.m_image_width, nextBufRow, nextBufStop);
                        }

                        nextBufRow = nextBufStop;
                    }
                }

                /* If we've gotten enough data, downsample a row group. */
                if (nextBufRow == nextBufStop)
                {
                    cinfo.m_downsample.Downsample(colorBuf, colorBufRowsOffset + thisRowGroup, output_buf, out_row_group_ctr);
                    out_row_group_ctr++;

                    /* Advance pointers with wraparound as necessary. */
                    thisRowGroup += cinfo.m_max_v_samp_factor;
                    var buf_height = cinfo.m_max_v_samp_factor * 3;

                    if (thisRowGroup >= buf_height)
                    {
                        thisRowGroup = 0;
                    }

                    if (nextBufRow >= buf_height)
                    {
                        nextBufRow = 0;
                    }

                    nextBufStop = nextBufRow + cinfo.m_max_v_samp_factor;
                }
            }
        }

        /// <summary>
        /// Expand an image vertically from height input_rows to height output_rows,
        /// by duplicating the bottom row.
        /// </summary>
        private static void expand_bottom_edge(byte[][] image_data, int rowsOffset, int num_cols, int input_rows, int output_rows)
        {
            for (var row = input_rows; row < output_rows; row++)
            {
                JpegUtils.jcopy_sample_rows(image_data, rowsOffset + input_rows - 1, image_data, row, 1, num_cols);
            }
        }
    }
}
