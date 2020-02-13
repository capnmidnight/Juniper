/*
 * This file contains Huffman entropy encoding routines.
 *
 * Much of the complexity here has to do with supporting output suspension.
 * If the data destination module demands suspension, we want to be able to
 * back up to the start of the current MCU.  To do this, we copy state
 * variables into local working storage, and update them back to the
 * permanent JPEG objects only upon successful completion of an MCU.
 *
 * We do not support output suspension for the progressive JPEG mode, since
 * the library currently does not allow multiple-scan files to be written
 * with output suspension.
 */

using System;

namespace BitMiracle.LibJpeg.Classic.Internal
{
    /// <summary>
    /// Expanded entropy encoder object for Huffman encoding.
    /// </summary>
    internal class HuffmanEntropyEncoder : JpegEntropyEncoder
    {
        /* The legal range of a DCT coefficient is
         *  -1024 .. +1023  for 8-bit data;
         * -16384 .. +16383 for 12-bit data.
         * Hence the magnitude should always fit in 10 or 14 bits respectively.
         */
        private const int MAX_COEF_BITS = 10;

        /* MAX_CORR_BITS is the number of bits the AC refinement correction-bit
         * buffer can hold.  Larger sizes may slightly improve compression, but
         * 1000 is already well into the realm of overkill.
         * The minimum safe size is 64 bits.
         */
        private const int MAX_CORR_BITS = 1000;	/* Max # of correction bits I can buffer */

        /* Derived data constructed for each Huffman table */
        private class CDerivedTable
        {
            public int[] ehufco = new int[256];   /* code for each symbol */
            public char[] ehufsi = new char[256];       /* length of code for each symbol */
            /* If no code has been allocated for a symbol S, ehufsi[S] contains 0 */
        }

        /* The savable_state subrecord contains fields that change within an MCU,
        * but must not be updated permanently until we complete the MCU.
        */
        private class SavableState
        {
            public int put_buffer { get; set; }       /* current bit-accumulation buffer */
            public int put_bits { get; set; }           /* # of bits now in it */
            public int[] last_dc_val = new int[JpegConstants.MAX_COMPS_IN_SCAN]; /* last DC coef for each component */

            public void ASSIGN_STATE(SavableState src)
            {
                put_buffer = src.put_buffer;
                put_bits = src.put_bits;

                for (var i = 0; i < last_dc_val.Length; i++)
                {
                    last_dc_val[i] = src.last_dc_val[i];
                }
            }
        }

        private readonly SavableState saved = new SavableState();        /* Bit buffer & DC state at start of MCU */

        /* These fields are NOT loaded into local working state. */
        private int restartsToGo;    /* MCUs left in this restart interval */
        private int nextRestartNum;       /* next restart number to write (0-7) */

        /* Pointers to derived tables (these workspaces have image lifespan) */
        private readonly CDerivedTable[] dcDerivedTables = new CDerivedTable[JpegConstants.NUM_HUFF_TBLS];
        private readonly CDerivedTable[] acDerivedTables = new CDerivedTable[JpegConstants.NUM_HUFF_TBLS];

        /* Statistics tables for optimization */
        private readonly long[][] dcCountPtrs = new long[JpegConstants.NUM_HUFF_TBLS][];
        private readonly long[][] acCountPtrs = new long[JpegConstants.NUM_HUFF_TBLS][];

        /* Following fields used only in progressive mode */

        /* Mode flag: TRUE for optimization, FALSE for actual data output */
        private bool gatherStatistics;

        private readonly JpegCompressStruct cinfo;

        /* Coding status for AC components */
        private int acTblNo;      /* the table number of the single component */
        private uint EOBRUN;        /* run length of EOBs */
        private uint BE;        /* # of buffered correction bits before MCU */
        private char[] bitBuffer;       /* buffer for correction bits (1 per char) */
        /* packing correction bits tightly would save some space but cost time... */

        public HuffmanEntropyEncoder(JpegCompressStruct cinfo)
        {
            this.cinfo = cinfo;

            /* Mark tables unallocated */
            for (var i = 0; i < JpegConstants.NUM_HUFF_TBLS; i++)
            {
                dcDerivedTables[i] = acDerivedTables[i] = null;
                dcCountPtrs[i] = acCountPtrs[i] = null;
            }

            if (this.cinfo.m_progressive_mode)
            {
                /* needed only in AC refinement scan */
                bitBuffer = null;
            }
        }

        /// <summary>
        /// Initialize for a Huffman-compressed scan.
        /// If gather_statistics is true, we do not output anything during the scan,
        /// just count the Huffman symbols used and generate Huffman code tables.
        /// </summary>
        public override void StartPass(bool gather_statistics)
        {
            gatherStatistics = gather_statistics;

            if (gather_statistics)
            {
                finishPass = FinishPassGather;
            }
            else
            {
                finishPass = FinishPassHuffman;
            }

            if (cinfo.m_progressive_mode)
            {
                /* We assume the scan parameters are already validated. */

                /* Select execution routine */
                if (cinfo.m_Ah == 0)
                {
                    if (cinfo.m_Ss == 0)
                    {
                        encodeMcu = EncodeMcuDCFirst;
                    }
                    else
                    {
                        encodeMcu = EncodeMcuACFirst;
                    }
                }
                else
                {
                    if (cinfo.m_Ss == 0)
                    {
                        encodeMcu = EncodeMcuDCRefine;
                    }
                    else
                    {
                        encodeMcu = EncodeMcuACRefine;
                        /* AC refinement needs a correction bit buffer */
                        if (bitBuffer is null)
                        {
                            bitBuffer = new char[MAX_CORR_BITS];
                        }
                    }
                }

                /* Initialize AC stuff */
                acTblNo = cinfo.Component_info[cinfo.m_cur_comp_info[0]].Ac_tbl_no;
                EOBRUN = 0;
                BE = 0;
            }
            else
            {
                if (gather_statistics)
                {
                    encodeMcu = EncodeMcuGather;
                }
                else
                {
                    encodeMcu = EncodeMcuHuffman;
                }
            }

            for (var ci = 0; ci < cinfo.m_comps_in_scan; ci++)
            {
                var compptr = cinfo.Component_info[cinfo.m_cur_comp_info[ci]];

                /* DC needs no table for refinement scan */
                if (cinfo.m_Ss == 0 && cinfo.m_Ah == 0)
                {
                    var tbl = compptr.Dc_tbl_no;
                    if (gather_statistics)
                    {
                        /* Check for invalid table index */
                        /* (make_c_derived_tbl does this in the other path) */
                        if (tbl < 0 || tbl >= JpegConstants.NUM_HUFF_TBLS)
                        {
                            cinfo.ErrExit(JMessageCode.JERR_NO_HUFF_TABLE, tbl);
                        }

                        /* Allocate and zero the statistics tables */
                        /* Note that jpeg_gen_optimal_table expects 257 entries in each table! */
                        if (dcCountPtrs[tbl] is null)
                        {
                            dcCountPtrs[tbl] = new long[257];
                        }
                        else
                        {
                            Array.Clear(dcCountPtrs[tbl], 0, dcCountPtrs[tbl].Length);
                        }
                    }
                    else
                    {
                        /* Compute derived values for Huffman tables */
                        /* We may do this more than once for a table, but it's not expensive */
                        JpegMakeCDerivedTable(true, tbl, ref dcDerivedTables[tbl]);
                    }

                    /* Initialize DC predictions to 0 */
                    saved.last_dc_val[ci] = 0;
                }

                /* AC needs no table when not present */
                if (cinfo.m_Se != 0)
                {
                    var tbl = compptr.Ac_tbl_no;
                    if (gather_statistics)
                    {
                        if (tbl < 0 || tbl >= JpegConstants.NUM_HUFF_TBLS)
                        {
                            cinfo.ErrExit(JMessageCode.JERR_NO_HUFF_TABLE, tbl);
                        }

                        if (acCountPtrs[tbl] is null)
                        {
                            acCountPtrs[tbl] = new long[257];
                        }
                        else
                        {
                            Array.Clear(acCountPtrs[tbl], 0, acCountPtrs[tbl].Length);
                        }
                    }
                    else
                    {
                        JpegMakeCDerivedTable(false, tbl, ref acDerivedTables[tbl]);
                    }
                }
            }

            /* Initialize bit buffer to empty */
            saved.put_buffer = 0;
            saved.put_bits = 0;

            /* Initialize restart stuff */
            restartsToGo = cinfo.m_restart_interval;
            nextRestartNum = 0;
        }

        /// <summary>
        /// Encode and output one MCU's worth of Huffman-compressed coefficients.
        /// </summary>
        private bool EncodeMcuHuffman(JBlock[][] MCU_data)
        {
            /* Load up working state */
            var state = new SavableState();
            state.ASSIGN_STATE(saved);

            /* Emit restart marker if needed */
            if (cinfo.m_restart_interval != 0)
            {
                if (restartsToGo == 0)
                {
                    if (!EmitRestartS(state, nextRestartNum))
                    {
                        return false;
                    }
                }
            }

            /* Encode the MCU data blocks */
            for (var blkn = 0; blkn < cinfo.m_blocks_in_MCU; blkn++)
            {
                var ci = cinfo.m_MCU_membership[blkn];
                var compptr = cinfo.Component_info[cinfo.m_cur_comp_info[ci]];
                if (!EncodeOneBlock(state, MCU_data[blkn][0].data, state.last_dc_val[ci],
                    dcDerivedTables[compptr.Dc_tbl_no],
                    acDerivedTables[compptr.Ac_tbl_no]))
                {
                    return false;
                }

                /* Update last_dc_val */
                state.last_dc_val[ci] = MCU_data[blkn][0][0];
            }

            /* Completed MCU, so update state */
            saved.ASSIGN_STATE(state);

            /* Update restart-interval state too */
            if (cinfo.m_restart_interval != 0)
            {
                if (restartsToGo == 0)
                {
                    restartsToGo = cinfo.m_restart_interval;
                    nextRestartNum++;
                    nextRestartNum &= 7;
                }

                restartsToGo--;
            }

            return true;
        }

        /// <summary>
        /// Finish up at the end of a Huffman-compressed scan.
        /// </summary>
        private void FinishPassHuffman()
        {
            if (cinfo.m_progressive_mode)
            {
                /* Flush out any buffered data */
                EmitEOBRun();
                FlushBitsE();
            }
            else
            {
                /* Load up working state ... flush_bits needs it */
                var state = new SavableState();
                state.ASSIGN_STATE(saved);

                /* Flush out the last data */
                if (!FlushBitsS(state))
                {
                    cinfo.ErrExit(JMessageCode.JERR_CANT_SUSPEND);
                }

                /* Update state */
                saved.ASSIGN_STATE(state);
            }
        }

        /// <summary>
        /// Trial-encode one MCU's worth of Huffman-compressed coefficients.
        /// No data is actually output, so no suspension return is possible.
        /// </summary>
        private bool EncodeMcuGather(JBlock[][] MCU_data)
        {
            /* Take care of restart intervals if needed */
            if (cinfo.m_restart_interval != 0)
            {
                if (restartsToGo == 0)
                {
                    /* Re-initialize DC predictions to 0 */
                    for (var ci = 0; ci < cinfo.m_comps_in_scan; ci++)
                    {
                        saved.last_dc_val[ci] = 0;
                    }

                    /* Update restart state */
                    restartsToGo = cinfo.m_restart_interval;
                }

                restartsToGo--;
            }

            for (var blkn = 0; blkn < cinfo.m_blocks_in_MCU; blkn++)
            {
                var ci = cinfo.m_MCU_membership[blkn];
                var compptr = cinfo.Component_info[cinfo.m_cur_comp_info[ci]];
                HTestOneBlock(MCU_data[blkn][0].data, saved.last_dc_val[ci],
                    dcCountPtrs[compptr.Dc_tbl_no],
                    acCountPtrs[compptr.Ac_tbl_no]);
                saved.last_dc_val[ci] = MCU_data[blkn][0][0];
            }

            return true;
        }

        /// <summary>
        /// Finish up a statistics-gathering pass and create the new Huffman tables.
        /// </summary>
        private void FinishPassGather()
        {
            if (cinfo.m_progressive_mode)
            {
                /* Flush out buffered data (all we care about is counting the EOB symbol) */
                EmitEOBRun();
            }

            /* It's important not to apply jpeg_gen_optimal_table more than once
             * per table, because it clobbers the input frequency counts!
             */
            var did_dc = new bool[JpegConstants.NUM_HUFF_TBLS];
            var did_ac = new bool[JpegConstants.NUM_HUFF_TBLS];

            for (var ci = 0; ci < cinfo.m_comps_in_scan; ci++)
            {
                var compptr = cinfo.Component_info[cinfo.m_cur_comp_info[ci]];
                /* DC needs no table for refinement scan */
                if (cinfo.m_Ss == 0 && cinfo.m_Ah == 0)
                {
                    var dctbl = compptr.Dc_tbl_no;
                    if (!did_dc[dctbl])
                    {
                        if (cinfo.m_dc_huff_tbl_ptrs[dctbl] is null)
                        {
                            cinfo.m_dc_huff_tbl_ptrs[dctbl] = new JHuffmanTable();
                        }

                        JpegGenOptimalTable(cinfo.m_dc_huff_tbl_ptrs[dctbl], dcCountPtrs[dctbl]);
                        did_dc[dctbl] = true;
                    }
                }

                /* AC needs no table when not present */
                if (cinfo.m_Se != 0)
                {
                    var actbl = compptr.Ac_tbl_no;
                    if (!did_ac[actbl])
                    {
                        if (cinfo.m_ac_huff_tbl_ptrs[actbl] is null)
                        {
                            cinfo.m_ac_huff_tbl_ptrs[actbl] = new JHuffmanTable();
                        }

                        JpegGenOptimalTable(cinfo.m_ac_huff_tbl_ptrs[actbl], acCountPtrs[actbl]);
                        did_ac[actbl] = true;
                    }
                }
            }
        }

        /// <summary>
        /// Encode a single block's worth of coefficients
        /// </summary>
        private bool EncodeOneBlock(SavableState state, short[] block, int last_dc_val, CDerivedTable dctbl, CDerivedTable actbl)
        {
            /* Encode the DC coefficient difference per section F.1.2.1 */
            var temp = block[0] - last_dc_val;
            var temp2 = temp;
            if (temp < 0)
            {
                temp = -temp;       /* temp is abs value of input */
                /* For a negative input, want temp2 = bitwise complement of abs(input) */
                /* This code assumes we are on a two's complement machine */
                temp2--;
            }

            /* Find the number of bits needed for the magnitude of the coefficient */
            var nbits = 0;
            while (temp != 0)
            {
                nbits++;
                temp >>= 1;
            }

            /* Check for out-of-range coefficient values.
             * Since we're encoding a difference, the range limit is twice as much.
             */
            if (nbits > MAX_COEF_BITS + 1)
            {
                cinfo.ErrExit(JMessageCode.JERR_BAD_DCT_COEF);
            }

            /* Emit the Huffman-coded symbol for the number of bits */
            if (!EmitBitsS(state, dctbl.ehufco[nbits], dctbl.ehufsi[nbits]))
            {
                return false;
            }

            /* Emit that number of bits of the value, if positive, */
            /* or the complement of its magnitude, if negative. */
            if (nbits != 0)
            {
                /* emit_bits rejects calls with size 0 */
                if (!EmitBitsS(state, temp2, nbits))
                {
                    return false;
                }
            }

            /* Encode the AC coefficients per section F.1.2.2 */
            var r = 0;          /* r = run length of zeros */
            var natural_order = cinfo.natural_order;
            var Se = cinfo.lim_Se;
            for (var k = 1; k <= Se; k++)
            {
                temp2 = block[natural_order[k]];
                if (temp2 == 0)
                {
                    r++;
                }
                else
                {
                    /* if run length > 15, must emit special run-length-16 codes (0xF0) */
                    while (r > 15)
                    {
                        if (!EmitBitsS(state, actbl.ehufco[0xF0], actbl.ehufsi[0xF0]))
                        {
                            return false;
                        }

                        r -= 16;
                    }

                    temp = temp2;
                    if (temp < 0)
                    {
                        temp = -temp;       /* temp is abs value of input */
                        /* This code assumes we are on a two's complement machine */
                        temp2--;
                    }

                    /* Find the number of bits needed for the magnitude of the coefficient */
                    nbits = 1;      /* there must be at least one 1 bit */
                    while ((temp >>= 1) != 0)
                    {
                        nbits++;
                    }

                    /* Check for out-of-range coefficient values */
                    if (nbits > MAX_COEF_BITS)
                    {
                        cinfo.ErrExit(JMessageCode.JERR_BAD_DCT_COEF);
                    }

                    /* Emit Huffman symbol for run length / number of bits */
                    temp = (r << 4) + nbits;
                    if (!EmitBitsS(state, actbl.ehufco[temp], actbl.ehufsi[temp]))
                    {
                        return false;
                    }

                    /* Emit that number of bits of the value, if positive, */
                    /* or the complement of its magnitude, if negative. */
                    if (!EmitBitsS(state, temp2, nbits))
                    {
                        return false;
                    }

                    r = 0;
                }
            }

            /* If the last coef(s) were zero, emit an end-of-block code */
            if (r > 0)
            {
                if (!EmitBitsS(state, actbl.ehufco[0], actbl.ehufsi[0]))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// <para>Huffman coding optimization.</para>
        /// <para>
        /// We first scan the supplied data and count the number of uses of each symbol
        /// that is to be Huffman-coded. (This process MUST agree with the code above.)
        /// Then we build a Huffman coding tree for the observed counts.
        /// Symbols which are not needed at all for the particular image are not
        /// assigned any code, which saves space in the DHT marker as well as in
        /// the compressed data.
        /// </para>
        /// </summary>
        private void HTestOneBlock(short[] block, int last_dc_val, long[] dc_counts, long[] ac_counts)
        {
            /* Encode the DC coefficient difference per section F.1.2.1 */
            var temp = block[0] - last_dc_val;
            if (temp < 0)
            {
                temp = -temp;
            }

            /* Find the number of bits needed for the magnitude of the coefficient */
            var nbits = 0;
            while (temp != 0)
            {
                nbits++;
                temp >>= 1;
            }

            /* Check for out-of-range coefficient values.
             * Since we're encoding a difference, the range limit is twice as much.
             */
            if (nbits > MAX_COEF_BITS + 1)
            {
                cinfo.ErrExit(JMessageCode.JERR_BAD_DCT_COEF);
            }

            /* Count the Huffman symbol for the number of bits */
            dc_counts[nbits]++;

            /* Encode the AC coefficients per section F.1.2.2 */
            var r = 0;          /* r = run length of zeros */
            var Se = cinfo.lim_Se;
            var natural_order = cinfo.natural_order;
            for (var k = 1; k <= Se; k++)
            {
                temp = block[natural_order[k]];
                if (temp == 0)
                {
                    r++;
                }
                else
                {
                    /* if run length > 15, must emit special run-length-16 codes (0xF0) */
                    while (r > 15)
                    {
                        ac_counts[0xF0]++;
                        r -= 16;
                    }

                    /* Find the number of bits needed for the magnitude of the coefficient */
                    if (temp < 0)
                    {
                        temp = -temp;
                    }

                    /* Find the number of bits needed for the magnitude of the coefficient */
                    nbits = 1;      /* there must be at least one 1 bit */
                    while ((temp >>= 1) != 0)
                    {
                        nbits++;
                    }

                    /* Check for out-of-range coefficient values */
                    if (nbits > MAX_COEF_BITS)
                    {
                        cinfo.ErrExit(JMessageCode.JERR_BAD_DCT_COEF);
                    }

                    /* Count Huffman symbol for run length / number of bits */
                    ac_counts[(r << 4) + nbits]++;

                    r = 0;
                }
            }

            /* If the last coef(s) were zero, emit an end-of-block code */
            if (r > 0)
            {
                ac_counts[0]++;
            }
        }

        //////////////////////////////////////////////////////////////////////////
        // Outputting bytes to the file.
        // NB: these must be called only when actually outputting,
        // that is, entropy.gather_statistics == false.

        private bool EmitByteS(int val)
        {
            return cinfo.m_dest.EmitByte(val);
        }

        private void EmitByteE(int val)
        {
            _ = cinfo.m_dest.EmitByte(val);
        }

        /// <summary>
        /// Only the right 24 bits of put_buffer are used; the valid bits are
        /// left-justified in this part.  At most 16 bits can be passed to emit_bits
        /// in one call, and we never retain more than 7 bits in put_buffer
        /// between calls, so 24 bits are sufficient.
        /// </summary>
        /// Emit some bits; return true if successful, false if must suspend
        private bool EmitBitsS(SavableState state, int code, int size)
        {
            /* This routine is heavily used, so it's worth coding tightly. */

            /* if size is 0, caller used an invalid Huffman table entry */
            if (size == 0)
            {
                cinfo.ErrExit(JMessageCode.JERR_HUFF_MISSING_CODE);
            }

            /* mask off any extra bits in code */
            var put_buffer = code & ((1 << size) - 1);

            /* new number of bits in buffer */
            var put_bits = size + state.put_bits;

            put_buffer <<= 24 - put_bits; /* align incoming bits */

            /* and merge with old buffer contents */
            put_buffer |= state.put_buffer;

            while (put_bits >= 8)
            {
                var c = (put_buffer >> 16) & 0xFF;
                if (!EmitByteS(c))
                {
                    return false;
                }

                if (c == 0xFF)
                {
                    /* need to stuff a zero byte? */
                    if (!EmitByteS(0))
                    {
                        return false;
                    }
                }

                put_buffer <<= 8;
                put_bits -= 8;
            }

            state.put_buffer = put_buffer; /* update state variables */
            state.put_bits = put_bits;

            return true;
        }

        /// <summary>
        /// <para>Outputting bits to the file</para>
        /// <para>
        /// Only the right 24 bits of put_buffer are used; the valid bits are
        /// left-justified in this part.  At most 16 bits can be passed to emit_bits
        /// in one call, and we never retain more than 7 bits in put_buffer
        /// between calls, so 24 bits are sufficient.
        /// </para>
        /// </summary>
        /// Emit some bits, unless we are in gather mode
        private void EmitBitsE(int code, int size)
        {
            /* This routine is heavily used, so it's worth coding tightly. */

            /* if size is 0, caller used an invalid Huffman table entry */
            if (size == 0)
            {
                cinfo.ErrExit(JMessageCode.JERR_HUFF_MISSING_CODE);
            }

            if (gatherStatistics)
            {
                /* do nothing if we're only getting stats */
                return;
            }

            var local_put_buffer = code & ((1 << size) - 1); /* mask off any extra bits in code */

            var put_bits = size + saved.put_bits;       /* new number of bits in buffer */

            local_put_buffer <<= 24 - put_bits; /* align incoming bits */

            local_put_buffer |= saved.put_buffer; /* and merge with old buffer contents */

            while (put_bits >= 8)
            {
                var c = (local_put_buffer >> 16) & 0xFF;

                EmitByteE(c);
                if (c == 0xFF)
                {
                    /* need to stuff a zero byte? */
                    EmitByteE(0);
                }

                local_put_buffer <<= 8;
                put_bits -= 8;
            }

            saved.put_buffer = local_put_buffer; /* update variables */
            saved.put_bits = put_bits;
        }

        private bool FlushBitsS(SavableState state)
        {
            if (!EmitBitsS(state, 0x7F, 7)) /* fill any partial byte with ones */
            {
                return false;
            }

            state.put_buffer = 0;  /* and reset bit-buffer to empty */
            state.put_bits = 0;
            return true;
        }

        private void FlushBitsE()
        {
            EmitBitsE(0x7F, 7); /* fill any partial byte with ones */
            saved.put_buffer = 0;     /* and reset bit-buffer to empty */
            saved.put_bits = 0;
        }

        // Emit (or just count) a Huffman symbol.
        private void EmitDCSymbol(int tbl_no, int symbol)
        {
            if (gatherStatistics)
            {
                dcCountPtrs[tbl_no][symbol]++;
            }
            else
            {
                var tbl = dcDerivedTables[tbl_no];
                EmitBitsE(tbl.ehufco[symbol], tbl.ehufsi[symbol]);
            }
        }

        private void EmitACSymbol(int tbl_no, int symbol)
        {
            if (gatherStatistics)
            {
                acCountPtrs[tbl_no][symbol]++;
            }
            else
            {
                var tbl = acDerivedTables[tbl_no];
                EmitBitsE(tbl.ehufco[symbol], tbl.ehufsi[symbol]);
            }
        }

        // Emit bits from a correction bit buffer.
        private void EmitBufferedBits(uint offset, uint nbits)
        {
            if (gatherStatistics)
            {
                /* no real work */
                return;
            }

            for (var i = 0; i < nbits; i++)
            {
                EmitBitsE(bitBuffer[offset + i], 1);
            }
        }

        // Emit any pending EOBRUN symbol.
        private void EmitEOBRun()
        {
            if (EOBRUN > 0)
            {
                /* if there is any pending EOBRUN */
                var temp = EOBRUN;
                var nbits = 0;
                while ((temp >>= 1) != 0)
                {
                    nbits++;
                }

                /* safety check: shouldn't happen given limited correction-bit buffer */
                if (nbits > 14)
                {
                    cinfo.ErrExit(JMessageCode.JERR_HUFF_MISSING_CODE);
                }

                EmitACSymbol(acTblNo, nbits << 4);
                if (nbits != 0)
                {
                    EmitBitsE((int)EOBRUN, nbits);
                }

                EOBRUN = 0;

                /* Emit any buffered correction bits */
                EmitBufferedBits(0, BE);
                BE = 0;
            }
        }

        /// <summary>
        /// Emit a restart marker and resynchronize predictions.
        /// </summary>
        private bool EmitRestartS(SavableState state, int restart_num)
        {
            if (!FlushBitsS(state))
            {
                return false;
            }

            if (!EmitByteS(0xFF))
            {
                return false;
            }

            if (!EmitByteS((int)(JpegMarker.RST0 + restart_num)))
            {
                return false;
            }

            /* Re-initialize DC predictions to 0 */
            for (var ci = 0; ci < cinfo.m_comps_in_scan; ci++)
            {
                state.last_dc_val[ci] = 0;
            }

            /* The restart counter is not updated until we successfully write the MCU. */
            return true;
        }

        // Emit a restart marker & resynchronize predictions.
        private void EmitRestartE(int restart_num)
        {
            EmitEOBRun();

            if (!gatherStatistics)
            {
                FlushBitsE();
                EmitByteE(0xFF);
                EmitByteE((int)(JpegMarker.RST0 + restart_num));
            }

            if (cinfo.m_Ss == 0)
            {
                /* Re-initialize DC predictions to 0 */
                for (var ci = 0; ci < cinfo.m_comps_in_scan; ci++)
                {
                    saved.last_dc_val[ci] = 0;
                }
            }
            else
            {
                /* Re-initialize all AC-related fields to 0 */
                EOBRUN = 0;
                BE = 0;
            }
        }

        /// <summary>
        /// IRIGHT_SHIFT is like RIGHT_SHIFT, but works on int rather than int.
        /// We assume that int right shift is unsigned if int right shift is,
        /// which should be safe.
        /// </summary>
        private static int IRightShift(int x, int shft)
        {
            if (x < 0)
            {
                return (x >> shft) | ((~0) << (16 - shft));
            }

            return x >> shft;
        }

        /// <summary>
        /// MCU encoding for DC initial scan (either spectral selection,
        /// or first pass of successive approximation).
        /// </summary>
        private bool EncodeMcuDCFirst(JBlock[][] MCU_data)
        {
            /* Emit restart marker if needed */
            if (cinfo.m_restart_interval != 0)
            {
                if (restartsToGo == 0)
                {
                    EmitRestartE(nextRestartNum);
                }
            }

            /* Encode the MCU data blocks */
            for (var blkn = 0; blkn < cinfo.m_blocks_in_MCU; blkn++)
            {
                var ci = cinfo.m_MCU_membership[blkn];
                var tbl = cinfo.Component_info[cinfo.m_cur_comp_info[ci]].Dc_tbl_no;

                /* Compute the DC value after the required point transform by Al.
                 * This is simply an arithmetic right shift.
                 */
                var temp = IRightShift(MCU_data[blkn][0][0], cinfo.m_Al);

                /* DC differences are figured on the point-transformed values. */

                var temp2 = temp - saved.last_dc_val[ci];
                saved.last_dc_val[ci] = temp;

                /* Encode the DC coefficient difference per section G.1.2.1 */
                temp = temp2;
                if (temp < 0)
                {
                    /* temp is abs value of input */
                    temp = -temp;

                    /* For a negative input, want temp2 = bitwise complement of abs(input) */
                    /* This code assumes we are on a two's complement machine */
                    temp2--;
                }

                /* Find the number of bits needed for the magnitude of the coefficient */
                var nbits = 0;
                while (temp != 0)
                {
                    nbits++;
                    temp >>= 1;
                }

                /* Check for out-of-range coefficient values.
                 * Since we're encoding a difference, the range limit is twice as much.
                 */
                if (nbits > MAX_COEF_BITS + 1)
                {
                    cinfo.ErrExit(JMessageCode.JERR_BAD_DCT_COEF);
                }

                /* Count/emit the Huffman-coded symbol for the number of bits */
                EmitDCSymbol(tbl, nbits);

                /* Emit that number of bits of the value, if positive, */
                /* or the complement of its magnitude, if negative. */
                if (nbits != 0)
                {
                    /* emit_bits rejects calls with size 0 */
                    EmitBitsE(temp2, nbits);
                }
            }

            /* Update restart-interval state too */
            if (cinfo.m_restart_interval != 0)
            {
                if (restartsToGo == 0)
                {
                    restartsToGo = cinfo.m_restart_interval;
                    nextRestartNum++;
                    nextRestartNum &= 7;
                }

                restartsToGo--;
            }

            return true;
        }

        /// <summary>
        /// MCU encoding for AC initial scan (either spectral selection,
        /// or first pass of successive approximation).
        /// </summary>
        private bool EncodeMcuACFirst(JBlock[][] MCU_data)
        {
            /* Emit restart marker if needed */
            if (cinfo.m_restart_interval != 0)
            {
                if (restartsToGo == 0)
                {
                    EmitRestartE(nextRestartNum);
                }
            }

            var natural_order = cinfo.natural_order;
            /* Encode the AC coefficients per section G.1.2.2, fig. G.3 */
            /* r = run length of zeros */
            var r = 0;
            for (var k = cinfo.m_Ss; k <= cinfo.m_Se; k++)
            {
                int temp = MCU_data[0][0][natural_order[k]];
                if (temp == 0)
                {
                    r++;
                    continue;
                }

                /* We must apply the point transform by Al.  For AC coefficients this
                 * is an integer division with rounding towards 0.  To do this portably
                 * in C, we shift after obtaining the absolute value; so the code is
                 * interwoven with finding the abs value (temp) and output bits (temp2).
                 */
                int temp2;
                if (temp < 0)
                {
                    temp = -temp;       /* temp is abs value of input */
                    temp >>= cinfo.m_Al;        /* apply the point transform */
                    /* For a negative coef, want temp2 = bitwise complement of abs(coef) */
                    temp2 = ~temp;
                }
                else
                {
                    temp >>= cinfo.m_Al;        /* apply the point transform */
                    temp2 = temp;
                }

                /* Watch out for case that nonzero coef is zero after point transform */
                if (temp == 0)
                {
                    r++;
                    continue;
                }

                /* Emit any pending EOBRUN */
                if (EOBRUN > 0)
                {
                    EmitEOBRun();
                }

                /* if run length > 15, must emit special run-length-16 codes (0xF0) */
                while (r > 15)
                {
                    EmitACSymbol(acTblNo, 0xF0);
                    r -= 16;
                }

                /* Find the number of bits needed for the magnitude of the coefficient */
                var nbits = 1;          /* there must be at least one 1 bit */
                while ((temp >>= 1) != 0)
                {
                    nbits++;
                }

                /* Check for out-of-range coefficient values */
                if (nbits > MAX_COEF_BITS)
                {
                    cinfo.ErrExit(JMessageCode.JERR_BAD_DCT_COEF);
                }

                /* Count/emit Huffman symbol for run length / number of bits */
                EmitACSymbol(acTblNo, (r << 4) + nbits);

                /* Emit that number of bits of the value, if positive, */
                /* or the complement of its magnitude, if negative. */
                EmitBitsE(temp2, nbits);

                r = 0;          /* reset zero run length */
            }

            if (r > 0)
            {
                /* If there are trailing zeroes, */
                EOBRUN++;      /* count an EOB */
                if (EOBRUN == 0x7FFF)
                {
                    EmitEOBRun();   /* force it out to avoid overflow */
                }
            }

            /* Update restart-interval state too */
            if (cinfo.m_restart_interval != 0)
            {
                if (restartsToGo == 0)
                {
                    restartsToGo = cinfo.m_restart_interval;
                    nextRestartNum++;
                    nextRestartNum &= 7;
                }

                restartsToGo--;
            }

            return true;
        }

        /// <summary>
        /// MCU encoding for DC successive approximation refinement scan.
        /// Note: we assume such scans can be multi-component, although the spec
        /// is not very clear on the point.
        /// </summary>
        private bool EncodeMcuDCRefine(JBlock[][] MCU_data)
        {
            /* Emit restart marker if needed */
            if (cinfo.m_restart_interval != 0)
            {
                if (restartsToGo == 0)
                {
                    EmitRestartE(nextRestartNum);
                }
            }

            /* Encode the MCU data blocks */
            for (var blkn = 0; blkn < cinfo.m_blocks_in_MCU; blkn++)
            {
                /* We simply emit the Al'th bit of the DC coefficient value. */
                int temp = MCU_data[blkn][0][0];
                EmitBitsE(temp >> cinfo.m_Al, 1);
            }

            /* Update restart-interval state too */
            if (cinfo.m_restart_interval != 0)
            {
                if (restartsToGo == 0)
                {
                    restartsToGo = cinfo.m_restart_interval;
                    nextRestartNum++;
                    nextRestartNum &= 7;
                }

                restartsToGo--;
            }

            return true;
        }

        /// <summary>
        /// MCU encoding for AC successive approximation refinement scan.
        /// </summary>
        private bool EncodeMcuACRefine(JBlock[][] MCU_data)
        {
            /* Emit restart marker if needed */
            if (cinfo.m_restart_interval != 0)
            {
                if (restartsToGo == 0)
                {
                    EmitRestartE(nextRestartNum);
                }
            }

            /* Encode the MCU data block */

            /* It is convenient to make a pre-pass to determine the transformed
             * coefficients' absolute values and the EOB position.
             */
            var EOB = 0;
            var natural_order = cinfo.natural_order;
            var absvalues = new int[JpegConstants.DCTSIZE2];
            for (var k = cinfo.m_Ss; k <= cinfo.m_Se; k++)
            {
                int temp = MCU_data[0][0][natural_order[k]];

                /* We must apply the point transform by Al.  For AC coefficients this
                 * is an integer division with rounding towards 0.  To do this portably
                 * in C, we shift after obtaining the absolute value.
                 */
                if (temp < 0)
                {
                    temp = -temp;       /* temp is abs value of input */
                }

                temp >>= cinfo.m_Al;        /* apply the point transform */
                absvalues[k] = temp;    /* save abs value for main pass */

                if (temp == 1)
                {
                    /* EOB = index of last newly-nonzero coef */
                    EOB = k;
                }
            }

            /* Encode the AC coefficients per section G.1.2.3, fig. G.7 */

            var r = 0;          /* r = run length of zeros */
            uint BR = 0;         /* BR = count of buffered bits added now */
            var bitBufferOffset = BE; /* Append bits to buffer */

            for (var k = cinfo.m_Ss; k <= cinfo.m_Se; k++)
            {
                var temp = absvalues[k];
                if (temp == 0)
                {
                    r++;
                    continue;
                }

                /* Emit any required ZRLs, but not if they can be folded into EOB */
                while (r > 15 && k <= EOB)
                {
                    /* emit any pending EOBRUN and the BE correction bits */
                    EmitEOBRun();

                    /* Emit ZRL */
                    EmitACSymbol(acTblNo, 0xF0);
                    r -= 16;

                    /* Emit buffered correction bits that must be associated with ZRL */
                    EmitBufferedBits(bitBufferOffset, BR);
                    bitBufferOffset = 0;/* BE bits are gone now */
                    BR = 0;
                }

                /* If the coef was previously nonzero, it only needs a correction bit.
                 * NOTE: a straight translation of the spec's figure G.7 would suggest
                 * that we also need to test r > 15.  But if r > 15, we can only get here
                 * if k > EOB, which implies that this coefficient is not 1.
                 */
                if (temp > 1)
                {
                    /* The correction bit is the next bit of the absolute value. */
                    bitBuffer[bitBufferOffset + BR] = (char)(temp & 1);
                    BR++;
                    continue;
                }

                /* Emit any pending EOBRUN and the BE correction bits */
                EmitEOBRun();

                /* Count/emit Huffman symbol for run length / number of bits */
                EmitACSymbol(acTblNo, (r << 4) + 1);

                /* Emit output bit for newly-nonzero coef */
                temp = (MCU_data[0][0][natural_order[k]] < 0) ? 0 : 1;
                EmitBitsE(temp, 1);

                /* Emit buffered correction bits that must be associated with this code */
                EmitBufferedBits(bitBufferOffset, BR);
                bitBufferOffset = 0;/* BE bits are gone now */
                BR = 0;
                r = 0;          /* reset zero run length */
            }

            if (r > 0 || BR > 0)
            {
                /* If there are trailing zeroes, */
                EOBRUN++;      /* count an EOB */
                BE += BR;      /* concat my correction bits to older ones */

                /* We force out the EOB if we risk either:
                 * 1. overflow of the EOB counter;
                 * 2. overflow of the correction bit buffer during the next MCU.
                 */
                if (EOBRUN == 0x7FFF || BE > (MAX_CORR_BITS - JpegConstants.DCTSIZE2 + 1))
                {
                    EmitEOBRun();
                }
            }

            /* Update restart-interval state too */
            if (cinfo.m_restart_interval != 0)
            {
                if (restartsToGo == 0)
                {
                    restartsToGo = cinfo.m_restart_interval;
                    nextRestartNum++;
                    nextRestartNum &= 7;
                }

                restartsToGo--;
            }

            return true;
        }

        /// <summary>
        /// Expand a Huffman table definition into the derived format
        /// Compute the derived values for a Huffman table.
        /// This routine also performs some validation checks on the table.
        /// </summary>
        private void JpegMakeCDerivedTable(bool isDC, int tblno, ref CDerivedTable dtbl)
        {
            /* Note that huffsize[] and huffcode[] are filled in code-length order,
            * paralleling the order of the symbols themselves in htbl.huffval[].
            */

            /* Find the input Huffman table */
            if (tblno < 0 || tblno >= JpegConstants.NUM_HUFF_TBLS)
            {
                cinfo.ErrExit(JMessageCode.JERR_NO_HUFF_TABLE, tblno);
            }

            var htbl = isDC ? cinfo.m_dc_huff_tbl_ptrs[tblno] : cinfo.m_ac_huff_tbl_ptrs[tblno];
            if (htbl is null)
            {
                cinfo.ErrExit(JMessageCode.JERR_NO_HUFF_TABLE, tblno);
            }

            /* Allocate a workspace if we haven't already done so. */
            if (dtbl is null)
            {
                dtbl = new CDerivedTable();
            }

            /* Figure C.1: make table of Huffman code length for each symbol */

            var p = 0;
            var huffsize = new char[257];
            for (var l = 1; l <= 16; l++)
            {
                int i = htbl.Bits[l];
                if (p + i > 256)    /* protect against table overrun */
                {
                    cinfo.ErrExit(JMessageCode.JERR_BAD_HUFF_TABLE);
                }

                while (i-- != 0)
                {
                    huffsize[p++] = (char)l;
                }
            }

            huffsize[p] = (char)0;
            var lastp = p;

            /* Figure C.2: generate the codes themselves */
            /* We also validate that the counts represent a legal Huffman code tree. */

            var code = 0;
            int si = huffsize[0];
            p = 0;
            var huffcode = new int[257];
            while (huffsize[p] != 0)
            {
                while (huffsize[p] == si)
                {
                    huffcode[p++] = code;
                    code++;
                }
                /* code is now 1 more than the last code used for codelength si; but
                * it must still fit in si bits, since no code is allowed to be all ones.
                */
                if (code >= (1 << si))
                {
                    cinfo.ErrExit(JMessageCode.JERR_BAD_HUFF_TABLE);
                }

                code <<= 1;
                si++;
            }

            /* Figure C.3: generate encoding tables */
            /* These are code and size indexed by symbol value */

            /* Set all codeless symbols to have code length 0;
            * this lets us detect duplicate VAL entries here, and later
            * allows emit_bits to detect any attempt to emit such symbols.
            */
            Array.Clear(dtbl.ehufsi, 0, dtbl.ehufsi.Length);

            /* This is also a convenient place to check for out-of-range
            * and duplicated VAL entries.  We allow 0..255 for AC symbols
            * but only 0..15 for DC.  (We could constrain them further
            * based on data depth and mode, but this seems enough.)
            */
            var maxsymbol = isDC ? 15 : 255;

            for (p = 0; p < lastp; p++)
            {
                int i = htbl.Huffval[p];
                if (i > maxsymbol || dtbl.ehufsi[i] != 0)
                {
                    cinfo.ErrExit(JMessageCode.JERR_BAD_HUFF_TABLE);
                }

                dtbl.ehufco[i] = huffcode[p];
                dtbl.ehufsi[i] = huffsize[p];
            }
        }

        /// <summary>
        /// <para>Generate the best Huffman code table for the given counts, fill htbl.</para>
        /// <para>
        /// The JPEG standard requires that no symbol be assigned a codeword of all
        /// one bits (so that padding bits added at the end of a compressed segment
        /// can't look like a valid code).  Because of the canonical ordering of
        /// codewords, this just means that there must be an unused slot in the
        /// longest codeword length category.  Section K.2 of the JPEG spec suggests
        /// reserving such a slot by pretending that symbol 256 is a valid symbol
        /// with count 1.  In theory that's not optimal; giving it count zero but
        /// including it in the symbol set anyway should give a better Huffman code.
        /// But the theoretically better code actually seems to come out worse in
        /// practice, because it produces more all-ones bytes (which incur stuffed
        /// zero bytes in the final file).  In any case the difference is tiny.
        /// </para>
        /// <para>
        /// The JPEG standard requires Huffman codes to be no more than 16 bits long.
        /// If some symbols have a very small but nonzero probability, the Huffman tree
        /// must be adjusted to meet the code length restriction.  We currently use
        /// the adjustment method suggested in JPEG section K.2.  This method is *not*
        /// optimal; it may not choose the best possible limited-length code.  But
        /// typically only very-low-frequency symbols will be given less-than-optimal
        /// lengths, so the code is almost optimal.  Experimental comparisons against
        /// an optimal limited-length-code algorithm indicate that the difference is
        /// microscopic --- usually less than a hundredth of a percent of total size.
        /// So the extra complexity of an optimal algorithm doesn't seem worthwhile.
        /// </para>
        /// </summary>
        protected void JpegGenOptimalTable(JHuffmanTable htbl, long[] freq)
        {
            const int MAX_CLEN = 32;     /* assumed maximum initial code length */

            var bits = new byte[MAX_CLEN + 1];   /* bits[k] = # of symbols with code length k */
            var codesize = new int[257];      /* codesize[k] = code length of symbol k */
            var others = new int[257];        /* next symbol in current branch of tree */
            int c1, c2;
            int p, i, j;
            long v;

            /* This algorithm is explained in section K.2 of the JPEG standard */
            for (i = 0; i < 257; i++)
            {
                others[i] = -1;     /* init links to empty */
            }

            freq[256] = 1;      /* make sure 256 has a nonzero count */
            /* Including the pseudo-symbol 256 in the Huffman procedure guarantees
            * that no real symbol is given code-value of all ones, because 256
            * will be placed last in the largest codeword category.
            */

            /* Huffman's basic algorithm to assign optimal code lengths to symbols */

            for (; ; )
            {
                /* Find the smallest nonzero frequency, set c1 = its symbol */
                /* In case of ties, take the larger symbol number */
                c1 = -1;
                v = 1000000000L;
                for (i = 0; i <= 256; i++)
                {
                    if (freq[i] != 0 && freq[i] <= v)
                    {
                        v = freq[i];
                        c1 = i;
                    }
                }

                /* Find the next smallest nonzero frequency, set c2 = its symbol */
                /* In case of ties, take the larger symbol number */
                c2 = -1;
                v = 1000000000L;
                for (i = 0; i <= 256; i++)
                {
                    if (freq[i] != 0 && freq[i] <= v && i != c1)
                    {
                        v = freq[i];
                        c2 = i;
                    }
                }

                /* Done if we've merged everything into one frequency */
                if (c2 < 0)
                {
                    break;
                }

                /* Else merge the two counts/trees */
                freq[c1] += freq[c2];
                freq[c2] = 0;

                /* Increment the codesize of everything in c1's tree branch */
                codesize[c1]++;
                while (others[c1] >= 0)
                {
                    c1 = others[c1];
                    codesize[c1]++;
                }

                others[c1] = c2;        /* chain c2 onto c1's tree branch */

                /* Increment the codesize of everything in c2's tree branch */
                codesize[c2]++;
                while (others[c2] >= 0)
                {
                    c2 = others[c2];
                    codesize[c2]++;
                }
            }

            /* Now count the number of symbols of each code length */
            for (i = 0; i <= 256; i++)
            {
                if (codesize[i] != 0)
                {
                    /* The JPEG standard seems to think that this can't happen, */
                    /* but I'm paranoid... */
                    if (codesize[i] > MAX_CLEN)
                    {
                        cinfo.ErrExit(JMessageCode.JERR_HUFF_CLEN_OVERFLOW);
                    }

                    bits[codesize[i]]++;
                }
            }

            /* JPEG doesn't allow symbols with code lengths over 16 bits, so if the pure
            * Huffman procedure assigned any such lengths, we must adjust the coding.
            * Here is what the JPEG spec says about how this next bit works:
            * Since symbols are paired for the longest Huffman code, the symbols are
            * removed from this length category two at a time.  The prefix for the pair
            * (which is one bit shorter) is allocated to one of the pair; then,
            * skipping the BITS entry for that prefix length, a code word from the next
            * shortest nonzero BITS entry is converted into a prefix for two code words
            * one bit longer.
            */

            for (i = MAX_CLEN; i > 16; i--)
            {
                while (bits[i] > 0)
                {
                    j = i - 2;      /* find length of new prefix to be used */
                    while (bits[j] == 0)
                    {
                        j--;
                    }

                    bits[i] -= 2;       /* remove two symbols */
                    bits[i - 1]++;      /* one goes in this length */
                    bits[j + 1] += 2;       /* two new symbols in this length */
                    bits[j]--;      /* symbol of this length is now a prefix */
                }
            }

            /* Remove the count for the pseudo-symbol 256 from the largest codelength */
            while (bits[i] == 0)        /* find largest codelength still in use */
            {
                i--;
            }

            bits[i]--;

            /* Return final symbol counts (only for lengths 0..16) */
            Buffer.BlockCopy(bits, 0, htbl.Bits, 0, htbl.Bits.Length);

            /* Return a list of the symbols sorted by code length */
            /* It's not real clear to me why we don't need to consider the codelength
            * changes made above, but the JPEG spec seems to think this works.
            */
            p = 0;
            for (i = 1; i <= MAX_CLEN; i++)
            {
                for (j = 0; j <= 255; j++)
                {
                    if (codesize[j] == i)
                    {
                        htbl.Huffval[p] = (byte)j;
                        p++;
                    }
                }
            }

            /* Set sent_table false so updated table will be written to JPEG file. */
            htbl.SentTable = false;
        }
    }
}
