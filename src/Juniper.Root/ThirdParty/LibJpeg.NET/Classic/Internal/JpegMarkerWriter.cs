/*
 * This file contains routines to write JPEG datastream markers.
 */

namespace BitMiracle.LibJpeg.Classic.Internal
{
    /// <summary>
    /// Marker writing
    /// </summary>
    internal class JpegMarkerWriter
    {
        private readonly JpegCompressStruct m_cinfo;
        private int m_last_restart_interval; /* last DRI value emitted; 0 after SOI */

        public JpegMarkerWriter(JpegCompressStruct cinfo)
        {
            m_cinfo = cinfo;
        }

        /// <summary>
        /// Write datastream header.
        /// This consists of an SOI and optional APPn markers.
        /// We recommend use of the JFIF marker, but not the Adobe marker,
        /// when using YCbCr or grayscale data. The JFIF marker is also used
        /// for other standard JPEG colorspaces. The Adobe marker is helpful
        /// to distinguish RGB, CMYK, and YCCK colorspaces.
        /// Note that an application can write additional header markers after
        /// jpeg_start_compress returns.
        /// </summary>
        public void WriteFileHeader()
        {
            EmitMarker(JpegMarker.SOI);  /* first the SOI */

            /* SOI is defined to reset restart interval to 0 */
            m_last_restart_interval = 0;

            if (m_cinfo.m_write_JFIF_header)   /* next an optional JFIF APP0 */
            {
                EmitJfifApp0();
            }

            if (m_cinfo.m_write_Adobe_marker) /* next an optional Adobe APP14 */
            {
                EmitAdobeApp14();
            }
        }

        /// <summary>
        /// Write frame header.
        /// This consists of DQT and SOFn markers,
        /// a conditional LSE marker and a conditional pseudo SOS marker.
        /// Note that we do not emit the SOF until we have emitted the DQT(s).
        /// This avoids compatibility problems with incorrect implementations that
        /// try to error-check the quant table numbers as soon as they see the SOF.
        /// </summary>
        public void WriteFrameHeader()
        {
            /* Emit DQT for each quantization table.
             * Note that emit_dqt() suppresses any duplicate tables.
             */
            var prec = 0;
            for (var ci = 0; ci < m_cinfo.m_num_components; ci++)
            {
                prec += EmitDQT(m_cinfo.Component_info[ci].Quant_tbl_no);
            }

            /* now prec is nonzero iff there are any 16-bit quant tables. */

            /* Check for a non-baseline specification.
             * Note we assume that Huffman table numbers won't be changed later.
             */
            bool is_baseline;
            if (m_cinfo.arith_code
                || m_cinfo.m_progressive_mode
                || m_cinfo.m_data_precision != 8
                || m_cinfo.block_size != JpegConstants.DCTSIZE)
            {
                is_baseline = false;
            }
            else
            {
                is_baseline = true;
                for (var ci = 0; ci < m_cinfo.m_num_components; ci++)
                {
                    if (m_cinfo.Component_info[ci].Dc_tbl_no > 1 || m_cinfo.Component_info[ci].Ac_tbl_no > 1)
                    {
                        is_baseline = false;
                    }
                }

                if (prec != 0 && is_baseline)
                {
                    is_baseline = false;
                    /* If it's baseline except for quantizer size, warn the user */
                    m_cinfo.TraceMS(0, JMessageCode.JTRC_16BIT_TABLES);
                }
            }

            /* Emit the proper SOF marker */
            if (m_cinfo.arith_code)
            {
                if (m_cinfo.m_progressive_mode)
                {
                    EmitSOF(JpegMarker.SOF10); /* SOF code for progressive arithmetic */
                }
                else
                {
                    EmitSOF(JpegMarker.SOF9);  /* SOF code for sequential arithmetic */
                }
            }
            else if (m_cinfo.m_progressive_mode)
            {
                EmitSOF(JpegMarker.SOF2);    /* SOF code for progressive Huffman */
            }
            else if (is_baseline)
            {
                EmitSOF(JpegMarker.SOF0);    /* SOF code for baseline implementation */
            }
            else
            {
                EmitSOF(JpegMarker.SOF1);    /* SOF code for non-baseline Huffman file */
            }

            /* Check to emit LSE inverse color transform specification marker */
            if (m_cinfo.color_transform != JColorTransform.JCT_NONE)
            {
                EmitLseIct();
            }

            /* Check to emit pseudo SOS marker */
            if (m_cinfo.m_progressive_mode && m_cinfo.block_size != JpegConstants.DCTSIZE)
            {
                EmitPseudoSOS();
            }
        }

        /// <summary>
        /// Write scan header.
        /// This consists of DHT or DAC markers, optional DRI, and SOS.
        /// Compressed data will be written following the SOS.
        /// </summary>
        public void WriteScanHeader()
        {
            if (m_cinfo.arith_code)
            {
                /* Emit arith conditioning info.  We may have some duplication
                 * if the file has multiple scans, but it's so small it's hardly
                 * worth worrying about.
                 */
                EmitDAC();
            }
            else
            {
                /* Emit Huffman tables.
                 * Note that emit_dht() suppresses any duplicate tables.
                 */
                for (var i = 0; i < m_cinfo.m_comps_in_scan; i++)
                {
                    var compptr = m_cinfo.Component_info[m_cinfo.m_cur_comp_info[i]];

                    /* DC needs no table for refinement scan */
                    if (m_cinfo.m_Ss == 0 && m_cinfo.m_Ah == 0)
                    {
                        EmitDHT(compptr.Dc_tbl_no, false);
                    }

                    /* AC needs no table when not present */
                    if (m_cinfo.m_Se != 0)
                    {
                        EmitDHT(compptr.Ac_tbl_no, true);
                    }
                }

                /* Emit DRI if required --- note that DRI value could change for each scan.
                 * We avoid wasting space with unnecessary DRIs, however.
                 */
                if (m_cinfo.m_restart_interval != m_last_restart_interval)
                {
                    EmitDRI();
                    m_last_restart_interval = m_cinfo.m_restart_interval;
                }

                EmitSOS();
            }
        }

        /// <summary>
        /// Write datastream trailer.
        /// </summary>
        public void WriteFileTrailer()
        {
            EmitMarker(JpegMarker.EOI);
        }

        /// <summary>
        /// Write an abbreviated table-specification datastream.
        /// This consists of SOI, DQT and DHT tables, and EOI.
        /// Any table that is defined and not marked sent_table = true will be
        /// emitted.  Note that all tables will be marked sent_table = true at exit.
        /// </summary>
        public void WriteTablesOnly()
        {
            EmitMarker(JpegMarker.SOI);

            for (var i = 0; i < JpegConstants.NUM_QUANT_TBLS; i++)
            {
                if (m_cinfo.m_quant_tbl_ptrs[i] is object)
                {
                    EmitDQT(i);
                }
            }

            for (var i = 0; i < JpegConstants.NUM_HUFF_TBLS; i++)
            {
                if (m_cinfo.m_dc_huff_tbl_ptrs[i] is object)
                {
                    EmitDHT(i, false);
                }

                if (m_cinfo.m_ac_huff_tbl_ptrs[i] is object)
                {
                    EmitDHT(i, true);
                }
            }

            EmitMarker(JpegMarker.EOI);
        }

        //////////////////////////////////////////////////////////////////////////
        // These routines allow writing an arbitrary marker with parameters.
        // The only intended use is to emit COM or APPn markers after calling
        // write_file_header and before calling write_frame_header.
        // Other uses are not guaranteed to produce desirable results.
        // Counting the parameter bytes properly is the caller's responsibility.

        /// <summary>
        /// Emit an arbitrary marker header
        /// </summary>
        public void WriteMarkerHeader(int marker, int datalen)
        {
            if (datalen > 65533)     /* safety check */
            {
                m_cinfo.ErrExit(JMessageCode.JERR_BAD_LENGTH);
            }

            EmitMarker((JpegMarker)marker);

            Emit2Bytes(datalen + 2);    /* total length */
        }

        /// <summary>
        /// Emit one byte of marker parameters following write_marker_header
        /// </summary>
        public void WriteMarkerByte(byte val)
        {
            EmitByte(val);
        }

        //////////////////////////////////////////////////////////////////////////
        // Routines to write specific marker types.
        //

        /// <summary>
        /// Emit a SOS marker
        /// </summary>
        private void EmitSOS()
        {
            EmitMarker(JpegMarker.SOS);

            Emit2Bytes((2 * m_cinfo.m_comps_in_scan) + 2 + 1 + 3); /* length */

            EmitByte(m_cinfo.m_comps_in_scan);

            for (var i = 0; i < m_cinfo.m_comps_in_scan; i++)
            {
                var componentIndex = m_cinfo.m_cur_comp_info[i];
                var compptr = m_cinfo.Component_info[componentIndex];
                EmitByte(compptr.Component_id);

                /* We emit 0 for unused field(s); this is recommended by the P&M text
                 * but does not seem to be specified in the standard.
                 */

                /* DC needs no table for refinement scan */
                var td = (m_cinfo.m_Ss == 0 && m_cinfo.m_Ah == 0) ? compptr.Dc_tbl_no : 0;

                /* AC needs no table when not present */
                var ta = (m_cinfo.m_Se != 0) ? compptr.Ac_tbl_no : 0;

                EmitByte((td << 4) + ta);
            }

            EmitByte(m_cinfo.m_Ss);
            EmitByte(m_cinfo.m_Se);
            EmitByte((m_cinfo.m_Ah << 4) + m_cinfo.m_Al);
        }

        /// <summary>
        /// Emit an LSE inverse color transform specification marker
        /// </summary>
        private void EmitLseIct()
        {
            /* Support only 1 transform */
            if (m_cinfo.color_transform != JColorTransform.JCT_SUBTRACT_GREEN || m_cinfo.Num_components < 3)
            {
                m_cinfo.ErrExit(JMessageCode.JERR_CONVERSION_NOTIMPL);
            }

            EmitMarker(JpegMarker.JPG8);

            Emit2Bytes(24); /* fixed length */

            EmitByte(0x0D); /* ID inverse transform specification */
            Emit2Bytes(JpegConstants.MAXJSAMPLE); /* MAXTRANS */
            EmitByte(3);        /* Nt=3 */
            EmitByte(m_cinfo.Component_info[1].Component_id);
            EmitByte(m_cinfo.Component_info[0].Component_id);
            EmitByte(m_cinfo.Component_info[2].Component_id);
            EmitByte(0x80); /* F1: CENTER1=1, NORM1=0 */
            Emit2Bytes(0);  /* A(1,1)=0 */
            Emit2Bytes(0);  /* A(1,2)=0 */
            EmitByte(0);    /* F2: CENTER2=0, NORM2=0 */
            Emit2Bytes(1);  /* A(2,1)=1 */
            Emit2Bytes(0);  /* A(2,2)=0 */
            EmitByte(0);    /* F3: CENTER3=0, NORM3=0 */
            Emit2Bytes(1);  /* A(3,1)=1 */
            Emit2Bytes(0);  /* A(3,2)=0 */
        }

        /// <summary>
        /// Emit a SOF marker
        /// </summary>
        private void EmitSOF(JpegMarker code)
        {
            EmitMarker(code);

            Emit2Bytes((3 * m_cinfo.m_num_components) + 2 + 5 + 1); /* length */

            /* Make sure image isn't bigger than SOF field can handle */
            if (m_cinfo.jpeg_height > 65535 || m_cinfo.jpeg_width > 65535)
            {
                m_cinfo.ErrExit(JMessageCode.JERR_IMAGE_TOO_BIG, 65535);
            }

            EmitByte(m_cinfo.m_data_precision);
            Emit2Bytes(m_cinfo.jpeg_height);
            Emit2Bytes(m_cinfo.jpeg_width);

            EmitByte(m_cinfo.m_num_components);

            for (var ci = 0; ci < m_cinfo.m_num_components; ci++)
            {
                var componentInfo = m_cinfo.Component_info[ci];
                EmitByte(componentInfo.Component_id);
                EmitByte((componentInfo.H_samp_factor << 4) + componentInfo.V_samp_factor);
                EmitByte(componentInfo.Quant_tbl_no);
            }
        }

        /// <summary>
        /// Emit an Adobe APP14 marker
        /// </summary>
        private void EmitAdobeApp14()
        {
            /*
             * Length of APP14 block    (2 bytes)
             * Block ID         (5 bytes - ASCII "Adobe")
             * Version Number       (2 bytes - currently 100)
             * Flags0           (2 bytes - currently 0)
             * Flags1           (2 bytes - currently 0)
             * Color transform      (1 byte)
             *
             * Although Adobe TN 5116 mentions Version = 101, all the Adobe files
             * now in circulation seem to use Version = 100, so that's what we write.
             *
             * We write the color transform byte as 1 if the JPEG color space is
             * YCbCr, 2 if it's YCCK, 0 otherwise.  Adobe's definition has to do with
             * whether the encoder performed a transformation, which is pretty useless.
             */

            EmitMarker(JpegMarker.APP14);

            Emit2Bytes(2 + 5 + 2 + 2 + 2 + 1); /* length */

            EmitByte(0x41); /* Identifier: ASCII "Adobe" */
            EmitByte(0x64);
            EmitByte(0x6F);
            EmitByte(0x62);
            EmitByte(0x65);
            Emit2Bytes(100);    /* Version */
            Emit2Bytes(0);  /* Flags0 */
            Emit2Bytes(0);  /* Flags1 */
            switch (m_cinfo.m_jpeg_color_space)
            {
                case JColorSpace.JCS_YCbCr:
                EmitByte(1);    /* Color transform = 1 */
                break;
                case JColorSpace.JCS_YCCK:
                EmitByte(2);    /* Color transform = 2 */
                break;
                default:
                EmitByte(0);    /* Color transform = 0 */
                break;
            }
        }

        /// <summary>
        /// Emit a DRI marker
        /// </summary>
        private void EmitDRI()
        {
            EmitMarker(JpegMarker.DRI);

            Emit2Bytes(4);  /* fixed length */

            Emit2Bytes(m_cinfo.m_restart_interval);
        }

        /// <summary>
        /// Emit a DHT marker
        /// </summary>
        private void EmitDHT(int index, bool is_ac)
        {
            var htbl = m_cinfo.m_dc_huff_tbl_ptrs[index];
            if (is_ac)
            {
                htbl = m_cinfo.m_ac_huff_tbl_ptrs[index];
                index += 0x10; /* output index has AC bit set */
            }

            if (htbl is null)
            {
                m_cinfo.ErrExit(JMessageCode.JERR_NO_HUFF_TABLE, index);
            }

            if (!htbl.SentTable)
            {
                EmitMarker(JpegMarker.DHT);

                var length = 0;
                for (var i = 1; i <= 16; i++)
                {
                    length += htbl.Bits[i];
                }

                Emit2Bytes(length + 2 + 1 + 16);
                EmitByte(index);

                for (var i = 1; i <= 16; i++)
                {
                    EmitByte(htbl.Bits[i]);
                }

                for (var i = 0; i < length; i++)
                {
                    EmitByte(htbl.Huffval[i]);
                }

                htbl.SentTable = true;
            }
        }

        /// <summary>
        /// Emit a DQT marker
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>the precision used (0 = 8bits, 1 = 16bits) for baseline checking</returns>
        private int EmitDQT(int index)
        {
            var qtbl = m_cinfo.m_quant_tbl_ptrs[index];
            if (qtbl is null)
            {
                m_cinfo.ErrExit(JMessageCode.JERR_NO_QUANT_TABLE, index);
            }

            var prec = 0;
            for (var i = 0; i <= m_cinfo.lim_Se; i++)
            {
                if (qtbl.quantBal[m_cinfo.natural_order[i]] > 255)
                {
                    prec = 1;
                }
            }

            if (!qtbl.SentTable)
            {
                EmitMarker(JpegMarker.DQT);

                Emit2Bytes((prec != 0)
                    ? (m_cinfo.lim_Se * 2) + 2 + 1 + 2
                    : m_cinfo.lim_Se + 1 + 1 + 2);

                EmitByte(index + (prec << 4));

                for (var i = 0; i <= m_cinfo.lim_Se; i++)
                {
                    /* The table entries must be emitted in zigzag order. */
                    int qval = qtbl.quantBal[m_cinfo.natural_order[i]];

                    if (prec != 0)
                    {
                        EmitByte(qval >> 8);
                    }

                    EmitByte(qval & 0xFF);
                }

                qtbl.SentTable = true;
            }

            return prec;
        }

        /* Emit a DAC marker */
        /* Since the useful info is so small, we want to emit all the tables in */
        /* one DAC marker.  Therefore this routine does its own scan of the table. */
        private void EmitDAC()
        {
            var dc_in_use = new byte[JpegConstants.NUM_ARITH_TBLS];
            var ac_in_use = new byte[JpegConstants.NUM_ARITH_TBLS];

            for (var i = 0; i < m_cinfo.m_comps_in_scan; i++)
            {
                var compptr = m_cinfo.Component_info[m_cinfo.m_cur_comp_info[i]];
                /* DC needs no table for refinement scan */
                if (m_cinfo.m_Ss == 0 && m_cinfo.m_Ah == 0)
                {
                    dc_in_use[compptr.Dc_tbl_no] = 1;
                }

                /* AC needs no table when not present */
                if (m_cinfo.m_Se != 0)
                {
                    ac_in_use[compptr.Ac_tbl_no] = 1;
                }
            }

            var length = 0;
            for (var i = 0; i < JpegConstants.NUM_ARITH_TBLS; i++)
            {
                length += dc_in_use[i] + ac_in_use[i];
            }

            if (length != 0)
            {
                EmitMarker(JpegMarker.DAC);

                Emit2Bytes((length * 2) + 2);

                for (var i = 0; i < JpegConstants.NUM_ARITH_TBLS; i++)
                {
                    if (dc_in_use[i] != 0)
                    {
                        EmitByte(i);
                        EmitByte(m_cinfo.arith_dc_L[i] + (m_cinfo.arith_dc_U[i] << 4));
                    }

                    if (ac_in_use[i] != 0)
                    {
                        EmitByte(i + 0x10);
                        EmitByte(m_cinfo.arith_ac_K[i]);
                    }
                }
            }
        }

        /// <summary>
        /// Emit a pseudo SOS marker
        /// </summary>
        private void EmitPseudoSOS()
        {
            EmitMarker(JpegMarker.SOS);

            Emit2Bytes(2 + 1 + 3); /* length */

            EmitByte(0); /* Ns */

            EmitByte(0); /* Ss */
            EmitByte((m_cinfo.block_size * m_cinfo.block_size) - 1); /* Se */
            EmitByte(0); /* Ah/Al */
        }

        /// <summary>
        /// Emit a JFIF-compliant APP0 marker
        /// </summary>
        private void EmitJfifApp0()
        {
            /*
             * Length of APP0 block (2 bytes)
             * Block ID         (4 bytes - ASCII "JFIF")
             * Zero byte            (1 byte to terminate the ID string)
             * Version Major, Minor (2 bytes - major first)
             * Units            (1 byte - 0x00 = none, 0x01 = inch, 0x02 = cm)
             * Xdpu         (2 bytes - dots per unit horizontal)
             * Ydpu         (2 bytes - dots per unit vertical)
             * Thumbnail X size     (1 byte)
             * Thumbnail Y size     (1 byte)
             */

            EmitMarker(JpegMarker.APP0);

            Emit2Bytes(2 + 4 + 1 + 2 + 1 + 2 + 2 + 1 + 1); /* length */

            EmitByte(0x4A); /* Identifier: ASCII "JFIF" */
            EmitByte(0x46);
            EmitByte(0x49);
            EmitByte(0x46);
            EmitByte(0);
            EmitByte(m_cinfo.m_JFIF_major_version); /* Version fields */
            EmitByte(m_cinfo.m_JFIF_minor_version);
            EmitByte((int)m_cinfo.m_density_unit); /* Pixel size information */
            Emit2Bytes(m_cinfo.m_X_density);
            Emit2Bytes(m_cinfo.m_Y_density);
            EmitByte(0);        /* No thumbnail image */
            EmitByte(0);
        }

        //////////////////////////////////////////////////////////////////////////
        // Basic output routines.
        //
        // Note that we do not support suspension while writing a marker.
        // Therefore, an application using suspension must ensure that there is
        // enough buffer space for the initial markers (typ. 600-700 bytes) before
        // calling jpeg_start_compress, and enough space to write the trailing EOI
        // (a few bytes) before calling jpeg_finish_compress.  Multipass compression
        // modes are not supported at all with suspension, so those two are the only
        // points where markers will be written.

        /// <summary>
        /// Emit a marker code
        /// </summary>
        private void EmitMarker(JpegMarker mark)
        {
            EmitByte(0xFF);
            EmitByte((int)mark);
        }

        /// <summary>
        /// Emit a 2-byte integer; these are always MSB first in JPEG files
        /// </summary>
        private void Emit2Bytes(int value)
        {
            EmitByte((value >> 8) & 0xFF);
            EmitByte(value & 0xFF);
        }

        /// <summary>
        /// Emit a byte
        /// </summary>
        private void EmitByte(int val)
        {
            if (!m_cinfo.m_dest.EmitByte(val))
            {
                m_cinfo.ErrExit(JMessageCode.JERR_CANT_SUSPEND);
            }
        }
    }
}
