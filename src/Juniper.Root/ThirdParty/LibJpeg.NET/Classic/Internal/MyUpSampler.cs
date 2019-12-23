/*
 * This file contains upsampling routines.
 *
 * Upsampling input data is counted in "row groups".  A row group
 * is defined to be (v_samp_factor * DCT_v_scaled_size / min_DCT_v_scaled_size)
 * sample rows of each component.  Upsampling will normally produce
 * max_v_samp_factor pixel rows from each row group (but this could vary
 * if the upsampler is applying a scale factor of its own).
 *
 * An excellent reference for image resampling is
 *   Digital Image Warping, George Wolberg, 1990.
 *   Pub. by IEEE Computer Society Press, Los Alamitos, CA. ISBN 0-8186-8944-7.
 */

namespace BitMiracle.LibJpeg.Classic.Internal
{
    internal class MyUpSampler : JpegUpSampler
    {
        private enum ComponentUpsampler
        {
            noop_upsampler,
            fullsize_upsampler,
            h2v1_upsampler,
            h2v2_upsampler,
            int_upsampler
        }

        private readonly JpegDecompressStruct m_cinfo;

        /* Color conversion buffer.  When using separate upsampling and color
        * conversion steps, this buffer holds one upsampled row group until it
        * has been color converted and output.
        * Note: we do not allocate any storage for component(s) which are full-size,
        * ie do not need rescaling.  The corresponding entry of color_buf[] is
        * simply set to point to the input data array, thereby avoiding copying.
        */
        private readonly ComponentBuffer[] m_color_buf = new ComponentBuffer[JpegConstants.MAX_COMPONENTS];

        // used only for fullsize_upsampler mode
        private readonly int[] m_perComponentOffsets = new int[JpegConstants.MAX_COMPONENTS];

        /* Per-component upsampling method pointers */
        private readonly ComponentUpsampler[] m_upsampleMethods = new ComponentUpsampler[JpegConstants.MAX_COMPONENTS];
        private int m_currentComponent; // component being upsampled
        private int m_upsampleRowOffset;

        private int m_next_row_out;       /* counts rows emitted from color_buf */
        private int m_rows_to_go;  /* counts rows remaining in image */

        /* Height of an input row group for each component. */
        private readonly int[] m_rowgroup_height = new int[JpegConstants.MAX_COMPONENTS];

        /* These arrays save pixel expansion factors so that int_expand need not
        * recompute them each time.  They are unused for other upsampling methods.
        */
        private readonly byte[] m_h_expand = new byte[JpegConstants.MAX_COMPONENTS];
        private readonly byte[] m_v_expand = new byte[JpegConstants.MAX_COMPONENTS];

        public MyUpSampler(JpegDecompressStruct cinfo)
        {
            m_cinfo = cinfo;
            m_need_context_rows = false; /* until we find out differently */

            if (cinfo.m_CCIR601_sampling)    /* this isn't supported */
            {
                cinfo.ErrExit(JMessageCode.JERR_CCIR601_NOTIMPL);
            }

            /* Verify we can handle the sampling factors, select per-component methods,
            * and create storage as needed.
            */
            for (var ci = 0; ci < cinfo.numComponents; ci++)
            {
                var componentInfo = cinfo.CompInfo[ci];

                /* Compute size of an "input group" after IDCT scaling.  This many samples
                * are to be converted to max_h_samp_factor * max_v_samp_factor pixels.
                */
                var h_in_group = (componentInfo.H_samp_factor * componentInfo.DCT_h_scaled_size) / cinfo.min_DCT_h_scaled_size;
                var v_in_group = (componentInfo.V_samp_factor * componentInfo.DCT_v_scaled_size) / cinfo.min_DCT_v_scaled_size;
                var h_out_group = cinfo.m_max_h_samp_factor;
                var v_out_group = cinfo.m_maxVSampleFactor;

                /* save for use later */
                m_rowgroup_height[ci] = v_in_group;

                if (!componentInfo.component_needed)
                {
                    /* Don't bother to upsample an uninteresting component. */
                    m_upsampleMethods[ci] = ComponentUpsampler.noop_upsampler;
                    continue;		/* don't need to allocate buffer */
                }

                if (h_in_group == h_out_group && v_in_group == v_out_group)
                {
                    /* Fullsize components can be processed without any work. */
                    m_upsampleMethods[ci] = ComponentUpsampler.fullsize_upsampler;
                    continue;		/* don't need to allocate buffer */
                }

                if (h_in_group * 2 == h_out_group && v_in_group == v_out_group)
                {
                    /* Special case for 2h1v upsampling */
                    m_upsampleMethods[ci] = ComponentUpsampler.h2v1_upsampler;
                }
                else if (h_in_group * 2 == h_out_group && v_in_group * 2 == v_out_group)
                {
                    /* Special case for 2h2v upsampling */
                    m_upsampleMethods[ci] = ComponentUpsampler.h2v2_upsampler;
                }
                else if ((h_out_group % h_in_group) == 0 && (v_out_group % v_in_group) == 0)
                {
                    /* Generic integral-factors upsampling method */
                    m_upsampleMethods[ci] = ComponentUpsampler.int_upsampler;
                    m_h_expand[ci] = (byte)(h_out_group / h_in_group);
                    m_v_expand[ci] = (byte)(v_out_group / v_in_group);
                }
                else
                {
                    cinfo.ErrExit(JMessageCode.JERR_FRACT_SAMPLE_NOTIMPL);
                }

                var cb = new ComponentBuffer();
                cb.SetBuffer(JpegCommonStruct.AllocJpegSamples(JpegUtils.jround_up(cinfo.outputWidth,
                    cinfo.m_max_h_samp_factor), cinfo.m_maxVSampleFactor), null, 0);

                m_color_buf[ci] = cb;
            }
        }

        /// <summary>
        /// Initialize for an upsampling pass.
        /// </summary>
        public override void StartPass()
        {
            /* Mark the conversion buffer empty */
            m_next_row_out = m_cinfo.m_maxVSampleFactor;

            /* Initialize total-height counter for detecting bottom of image */
            m_rows_to_go = m_cinfo.outputHeight;
        }

        /// <summary>
        /// <para>Control routine to do upsampling (and color conversion).</para>
        /// <para>
        /// In this version we upsample each component independently.
        /// We upsample one row group into the conversion buffer, then apply
        /// color conversion a row at a time.
        /// </para>
        /// </summary>
        public override void UpSample(ComponentBuffer[] input_buf, ref int in_row_group_ctr, int in_row_groups_avail, byte[][] output_buf, ref int out_row_ctr, int out_rows_avail)
        {
            /* Fill the conversion buffer, if it's empty */
            if (m_next_row_out >= m_cinfo.m_maxVSampleFactor)
            {
                for (var ci = 0; ci < m_cinfo.numComponents; ci++)
                {
                    m_perComponentOffsets[ci] = 0;

                    /* Invoke per-component upsample method.*/
                    m_currentComponent = ci;
                    m_upsampleRowOffset = in_row_group_ctr * m_rowgroup_height[ci];
                    UpSampleComponent(ref input_buf[ci]);
                }

                m_next_row_out = 0;
            }

            /* Color-convert and emit rows */

            /* How many we have in the buffer: */
            var num_rows = m_cinfo.m_maxVSampleFactor - m_next_row_out;

            /* Not more than the distance to the end of the image.  Need this test
             * in case the image height is not a multiple of max_v_samp_factor:
             */
            if (num_rows > m_rows_to_go)
            {
                num_rows = m_rows_to_go;
            }

            /* And not more than what the client can accept: */
            out_rows_avail -= out_row_ctr;
            if (num_rows > out_rows_avail)
            {
                num_rows = out_rows_avail;
            }

            m_cinfo.m_cconvert.ColorConvert(m_color_buf, m_perComponentOffsets, m_next_row_out, output_buf, out_row_ctr, num_rows);

            /* Adjust counts */
            out_row_ctr += num_rows;
            m_rows_to_go -= num_rows;
            m_next_row_out += num_rows;

            /* When the buffer is emptied, declare this input row group consumed */
            if (m_next_row_out >= m_cinfo.m_maxVSampleFactor)
            {
                in_row_group_ctr++;
            }
        }

        private void UpSampleComponent(ref ComponentBuffer input_data)
        {
            switch (m_upsampleMethods[m_currentComponent])
            {
                case ComponentUpsampler.noop_upsampler:
                NoOpUpSample();
                break;
                case ComponentUpsampler.fullsize_upsampler:
                FullSizeUpSample(ref input_data);
                break;
                case ComponentUpsampler.h2v1_upsampler:
                H2V1UpSample(ref input_data);
                break;
                case ComponentUpsampler.h2v2_upsampler:
                H2V2UpSample(ref input_data);
                break;
                case ComponentUpsampler.int_upsampler:
                IntUpSample(ref input_data);
                break;
                default:
                m_cinfo.ErrExit(JMessageCode.JERR_NOTIMPL);
                break;
            }
        }

        /*
         * These are the routines invoked to upsample pixel values
         * of a single component.  One row group is processed per call.
         */

        /// <summary>
        /// This is a no-op version used for "uninteresting" components.
        /// These components will not be referenced by color conversion.
        /// </summary>
        private static void NoOpUpSample()
        {
            // do nothing
        }

        /// <summary>
        /// For full-size components, we just make color_buf[ci] point at the
        /// input buffer, and thus avoid copying any data.  Note that this is
        /// safe only because sep_upsample doesn't declare the input row group
        /// "consumed" until we are done color converting and emitting it.
        /// </summary>
        private void FullSizeUpSample(ref ComponentBuffer input_data)
        {
            m_color_buf[m_currentComponent] = input_data;
            m_perComponentOffsets[m_currentComponent] = m_upsampleRowOffset;
        }

        /// <summary>
        /// Fast processing for the common case of 2:1 horizontal and 1:1 vertical.
        /// It's still a box filter.
        /// </summary>
        private void H2V1UpSample(ref ComponentBuffer input_data)
        {
            var output_data = m_color_buf[m_currentComponent];

            for (var inrow = 0; inrow < m_cinfo.m_maxVSampleFactor; inrow++)
            {
                var row = m_upsampleRowOffset + inrow;
                var outIndex = 0;

                for (var col = 0; outIndex < m_cinfo.outputWidth; col++)
                {
                    var invalue = input_data[row][col]; /* don't need GETJSAMPLE() here */
                    output_data[inrow][outIndex] = invalue;
                    outIndex++;
                    output_data[inrow][outIndex] = invalue;
                    outIndex++;
                }
            }
        }

        /// <summary>
        /// Fast processing for the common case of 2:1 horizontal and 2:1 vertical.
        /// It's still a box filter.
        /// </summary>
        private void H2V2UpSample(ref ComponentBuffer input_data)
        {
            var output_data = m_color_buf[m_currentComponent];

            var inrow = 0;
            var outrow = 0;
            while (outrow < m_cinfo.m_maxVSampleFactor)
            {
                var row = m_upsampleRowOffset + inrow;
                var outIndex = 0;

                for (var col = 0; outIndex < m_cinfo.outputWidth; col++)
                {
                    var invalue = input_data[row][col]; /* don't need GETJSAMPLE() here */
                    output_data[outrow][outIndex] = invalue;
                    outIndex++;
                    output_data[outrow][outIndex] = invalue;
                    outIndex++;
                }

                JpegUtils.jcopy_sample_rows(output_data, outrow, output_data, outrow + 1, 1, m_cinfo.outputWidth);
                inrow++;
                outrow += 2;
            }
        }

        /// <summary>
        /// This version handles any integral sampling ratios.
        /// This is not used for typical JPEG files, so it need not be fast.
        /// Nor, for that matter, is it particularly accurate: the algorithm is
        /// simple replication of the input pixel onto the corresponding output
        /// pixels.  The hi-falutin sampling literature refers to this as a
        /// "box filter".  A box filter tends to introduce visible artifacts,
        /// so if you are actually going to use 3:1 or 4:1 sampling ratios
        /// you would be well advised to improve this code.
        /// </summary>
        private void IntUpSample(ref ComponentBuffer input_data)
        {
            var output_data = m_color_buf[m_currentComponent];
            int h_expand = m_h_expand[m_currentComponent];
            int v_expand = m_v_expand[m_currentComponent];

            var inrow = 0;
            var outrow = 0;
            while (outrow < m_cinfo.m_maxVSampleFactor)
            {
                /* Generate one output row with proper horizontal expansion */
                var row = m_upsampleRowOffset + inrow;
                for (var col = 0; col < m_cinfo.outputWidth; col++)
                {
                    var invalue = input_data[row][col]; /* don't need GETJSAMPLE() here */
                    var outIndex = 0;
                    for (var h = h_expand; h > 0; h--)
                    {
                        output_data[outrow][outIndex] = invalue;
                        outIndex++;
                    }
                }

                /* Generate any additional output rows by duplicating the first one */
                if (v_expand > 1)
                {
                    JpegUtils.jcopy_sample_rows(output_data, outrow, output_data,
                        outrow + 1, v_expand - 1, m_cinfo.outputWidth);
                }

                inrow++;
                outrow += v_expand;
            }
        }
    }
}
