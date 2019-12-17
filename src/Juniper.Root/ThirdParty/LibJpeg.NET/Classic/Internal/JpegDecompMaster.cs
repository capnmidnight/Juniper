/*
 * This file contains master control logic for the JPEG decompressor.
 * These routines are concerned with selecting the modules to be executed
 * and with determining the number of passes and the work to be done in each
 * pass.
 */

using System;

namespace BitMiracle.LibJpeg.Classic.Internal
{
    /// <summary>
    /// Master control module
    /// </summary>
    internal class JpegDecompMaster
    {
        private readonly JpegDecompressStruct m_cinfo;

        private int m_pass_number;        /* # of passes completed */
        private bool m_is_dummy_pass; /* True during 1st pass for 2-pass quant */

        private bool m_using_merged_upsample; /* true if using merged upsample/cconvert */

        /* Saved references to initialized quantizer modules,
        * in case we need to switch modes.
        */
        private IJpegColorQuantizer m_quantizer_1pass;
        private IJpegColorQuantizer m_quantizer_2pass;

        public JpegDecompMaster(JpegDecompressStruct cinfo)
        {
            m_cinfo = cinfo;
            MasterSelection();
        }

        /// <summary>
        /// Per-pass setup.
        /// This is called at the beginning of each output pass.  We determine which
        /// modules will be active during this pass and give them appropriate
        /// start_pass calls.  We also set is_dummy_pass to indicate whether this
        /// is a "real" output pass or a dummy pass for color quantization.
        /// (In the latter case, we will crank the pass to completion.)
        /// </summary>
        public void PrepareForOutputPass()
        {
            if (m_is_dummy_pass)
            {
                /* Final pass of 2-pass quantization */
                m_is_dummy_pass = false;
                m_cinfo.m_cquantize.StartPass(false);
                m_cinfo.m_post.StartPass(JBufMode.CrankDest);
                m_cinfo.m_main.StartPass(JBufMode.CrankDest);
            }
            else
            {
                if (m_cinfo.quantizeColors && m_cinfo.colormap == null)
                {
                    /* Select new quantization method */
                    if (m_cinfo.twoPassQuantize && m_cinfo.enable2PassQuant)
                    {
                        m_cinfo.m_cquantize = m_quantizer_2pass;
                        m_is_dummy_pass = true;
                    }
                    else if (m_cinfo.enable1PassQuant)
                    {
                        m_cinfo.m_cquantize = m_quantizer_1pass;
                    }
                    else
                    {
                        m_cinfo.ErrExit(JMessageCode.JERR_MODE_CHANGE);
                    }
                }

                m_cinfo.m_idct.StartPass();
                m_cinfo.m_coef.StartOutputPass();

                if (!m_cinfo.rawDataOut)
                {
                    m_cinfo.m_upsample.StartPass();

                    if (m_cinfo.quantizeColors)
                    {
                        m_cinfo.m_cquantize.StartPass(m_is_dummy_pass);
                    }

                    m_cinfo.m_post.StartPass(m_is_dummy_pass ? JBufMode.SaveAndPass : JBufMode.PassThrough);
                    m_cinfo.m_main.StartPass(JBufMode.PassThrough);
                }
            }

            /* Set up progress monitor's pass info if present */
            if (m_cinfo.progress != null)
            {
                m_cinfo.progress.CompletedPasses = m_pass_number;
                m_cinfo.progress.TotalPasses = m_pass_number + (m_is_dummy_pass ? 2 : 1);

                /* In buffered-image mode, we assume one more output pass if EOI not
                 * yet reached, but no more passes if EOI has been reached.
                 */
                if (m_cinfo.bufferedImage && !m_cinfo.m_inputctl.EOIReached())
                {
                    m_cinfo.progress.TotalPasses += (m_cinfo.enable2PassQuant ? 2 : 1);
                }
            }
        }

        /// <summary>
        /// Finish up at end of an output pass.
        /// </summary>
        public void FinishOutputPass()
        {
            if (m_cinfo.quantizeColors)
            {
                m_cinfo.m_cquantize.FinishPass();
            }

            m_pass_number++;
        }

        public bool IsDummyPass()
        {
            return m_is_dummy_pass;
        }

        /// <summary>
        /// <para>
        /// Master selection of decompression modules.
        /// This is done once at jpeg_start_decompress time.  We determine
        /// which modules will be used and give them appropriate initialization calls.
        /// We also initialize the decompressor input side to begin consuming data.
        /// </para>
        /// <para>
        /// Since jpeg_read_header has finished, we know what is in the SOF
        /// and (first) SOS markers.  We also have all the application parameter
        /// settings.
        /// </para>
        /// </summary>
        private void MasterSelection()
        {
            /* For now, precision must match compiled-in value... */
            if (m_cinfo.dataPrecision != JpegConstants.BITS_IN_JSAMPLE)
            {
                m_cinfo.ErrExit(JMessageCode.JERR_BAD_PRECISION, m_cinfo.dataPrecision);
            }

            /* Initialize dimensions and other stuff */
            m_cinfo.JpegCalcOutputDimensions();
            PrepareRangeLimitTable();

            /* Sanity check on image dimensions */
            if (m_cinfo.outputHeight <= 0 || m_cinfo.outputWidth <= 0 ||
                m_cinfo.outColorComponents <= 0)
            {
                m_cinfo.ErrExit(JMessageCode.JERR_EMPTY_IMAGE);
            }

            /* Width of an output scanline must be representable as int. */
            long samplesperrow = m_cinfo.outputWidth * m_cinfo.outColorComponents;
            var jd_samplesperrow = (int)samplesperrow;
            if (jd_samplesperrow != samplesperrow)
            {
                m_cinfo.ErrExit(JMessageCode.JERR_WIDTH_OVERFLOW);
            }

            /* Initialize my private state */
            m_pass_number = 0;
            m_using_merged_upsample = m_cinfo.UseMergedUpSample();

            /* Color quantizer selection */
            m_quantizer_1pass = null;
            m_quantizer_2pass = null;

            /* No mode changes if not using buffered-image mode. */
            if (!m_cinfo.quantizeColors || !m_cinfo.bufferedImage)
            {
                m_cinfo.enable1PassQuant = false;
                m_cinfo.enableExternalQuant = false;
                m_cinfo.enable2PassQuant = false;
            }

            if (m_cinfo.quantizeColors)
            {
                if (m_cinfo.rawDataOut)
                {
                    m_cinfo.ErrExit(JMessageCode.JERR_NOTIMPL);
                }

                /* 2-pass quantizer only works in 3-component color space. */
                if (m_cinfo.outColorComponents != 3)
                {
                    m_cinfo.enable1PassQuant = true;
                    m_cinfo.enableExternalQuant = false;
                    m_cinfo.enable2PassQuant = false;
                    m_cinfo.colormap = null;
                }
                else if (m_cinfo.colormap != null)
                {
                    m_cinfo.enableExternalQuant = true;
                }
                else if (m_cinfo.twoPassQuantize)
                {
                    m_cinfo.enable2PassQuant = true;
                }
                else
                {
                    m_cinfo.enable1PassQuant = true;
                }

                if (m_cinfo.enable1PassQuant)
                {
                    m_cinfo.m_cquantize = new My1PassCQuantizer(m_cinfo);
                    m_quantizer_1pass = m_cinfo.m_cquantize;
                }

                /* We use the 2-pass code to map to external colormaps. */
                if (m_cinfo.enable2PassQuant || m_cinfo.enableExternalQuant)
                {
                    m_cinfo.m_cquantize = new My2PassCQuantizer(m_cinfo);
                    m_quantizer_2pass = m_cinfo.m_cquantize;
                }
                /* If both quantizers are initialized, the 2-pass one is left active;
                 * this is necessary for starting with quantization to an external map.
                 */
            }

            /* Post-processing: in particular, color conversion first */
            if (!m_cinfo.rawDataOut)
            {
                if (m_using_merged_upsample)
                {
                    /* does color conversion too */
                    m_cinfo.m_upsample = new MyMergedUpSampler(m_cinfo);
                }
                else
                {
                    m_cinfo.m_cconvert = new JpegColorDeconverter(m_cinfo);
                    m_cinfo.m_upsample = new MyUpSampler(m_cinfo);
                }

                m_cinfo.m_post = new JpegDPostController(m_cinfo, m_cinfo.enable2PassQuant);
            }

            /* Inverse DCT */
            m_cinfo.m_idct = new JpegInverseDct(m_cinfo);

            if (m_cinfo.arithCode)
            {
                m_cinfo.m_entropy = new ArithEntropyDecoder(m_cinfo);
            }
            else
            {
                m_cinfo.m_entropy = new HuffmanEntropyDecoder(m_cinfo);
            }

            /* Initialize principal buffer controllers. */
            var use_c_buffer = m_cinfo.m_inputctl.HasMultipleScans() || m_cinfo.bufferedImage;
            m_cinfo.m_coef = new JpegDCoefController(m_cinfo, use_c_buffer);

            if (!m_cinfo.rawDataOut)
            {
                m_cinfo.m_main = new JpegDMainController(m_cinfo);
            }

            /* Initialize input side of decompressor to consume first scan. */
            m_cinfo.m_inputctl.StartInputPass();

            /* If jpeg_start_decompress will read the whole file, initialize
             * progress monitoring appropriately.  The input step is counted
             * as one pass.
             */
            if (m_cinfo.progress != null && !m_cinfo.bufferedImage && m_cinfo.m_inputctl.HasMultipleScans())
            {
                /* Estimate number of scans to set pass_limit. */
                int nscans;
                if (m_cinfo.progressiveMode)
                {
                    /* Arbitrarily estimate 2 interleaved DC scans + 3 AC scans/component. */
                    nscans = 2 + (3 * m_cinfo.numComponents);
                }
                else
                {
                    /* For a non progressive multiscan file, estimate 1 scan per component. */
                    nscans = m_cinfo.numComponents;
                }

                m_cinfo.progress.PassCounter = 0;
                m_cinfo.progress.PassLimit = m_cinfo.m_total_iMCU_rows * nscans;
                m_cinfo.progress.CompletedPasses = 0;
                m_cinfo.progress.TotalPasses = (m_cinfo.enable2PassQuant ? 3 : 2);

                /* Count the input pass as done */
                m_pass_number++;
            }
        }

        /// <summary>
        /// <para>Allocate and fill in the sample_range_limit table.</para>
        /// <para>
        /// Several decompression processes need to range-limit values to the range
        /// 0..MAXJSAMPLE; the input value may fall somewhat outside this range
        /// due to noise introduced by quantization, roundoff error, etc. These
        /// processes are inner loops and need to be as fast as possible. On most
        /// machines, particularly CPUs with pipelines or instruction prefetch,
        /// a (subscript-check-less) C table lookup
        ///     x = sample_range_limit[x];
        /// is faster than explicit tests
        /// <c>
        ///     if (x &amp; 0)
        ///        x = 0;
        ///     else if (x > MAXJSAMPLE)
        ///        x = MAXJSAMPLE;
        /// </c>
        /// These processes all use a common table prepared by the routine below.
        /// </para>
        /// <para>
        /// For most steps we can mathematically guarantee that the initial value
        /// of x is within 2*(MAXJSAMPLE+1) of the legal range, so a table running
        /// from -2*(MAXJSAMPLE+1) to 3*MAXJSAMPLE+2 is sufficient.But for the
        /// initial limiting step(just after the IDCT), a wildly out-of-range value
        /// is possible if the input data is corrupt.To avoid any chance of indexing
        /// off the end of memory and getting a bad-pointer trap, we perform the
        /// post-IDCT limiting thus:
        ///     <c>x = (sample_range_limit - SUBSET)[(x + CENTER) &amp; MASK];</c>
        /// where MASK is 2 bits wider than legal sample data, ie 10 bits for 8-bit
        /// samples.  Under normal circumstances this is more than enough range and
        /// a correct output will be generated; with bogus input data the mask will
        /// cause wraparound, and we will safely generate a bogus-but-in-range output.
        /// For the post-IDCT step, we want to convert the data from signed to unsigned
        /// representation by adding CENTERJSAMPLE at the same time that we limit it.
        /// This is accomplished with SUBSET = CENTER - CENTERJSAMPLE.
        /// </para>
        /// <para>
        /// Note that the table is allocated in near data space on PCs; it's small
        /// enough and used often enough to justify this.
        /// </para>
        /// </summary>
        private void PrepareRangeLimitTable()
        {
            var table = new byte[5 * (JpegConstants.MAXJSAMPLE + 1)];
            /* First segment of range limit table: limit[x] = 0 for x < 0 */

            /* allow negative subscripts of simple table */
            const int tableOffset = 2 * (JpegConstants.MAXJSAMPLE + 1);
            m_cinfo.m_sample_range_limit = table;
            m_cinfo.m_sampleRangeLimitOffset = tableOffset;

            /* Main part of range limit table: limit[x] = x */
            int i;
            for (i = 0; i <= JpegConstants.MAXJSAMPLE; i++)
            {
                table[tableOffset + i] = (byte)i;
            }

            /* End of range limit table: limit[x] = MAXJSAMPLE for x > MAXJSAMPLE */
            for (; i < 3 * (JpegConstants.MAXJSAMPLE + 1); i++)
            {
                table[tableOffset + i] = JpegConstants.MAXJSAMPLE;
            }
        }
    }
}
