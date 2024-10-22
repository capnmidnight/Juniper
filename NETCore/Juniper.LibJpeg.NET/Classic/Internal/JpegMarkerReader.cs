/*
 * This file contains routines to decode JPEG datastream markers.
 * Most of the complexity arises from our desire to support input
 * suspension: if not all of the data for a marker is available,
 * we must exit back to the application.  On resumption, we reprocess
 * the marker.
 */

using System;

namespace BitMiracle.LibJpeg.Classic.Internal
{
    /// <summary>
    /// Marker reading and parsing
    /// </summary>
    internal class JpegMarkerReader
    {
        private const int APP0_DATA_LEN = 14;  /* Length of interesting data in APP0 */
        private const int APP14_DATA_LEN = 12;  /* Length of interesting data in APP14 */
        private const int APPN_DATA_LEN = 14;  /* Must be the largest of the above!! */

        private readonly JpegDecompressStruct m_cinfo;

        /* Application-overridable marker processing methods */
        private JpegDecompressStruct.JpegMarkerParserMethod m_process_COM;
        private readonly JpegDecompressStruct.JpegMarkerParserMethod[] m_process_APPn = new JpegDecompressStruct.JpegMarkerParserMethod[16];

        /* Limit on marker data length to save for each marker type */
        private int m_length_limit_COM;
        private readonly int[] m_length_limit_APPn = new int[16];

        private bool m_saw_SOI;       /* found SOI? */
        private bool m_saw_SOF;       /* found SOF? */
        private int m_next_restart_num;       /* next restart number expected (0-7) */
        private int m_discarded_bytes;   /* # of bytes skipped looking for a marker */

        /* Status of COM/APPn marker saving */
        private JpegMarkerStruct m_cur_marker; /* null if not processing a marker */
        private int m_bytes_read;        /* data bytes read so far in marker */
        /* Note: cur_marker is not linked into marker_list until it's all read. */

        /// <summary>
        /// Initialize the marker reader module.
        /// This is called only once, when the decompression object is created.
        /// </summary>
        public JpegMarkerReader(JpegDecompressStruct cinfo)
        {
            m_cinfo = cinfo;

            /* Initialize COM/APPn processing.
            * By default, we examine and then discard APP0 and APP14,
            * but simply discard COM and all other APPn.
            */
            m_process_COM = SkipVariable;

            for (var i = 0; i < 16; i++)
            {
                m_process_APPn[i] = SkipVariable;
                m_length_limit_APPn[i] = 0;
            }

            m_process_APPn[0] = GetInterestingAppN;
            m_process_APPn[14] = GetInterestingAppN;

            /* Reset marker processing state */
            ResetMarkerReader();
        }

        /// <summary>
        /// Reset marker processing state to begin a fresh datastream.
        /// </summary>
        public void ResetMarkerReader()
        {
            m_cinfo.CompInfo = null;        /* until allocated by get_sof */
            m_cinfo.inputScanNumber = 0;       /* no SOS seen yet */
            m_cinfo.m_unreadMarker = 0;       /* no pending marker */
            m_saw_SOI = false;        /* set internal state too */
            m_saw_SOF = false;
            m_discarded_bytes = 0;
            m_cur_marker = null;
        }

        /// <summary>
        /// <para>Read markers until SOS or EOI.</para>
        /// <para>
        /// Returns same codes as are defined for jpeg_consume_input:
        /// JPEG_SUSPENDED, JPEG_REACHED_SOS, or JPEG_REACHED_EOI.
        /// </para>
        /// <para>
        /// Note: This function may return a pseudo SOS marker(with zero
        /// component number) for treat by input controller's consume_input.
        /// consume_input itself should filter out (skip) the pseudo marker
        /// after processing for the caller.
        /// </para>
        /// </summary>
        public ReadResult ReadMarkers()
        {
            /* Outer loop repeats once for each marker. */
            while (true)
            {
                /* Collect the marker proper, unless we already did. */
                /* NB: first_marker() enforces the requirement that SOI appear first. */
                if (m_cinfo.m_unreadMarker == 0)
                {
                    if (!m_cinfo.m_marker.m_saw_SOI)
                    {
                        if (!FirstMarker())
                        {
                            return ReadResult.JPEG_SUSPENDED;
                        }
                    }
                    else if (!NextMarker())
                    {
                        return ReadResult.JPEG_SUSPENDED;
                    }
                }

                /* At this point m_cinfo.unread_marker contains the marker code and the
                 * input point is just past the marker proper, but before any parameters.
                 * A suspension will cause us to return with this state still true.
                 */
                switch ((JpegMarker)m_cinfo.m_unreadMarker)
                {
                    case JpegMarker.SOI:
                    GetSOI();
                    break;

                    case JpegMarker.SOF0:
                    /* Baseline */
                    if (!GetSOF(true, false, false))
                    {
                        return ReadResult.JPEG_SUSPENDED;
                    }

                    break;

                    case JpegMarker.SOF1:
                    /* Extended sequential, Huffman */
                    if (!GetSOF(false, false, false))
                    {
                        return ReadResult.JPEG_SUSPENDED;
                    }

                    break;

                    case JpegMarker.SOF2:
                    /* Progressive, Huffman */
                    if (!GetSOF(false, true, false))
                    {
                        return ReadResult.JPEG_SUSPENDED;
                    }

                    break;

                    case JpegMarker.SOF9:
                    /* Extended sequential, arithmetic */
                    if (!GetSOF(false, false, true))
                    {
                        return ReadResult.JPEG_SUSPENDED;
                    }

                    break;

                    case JpegMarker.SOF10:
                    /* Progressive, arithmetic */
                    if (!GetSOF(false, true, true))
                    {
                        return ReadResult.JPEG_SUSPENDED;
                    }

                    break;

                    /* Currently unsupported SOFn types */
                    case JpegMarker.SOF3:
                    /* Lossless, Huffman */
                    case JpegMarker.SOF5:
                    /* Differential sequential, Huffman */
                    case JpegMarker.SOF6:
                    /* Differential progressive, Huffman */
                    case JpegMarker.SOF7:
                    /* Differential lossless, Huffman */
                    case JpegMarker.JPG:
                    /* Reserved for JPEG extensions */
                    case JpegMarker.SOF11:
                    /* Lossless, arithmetic */
                    case JpegMarker.SOF13:
                    /* Differential sequential, arithmetic */
                    case JpegMarker.SOF14:
                    /* Differential progressive, arithmetic */
                    case JpegMarker.SOF15:
                    /* Differential lossless, arithmetic */
                    m_cinfo.ErrExit(JMessageCode.JERR_SOF_UNSUPPORTED, m_cinfo.m_unreadMarker);
                    break;

                    case JpegMarker.SOS:
                    if (!GetSOS())
                    {
                        return ReadResult.JPEG_SUSPENDED;
                    }

                    m_cinfo.m_unreadMarker = 0;   /* processed the marker */
                    return ReadResult.JPEG_REACHED_SOS;

                    case JpegMarker.EOI:
                    m_cinfo.TraceMS(1, JMessageCode.JTRC_EOI);
                    m_cinfo.m_unreadMarker = 0;   /* processed the marker */
                    return ReadResult.JPEG_REACHED_EOI;

                    case JpegMarker.DAC:
                    if (!GetDAC())
                    {
                        return ReadResult.JPEG_SUSPENDED;
                    }

                    break;

                    case JpegMarker.DHT:
                    if (!GetDHT())
                    {
                        return ReadResult.JPEG_SUSPENDED;
                    }

                    break;

                    case JpegMarker.DQT:
                    if (!GetDQT())
                    {
                        return ReadResult.JPEG_SUSPENDED;
                    }

                    break;

                    case JpegMarker.DRI:
                    if (!GetDRI())
                    {
                        return ReadResult.JPEG_SUSPENDED;
                    }

                    break;

                    case JpegMarker.JPG8:
                    if (!GetLSE())
                    {
                        return ReadResult.JPEG_SUSPENDED;
                    }

                    break;

                    case JpegMarker.APP0:
                    case JpegMarker.APP1:
                    case JpegMarker.APP2:
                    case JpegMarker.APP3:
                    case JpegMarker.APP4:
                    case JpegMarker.APP5:
                    case JpegMarker.APP6:
                    case JpegMarker.APP7:
                    case JpegMarker.APP8:
                    case JpegMarker.APP9:
                    case JpegMarker.APP10:
                    case JpegMarker.APP11:
                    case JpegMarker.APP12:
                    case JpegMarker.APP13:
                    case JpegMarker.APP14:
                    case JpegMarker.APP15:
                    if (!m_cinfo.m_marker.m_process_APPn[m_cinfo.m_unreadMarker - (int)JpegMarker.APP0](m_cinfo))
                    {
                        return ReadResult.JPEG_SUSPENDED;
                    }

                    break;

                    case JpegMarker.COM:
                    if (!m_cinfo.m_marker.m_process_COM(m_cinfo))
                    {
                        return ReadResult.JPEG_SUSPENDED;
                    }

                    break;

                    /* these are all parameterless */
                    case JpegMarker.RST0:
                    case JpegMarker.RST1:
                    case JpegMarker.RST2:
                    case JpegMarker.RST3:
                    case JpegMarker.RST4:
                    case JpegMarker.RST5:
                    case JpegMarker.RST6:
                    case JpegMarker.RST7:
                    case JpegMarker.TEM:
                    m_cinfo.TraceMS(1, JMessageCode.JTRC_PARMLESS_MARKER, m_cinfo.m_unreadMarker);
                    break;

                    case JpegMarker.DNL:
                    /* Ignore DNL ... perhaps the wrong thing */
                    if (!SkipVariable(m_cinfo))
                    {
                        return ReadResult.JPEG_SUSPENDED;
                    }

                    break;

                    default:
                    /* must be DHP, EXP, JPGn, or RESn */
                    /* For now, we treat the reserved markers as fatal errors since they are
                     * likely to be used to signal incompatible JPEG Part 3 extensions.
                     * Once the JPEG 3 version-number marker is well defined, this code
                     * ought to change!
                     */
                    m_cinfo.ErrExit(JMessageCode.JERR_UNKNOWN_MARKER, m_cinfo.m_unreadMarker);
                    break;
                }

                /* Successfully processed marker, so reset state variable */
                m_cinfo.m_unreadMarker = 0;
            } /* end loop */
        }

        /// <summary>
        /// <para>
        /// Read a restart marker, which is expected to appear next in the datastream;
        /// if the marker is not there, take appropriate recovery action.
        /// Returns false if suspension is required.
        /// </para>
        /// <para>Made public for use by entropy decoder only</para>
        /// <para>
        /// This is called by the entropy decoder after it has read an appropriate
        /// number of MCUs.  cinfo.unread_marker may be nonzero if the entropy decoder
        /// has already read a marker from the data source.  Under normal conditions
        /// cinfo.unread_marker will be reset to 0 before returning; if not reset,
        /// it holds a marker which the decoder will be unable to read past.
        /// </para>
        /// </summary>
        public bool ReadRestartMarker()
        {
            /* Obtain a marker unless we already did. */
            /* Note that next_marker will complain if it skips any data. */
            if (m_cinfo.m_unreadMarker == 0)
            {
                if (!NextMarker())
                {
                    return false;
                }
            }

            if (m_cinfo.m_unreadMarker == ((int)JpegMarker.RST0 + m_cinfo.m_marker.m_next_restart_num))
            {
                /* Normal case --- swallow the marker and let entropy decoder continue */
                m_cinfo.TraceMS(3, JMessageCode.JTRC_RST, m_cinfo.m_marker.m_next_restart_num);
                m_cinfo.m_unreadMarker = 0;
            }
            else
            {
                /* Uh-oh, the restart markers have been messed up. */
                /* Let the data source manager determine how to resync. */
                if (!m_cinfo.m_src.ReSyncToRestart(m_cinfo, m_cinfo.m_marker.m_next_restart_num))
                {
                    return false;
                }
            }

            /* Update next-restart state */
            m_cinfo.m_marker.m_next_restart_num = (m_cinfo.m_marker.m_next_restart_num + 1) & 7;

            return true;
        }

        /// <summary>
        /// <para>
        /// Find the next JPEG marker, save it in cinfo.unread_marker.
        /// Returns false if had to suspend before reaching a marker;
        /// in that case cinfo.unread_marker is unchanged.
        /// </para>
        /// <para>
        /// Note that the result might not be a valid marker code,
        /// but it will never be 0 or FF.
        /// </para>
        /// </summary>
        public bool NextMarker()
        {
            int c;
            while (true)
            {
                if (!m_cinfo.m_src.GetByte(out c))
                {
                    return false;
                }

                /* Skip any non-FF bytes.
                 * This may look a bit inefficient, but it will not occur in a valid file.
                 * We sync after each discarded byte so that a suspending data source
                 * can discard the byte from its buffer.
                 */
                while (c != 0xFF)
                {
                    m_cinfo.m_marker.m_discarded_bytes++;
                    if (!m_cinfo.m_src.GetByte(out c))
                    {
                        return false;
                    }
                }

                /* This loop swallows any duplicate FF bytes.  Extra FFs are legal as
                 * pad bytes, so don't count them in discarded_bytes.  We assume there
                 * will not be so many consecutive FF bytes as to overflow a suspending
                 * data source's input buffer.
                 */
                do
                {
                    if (!m_cinfo.m_src.GetByte(out c))
                    {
                        return false;
                    }
                }
                while (c == 0xFF);

                if (c != 0)
                {
                    /* found a valid marker, exit loop */
                    break;
                }

                /* Reach here if we found a stuffed-zero data sequence (FF/00).
                 * Discard it and loop back to try again.
                 */
                m_cinfo.m_marker.m_discarded_bytes += 2;
            }

            if (m_cinfo.m_marker.m_discarded_bytes != 0)
            {
                m_cinfo.WarnMS(JMessageCode.JWRN_EXTRANEOUS_DATA, m_cinfo.m_marker.m_discarded_bytes, c);
                m_cinfo.m_marker.m_discarded_bytes = 0;
            }

            m_cinfo.m_unreadMarker = c;
            return true;
        }

        /// <summary>
        /// Install a special processing method for COM or APPn markers.
        /// </summary>
        public void JpegSetMarkerProcessor(int marker_code, JpegDecompressStruct.JpegMarkerParserMethod routine)
        {
            if (marker_code == (int)JpegMarker.COM)
            {
                m_process_COM = routine;
            }
            else if (marker_code >= (int)JpegMarker.APP0 && marker_code <= (int)JpegMarker.APP15)
            {
                m_process_APPn[marker_code - (int)JpegMarker.APP0] = routine;
            }
            else
            {
                m_cinfo.ErrExit(JMessageCode.JERR_UNKNOWN_MARKER, marker_code);
            }
        }

        public void JpegSaveMarkers(int marker_code, int length_limit)
        {
            /* Choose processor routine to use.
             * APP0/APP14 have special requirements.
             */
            JpegDecompressStruct.JpegMarkerParserMethod processor;
            if (length_limit != 0)
            {
                processor = SaveMarker;
                /* If saving APP0/APP14, save at least enough for our internal use. */
                if (marker_code == (int)JpegMarker.APP0 && length_limit < APP0_DATA_LEN)
                {
                    length_limit = APP0_DATA_LEN;
                }
                else if (marker_code == (int)JpegMarker.APP14 && length_limit < APP14_DATA_LEN)
                {
                    length_limit = APP14_DATA_LEN;
                }
            }
            else
            {
                processor = SkipVariable;
                /* If discarding APP0/APP14, use our regular on-the-fly processor. */
                if (marker_code == (int)JpegMarker.APP0 || marker_code == (int)JpegMarker.APP14)
                {
                    processor = GetInterestingAppN;
                }
            }

            if (marker_code == (int)JpegMarker.COM)
            {
                m_process_COM = processor;
                m_length_limit_COM = length_limit;
            }
            else if (marker_code >= (int)JpegMarker.APP0 && marker_code <= (int)JpegMarker.APP15)
            {
                m_process_APPn[marker_code - (int)JpegMarker.APP0] = processor;
                m_length_limit_APPn[marker_code - (int)JpegMarker.APP0] = length_limit;
            }
            else
            {
                m_cinfo.ErrExit(JMessageCode.JERR_UNKNOWN_MARKER, marker_code);
            }
        }

        /* State of marker reader, applications
        * supplying COM or APPn handlers might like to know the state.
        */
        public bool SawSOI()
        {
            return m_saw_SOI;
        }

        public bool SawSOF()
        {
            return m_saw_SOF;
        }

        public int NextRestartNumber()
        {
            return m_next_restart_num;
        }

        public int DiscardedByteCount()
        {
            return m_discarded_bytes;
        }

        public void SkipBytes(int count)
        {
            m_discarded_bytes += count;
        }

        /// <summary>
        /// Save an APPn or COM marker into the marker list
        /// </summary>
        private static bool SaveMarker(JpegDecompressStruct cinfo)
        {
            var cur_marker = cinfo.m_marker.m_cur_marker;
            var length = 0;
            int bytes_read;
            int data_length;
            var dataOffset = 0;

            byte[] data;
            if (cur_marker is null)
            {
                /* begin reading a marker */
                if (!cinfo.m_src.GetTwoBytes(out length))
                {
                    return false;
                }

                length -= 2;
                if (length >= 0)
                {
                    /* watch out for bogus length word */
                    /* figure out how much we want to save */
                    int limit;
                    if (cinfo.m_unreadMarker == (int)JpegMarker.COM)
                    {
                        limit = cinfo.m_marker.m_length_limit_COM;
                    }
                    else
                    {
                        limit = cinfo.m_marker.m_length_limit_APPn[cinfo.m_unreadMarker - (int)JpegMarker.APP0];
                    }

                    if (length < limit)
                    {
                        limit = length;
                    }

                    /* allocate and initialize the marker item */
                    cur_marker = new JpegMarkerStruct((byte)cinfo.m_unreadMarker, length, limit);

                    /* data area is just beyond the jpeg_marker_struct */
                    data = cur_marker.Data;
                    cinfo.m_marker.m_cur_marker = cur_marker;
                    cinfo.m_marker.m_bytes_read = 0;
                    bytes_read = 0;
                    data_length = limit;
                }
                else
                {
                    /* deal with bogus length word */
                    bytes_read = data_length = 0;
                    data = null;
                }
            }
            else
            {
                /* resume reading a marker */
                bytes_read = cinfo.m_marker.m_bytes_read;
                data_length = cur_marker.Data.Length;
                data = cur_marker.Data;
                dataOffset = bytes_read;
            }

            byte[] tempData = null;
            if (data_length != 0)
            {
                tempData = new byte[data.Length];
            }

            while (bytes_read < data_length)
            {
                /* move the restart point to here */
                cinfo.m_marker.m_bytes_read = bytes_read;

                /* If there's not at least one byte in buffer, suspend */
                if (!cinfo.m_src.MakeByteAvailable())
                {
                    return false;
                }

                /* Copy bytes with reasonable rapidity */
                var read = cinfo.m_src.GetBytes(tempData, data_length - bytes_read);
                Buffer.BlockCopy(tempData, 0, data, dataOffset, read);
                bytes_read += read;
                dataOffset += read;
            }

            /* Done reading what we want to read */
            if (cur_marker is object)
            {
                /* will be null if bogus length word */
                /* Add new marker to end of list */
                cinfo.m_marker_list.Add(cur_marker);

                /* Reset pointer & calc remaining data length */
                data = cur_marker.Data;
                dataOffset = 0;
                length = cur_marker.OriginalLength - data_length;
            }

            /* Reset to initial state for next marker */
            cinfo.m_marker.m_cur_marker = null;

            var currentMarker = (JpegMarker)cinfo.m_unreadMarker;
            if (data_length != 0 && (currentMarker == JpegMarker.APP0 || currentMarker == JpegMarker.APP14))
            {
                tempData = new byte[data.Length];
                Buffer.BlockCopy(data, dataOffset, tempData, 0, data.Length - dataOffset);
            }

            /* Process the marker if interesting; else just make a generic trace msg */
            switch ((JpegMarker)cinfo.m_unreadMarker)
            {
                case JpegMarker.APP0:
                ExamineApp0(cinfo, tempData, data_length, length);
                break;
                case JpegMarker.APP14:
                ExamineApp14(cinfo, tempData, data_length, length);
                break;
                default:
                cinfo.TraceMS(1, JMessageCode.JTRC_MISC_MARKER, cinfo.m_unreadMarker, data_length + length);
                break;
            }

            /* skip any remaining data -- could be lots */
            if (length > 0)
            {
                cinfo.m_src.SkipInputData(length);
            }

            return true;
        }

        /// <summary>
        /// Skip over an unknown or uninteresting variable-length marker
        /// </summary>
        private static bool SkipVariable(JpegDecompressStruct cinfo)
        {
            if (!cinfo.m_src.GetTwoBytes(out var length))
            {
                return false;
            }

            length -= 2;

            cinfo.TraceMS(1, JMessageCode.JTRC_MISC_MARKER, cinfo.m_unreadMarker, length);

            if (length > 0)
            {
                cinfo.m_src.SkipInputData(length);
            }

            return true;
        }

        /// <summary>
        /// Process an APP0 or APP14 marker without saving it
        /// </summary>
        private static bool GetInterestingAppN(JpegDecompressStruct cinfo)
        {
            if (!cinfo.m_src.GetTwoBytes(out var length))
            {
                return false;
            }

            length -= 2;

            /* get the interesting part of the marker data */
            var numtoread = 0;
            if (length >= APPN_DATA_LEN)
            {
                numtoread = APPN_DATA_LEN;
            }
            else if (length > 0)
            {
                numtoread = length;
            }

            var b = new byte[APPN_DATA_LEN];
            for (var i = 0; i < numtoread; i++)
            {
                if (!cinfo.m_src.GetByte(out var temp))
                {
                    return false;
                }

                b[i] = (byte)temp;
            }

            length -= numtoread;

            /* process it */
            switch ((JpegMarker)cinfo.m_unreadMarker)
            {
                case JpegMarker.APP0:
                ExamineApp0(cinfo, b, numtoread, length);
                break;
                case JpegMarker.APP14:
                ExamineApp14(cinfo, b, numtoread, length);
                break;
                default:
                /* can't get here unless jpeg_save_markers chooses wrong processor */
                cinfo.ErrExit(JMessageCode.JERR_UNKNOWN_MARKER, cinfo.m_unreadMarker);
                break;
            }

            /* skip any remaining data -- could be lots */
            if (length > 0)
            {
                cinfo.m_src.SkipInputData(length);
            }

            return true;
        }

        /*
         * Routines for processing APPn and COM markers.
         * These are either saved in memory or discarded, per application request.
         * APP0 and APP14 are specially checked to see if they are
         * JFIF and Adobe markers, respectively.
         */

        /// <summary>
        /// Examine first few bytes from an APP0.
        /// Take appropriate action if it is a JFIF marker.
        /// datalen is # of bytes at data[], remaining is length of rest of marker data.
        /// </summary>
        private static void ExamineApp0(JpegDecompressStruct cinfo, byte[] data, int datalen, int remaining)
        {
            var totallen = datalen + remaining;

            if (datalen >= APP0_DATA_LEN
                && data[0] == 0x4A
                && data[1] == 0x46
                && data[2] == 0x49
                && data[3] == 0x46
                && data[4] == 0)
            {
                /* Found JFIF APP0 marker: save info */
                cinfo.m_saw_JFIF_marker = true;
                cinfo.m_JFIF_major_version = data[5];
                cinfo.m_JFIF_minor_version = data[6];
                cinfo.m_densityUnit = (DensityUnit)data[7];
                cinfo.m_xDensity = (short)((data[8] << 8) + data[9]);
                cinfo.m_yDensity = (short)((data[10] << 8) + data[11]);

                /* Check version.
                 * Major version must be 1 or 2, anything else signals an incompatible change.
                 * (We used to treat this as an error, but now it's a nonfatal warning,
                 * because some bozo at Hijaak couldn't read the spec.)
                 * Minor version should be 0..2, but process anyway if newer.
                 */
                if (cinfo.m_JFIF_major_version != 1 && cinfo.m_JFIF_major_version != 2)
                {
                    cinfo.WarnMS(JMessageCode.JWRN_JFIF_MAJOR, cinfo.m_JFIF_major_version, cinfo.m_JFIF_minor_version);
                }

                /* Generate trace messages */
                cinfo.TraceMS(1, JMessageCode.JTRC_JFIF, cinfo.m_JFIF_major_version, cinfo.m_JFIF_minor_version, cinfo.m_xDensity,
                                cinfo.m_yDensity, cinfo.m_densityUnit);

                /* Validate thumbnail dimensions and issue appropriate messages */
                if ((data[12] | data[13]) != 0)
                {
                    cinfo.TraceMS(1, JMessageCode.JTRC_JFIF_THUMBNAIL, data[12], data[13]);
                }

                totallen -= APP0_DATA_LEN;
                if (totallen != (data[12] * data[13] * 3))
                {
                    cinfo.TraceMS(1, JMessageCode.JTRC_JFIF_BADTHUMBNAILSIZE, totallen);
                }
            }
            else if (datalen >= 6 && data[0] == 0x4A && data[1] == 0x46 && data[2] == 0x58 && data[3] == 0x58 && data[4] == 0)
            {
                /* Found JFIF "JFXX" extension APP0 marker */
                /* The library doesn't actually do anything with these,
                 * but we try to produce a helpful trace message.
                 */
                switch (data[5])
                {
                    case 0x10:
                    cinfo.TraceMS(1, JMessageCode.JTRC_THUMB_JPEG, totallen);
                    break;
                    case 0x11:
                    cinfo.TraceMS(1, JMessageCode.JTRC_THUMB_PALETTE, totallen);
                    break;
                    case 0x13:
                    cinfo.TraceMS(1, JMessageCode.JTRC_THUMB_RGB, totallen);
                    break;
                    default:
                    cinfo.TraceMS(1, JMessageCode.JTRC_JFIF_EXTENSION, data[5], totallen);
                    break;
                }
            }
            else
            {
                /* Start of APP0 does not match "JFIF" or "JFXX", or too short */
                cinfo.TraceMS(1, JMessageCode.JTRC_APP0, totallen);
            }
        }

        /// <summary>
        /// Examine first few bytes from an APP14.
        /// Take appropriate action if it is an Adobe marker.
        /// datalen is # of bytes at data[], remaining is length of rest of marker data.
        /// </summary>
        private static void ExamineApp14(JpegDecompressStruct cinfo, byte[] data, int datalen, int remaining)
        {
            if (datalen >= APP14_DATA_LEN
                && data[0] == 0x41
                && data[1] == 0x64
                && data[2] == 0x6F
                && data[3] == 0x62
                && data[4] == 0x65)
            {
                /* Found Adobe APP14 marker */
                var version = (data[5] << 8) + data[6];
                var flags0 = (data[7] << 8) + data[8];
                var flags1 = (data[9] << 8) + data[10];
                int transform = data[11];
                cinfo.TraceMS(1, JMessageCode.JTRC_ADOBE, version, flags0, flags1, transform);
                cinfo.m_saw_Adobe_marker = true;
                cinfo.m_Adobe_transform = (byte)transform;
            }
            else
            {
                /* Start of APP14 does not match "Adobe", or too short */
                cinfo.TraceMS(1, JMessageCode.JTRC_APP14, datalen + remaining);
            }
        }

        /*
         * Routines to process JPEG markers.
         *
         * Entry condition: JPEG marker itself has been read and its code saved
         *   in cinfo.unread_marker; input restart point is just after the marker.
         *
         * Exit: if return true, have read and processed any parameters, and have
         *   updated the restart point to point after the parameters.
         *   If return false, was forced to suspend before reaching end of
         *   marker parameters; restart point has not been moved.  Same routine
         *   will be called again after application supplies more input data.
         *
         * This approach to suspension assumes that all of a marker's parameters
         * can fit into a single input bufferload.  This should hold for "normal"
         * markers.  Some COM/APPn markers might have large parameter segments
         * that might not fit.  If we are simply dropping such a marker, we use
         * skip_input_data to get past it, and thereby put the problem on the
         * source manager's shoulders.  If we are saving the marker's contents
         * into memory, we use a slightly different convention: when forced to
         * suspend, the marker processor updates the restart point to the end of
         * what it's consumed (ie, the end of the buffer) before returning false.
         * On resumption, cinfo.unread_marker still contains the marker code,
         * but the data source will point to the next chunk of marker data.
         * The marker processor must retain internal state to deal with this.
         *
         * Note that we don't bother to avoid duplicate trace messages if a
         * suspension occurs within marker parameters.  Other side effects
         * require more care.
         */

        /// <summary>
        /// Process an SOI marker
        /// </summary>
        private void GetSOI()
        {
            m_cinfo.TraceMS(1, JMessageCode.JTRC_SOI);

            if (m_cinfo.m_marker.m_saw_SOI)
            {
                m_cinfo.ErrExit(JMessageCode.JERR_SOI_DUPLICATE);
            }

            /* Reset all parameters that are defined to be reset by SOI */
            m_cinfo.m_restart_interval = 0;

            /* Set initial assumptions for colorspace etc */

            m_cinfo.jpegColorSpace = JColorSpace.JCS_UNKNOWN;
            m_cinfo.color_transform = JColorTransform.JCT_NONE;
            m_cinfo.m_CCIR601_sampling = false; /* Assume non-CCIR sampling??? */

            m_cinfo.m_saw_JFIF_marker = false;
            m_cinfo.m_JFIF_major_version = 1; /* set default JFIF APP0 values */
            m_cinfo.m_JFIF_minor_version = 1;
            m_cinfo.m_densityUnit = DensityUnit.Unknown;
            m_cinfo.m_xDensity = 1;
            m_cinfo.m_yDensity = 1;
            m_cinfo.m_saw_Adobe_marker = false;
            m_cinfo.m_Adobe_transform = 0;

            m_cinfo.m_marker.m_saw_SOI = true;
        }

        /// <summary>
        /// Process a SOFn marker
        /// </summary>
        private bool GetSOF(bool is_baseline, bool is_prog, bool is_arith)
        {
            m_cinfo.isBaseline = is_baseline;
            m_cinfo.progressiveMode = is_prog;
            m_cinfo.arithCode = is_arith;

            if (!m_cinfo.m_src.GetTwoBytes(out var length))
            {
                return false;
            }

            if (!m_cinfo.m_src.GetByte(out m_cinfo.m_dataPrecision))
            {
                return false;
            }

            if (!m_cinfo.m_src.GetTwoBytes(out var temp))
            {
                return false;
            }

            m_cinfo.imageHeight = temp;

            if (!m_cinfo.m_src.GetTwoBytes(out temp))
            {
                return false;
            }

            m_cinfo.imageWidth = temp;

            if (!m_cinfo.m_src.GetByte(out m_cinfo.numComponents))
            {
                return false;
            }

            length -= 8;

            m_cinfo.TraceMS(1, JMessageCode.JTRC_SOF, m_cinfo.m_unreadMarker,
                m_cinfo.imageWidth, m_cinfo.imageHeight, m_cinfo.numComponents);

            if (m_cinfo.m_marker.m_saw_SOF)
            {
                m_cinfo.ErrExit(JMessageCode.JERR_SOF_DUPLICATE);
            }

            /* We don't support files in which the image height is initially specified */
            /* as 0 and is later redefined by DNL.  As long as we have to check that,  */
            /* might as well have a general sanity check. */
            if (m_cinfo.imageHeight <= 0 || m_cinfo.imageWidth <= 0 || m_cinfo.numComponents <= 0)
            {
                m_cinfo.ErrExit(JMessageCode.JERR_EMPTY_IMAGE);
            }

            if (length != (m_cinfo.numComponents * 3))
            {
                m_cinfo.ErrExit(JMessageCode.JERR_BAD_LENGTH);
            }

            if (m_cinfo.CompInfo is null)
            {
                /* do only once, even if suspend */
                m_cinfo.CompInfo = JpegComponentInfo.CreateArrayOfComponents(m_cinfo.numComponents);
            }

            for (var ci = 0; ci < m_cinfo.numComponents; ci++)
            {
                //jpeg_component_info compptr = m_cinfo.Comp_info[ci];

                if (!m_cinfo.m_src.GetByte(out var c))
                {
                    return false;
                }

                /* Check to see whether component id has already been seen   */
                /* (in violation of the spec, but unfortunately seen in some */
                /* files).  If so, create "fake" component id equal to the   */
                /* max id seen so far + 1. */
                var componentInfoIndex = 0;
                JpegComponentInfo compptr;
                for (var i = 0; i < ci; i++, componentInfoIndex++)
                {
                    compptr = m_cinfo.CompInfo[componentInfoIndex];
                    if (c == compptr.Component_id)
                    {
                        componentInfoIndex = 0;
                        compptr = m_cinfo.CompInfo[componentInfoIndex];
                        c = compptr.Component_id;

                        componentInfoIndex++;
                        _ = m_cinfo.CompInfo[componentInfoIndex];

                        for (i = 1; i < ci; i++, componentInfoIndex++)
                        {
                            compptr = m_cinfo.CompInfo[componentInfoIndex];
                            if (compptr.Component_id > c)
                            {
                                c = compptr.Component_id;
                            }
                        }

                        c++;
                        break;
                    }
                }

                compptr = m_cinfo.CompInfo[componentInfoIndex];
                compptr.Component_id = c;
                compptr.Component_index = ci;

                if (!m_cinfo.m_src.GetByte(out c))
                {
                    return false;
                }

                compptr.H_samp_factor = (c >> 4) & 15;
                compptr.V_samp_factor = c & 15;

                if (!m_cinfo.m_src.GetByte(out var quant_tbl_no))
                {
                    return false;
                }

                compptr.Quant_tbl_no = quant_tbl_no;

                m_cinfo.TraceMS(1, JMessageCode.JTRC_SOF_COMPONENT, compptr.Component_id,
                    compptr.H_samp_factor, compptr.V_samp_factor,
                    compptr.Quant_tbl_no);
            }

            m_cinfo.m_marker.m_saw_SOF = true;
            return true;
        }

        /// <summary>
        /// Process a SOS marker
        /// </summary>
        private bool GetSOS()
        {
            if (!m_cinfo.m_marker.m_saw_SOF)
            {
                m_cinfo.ErrExit(JMessageCode.JERR_SOF_BEFORE, "SOS");
            }

            if (!m_cinfo.m_src.GetTwoBytes(out var length))
            {
                return false;
            }

            /* Number of components */
            if (!m_cinfo.m_src.GetByte(out var n))
            {
                return false;
            }

            m_cinfo.TraceMS(1, JMessageCode.JTRC_SOS, n);

            if (length != ((n * 2) + 6)
                || n > JpegConstants.MAX_COMPS_IN_SCAN
                || (n == 0
                    && m_cinfo.progressiveMode))
            {
                /* pseudo SOS marker only allowed in progressive mode */
                m_cinfo.ErrExit(JMessageCode.JERR_BAD_LENGTH);
            }

            m_cinfo.m_comps_in_scan = n;

            /* Collect the component-spec parameters */

            for (var i = 0; i < n; i++)
            {
                if (!m_cinfo.m_src.GetByte(out var c))
                {
                    return false;
                }

                /* Detect the case where component id's are not unique, and, if so, */
                /* create a fake component id using the same logic as in get_sof.   */
                /* Note:  This also ensures that all of the SOF components are      */
                /* referenced in the single scan case, which prevents access to     */
                /* uninitialized memory in later decoding stages. */
                for (var ci = 0; ci < i; ci++)
                {
                    var componentInfo = m_cinfo.CompInfo[m_cinfo.m_cur_comp_info[ci]];
                    if (c == componentInfo.Component_id)
                    {
                        componentInfo = m_cinfo.CompInfo[m_cinfo.m_cur_comp_info[0]];
                        c = componentInfo.Component_id;
                        for (ci = 1; ci < i; ci++)
                        {
                            componentInfo = m_cinfo.CompInfo[m_cinfo.m_cur_comp_info[ci]];
                            if (componentInfo.Component_id > c)
                            {
                                c = componentInfo.Component_id;
                            }
                        }

                        c++;
                        break;
                    }
                }

                var idFound = false;
                var foundIndex = -1;
                for (var ci = 0; ci < m_cinfo.numComponents; ci++)
                {
                    if (c == m_cinfo.CompInfo[ci].Component_id)
                    {
                        foundIndex = ci;
                        idFound = true;
                        break;
                    }
                }

                if (!idFound)
                {
                    m_cinfo.ErrExit(JMessageCode.JERR_BAD_COMPONENT_ID, c);
                }

                m_cinfo.m_cur_comp_info[i] = foundIndex;

                if (!m_cinfo.m_src.GetByte(out c))
                {
                    return false;
                }

                m_cinfo.CompInfo[foundIndex].Dc_tbl_no = (c >> 4) & 15;
                m_cinfo.CompInfo[foundIndex].Ac_tbl_no = c & 15;

                m_cinfo.TraceMS(1, JMessageCode.JTRC_SOS_COMPONENT,
                    m_cinfo.CompInfo[foundIndex].Component_id,
                    m_cinfo.CompInfo[foundIndex].Dc_tbl_no,
                    m_cinfo.CompInfo[foundIndex].Ac_tbl_no);
            }

            /* Collect the additional scan parameters Ss, Se, Ah/Al. */
            if (!m_cinfo.m_src.GetByte(out var temp))
            {
                return false;
            }

            m_cinfo.m_Ss = temp;
            if (!m_cinfo.m_src.GetByte(out temp))
            {
                return false;
            }

            m_cinfo.m_Se = temp;
            if (!m_cinfo.m_src.GetByte(out temp))
            {
                return false;
            }

            m_cinfo.m_Ah = (temp >> 4) & 15;
            m_cinfo.m_Al = temp & 15;

            m_cinfo.TraceMS(1, JMessageCode.JTRC_SOS_PARAMS, m_cinfo.m_Ss, m_cinfo.m_Se, m_cinfo.m_Ah, m_cinfo.m_Al);

            /* Prepare to scan data & restart markers */
            m_cinfo.m_marker.m_next_restart_num = 0;

            /* Count another (non-pseudo) SOS marker */
            if (n != 0)
            {
                m_cinfo.inputScanNumber++;
            }

            return true;
        }

        /// <summary>
        /// Process a DAC marker
        /// </summary>
        private bool GetDAC()
        {
            if (!m_cinfo.m_src.GetTwoBytes(out var length))
            {
                return false;
            }

            length -= 2;
            while (length > 0)
            {
                if (!m_cinfo.m_src.GetByte(out var index))
                {
                    return false;
                }

                if (!m_cinfo.m_src.GetByte(out var val))
                {
                    return false;
                }

                length -= 2;

                m_cinfo.TraceMS(1, JMessageCode.JTRC_DAC, index, val);

                if (index < 0 || index >= (2 * JpegConstants.NUM_ARITH_TBLS))
                {
                    m_cinfo.ErrExit(JMessageCode.JERR_DAC_INDEX, index);
                }

                if (index >= JpegConstants.NUM_ARITH_TBLS)
                { /* define AC table */
                    m_cinfo.arithAcK[index - JpegConstants.NUM_ARITH_TBLS] = (byte)val;
                }
                else
                {          /* define DC table */
                    m_cinfo.arithDcL[index] = (byte)(val & 0x0F);
                    m_cinfo.arithDcU[index] = (byte)(val >> 4);
                    if (m_cinfo.arithDcL[index] > m_cinfo.arithDcU[index])
                    {
                        m_cinfo.ErrExit(JMessageCode.JERR_DAC_VALUE, val);
                    }
                }
            }

            if (length != 0)
            {
                m_cinfo.ErrExit(JMessageCode.JERR_BAD_LENGTH);
            }

            return true;
        }

        /// <summary>
        /// Process a DHT marker
        /// </summary>
        private bool GetDHT()
        {
            if (!m_cinfo.m_src.GetTwoBytes(out var length))
            {
                return false;
            }

            length -= 2;

            var bits = new byte[17];
            var huffval = new byte[256];
            while (length > 16)
            {
                if (!m_cinfo.m_src.GetByte(out var index))
                {
                    return false;
                }

                m_cinfo.TraceMS(1, JMessageCode.JTRC_DHT, index);

                bits[0] = 0;
                var count = 0;
                for (var i = 1; i <= 16; i++)
                {
                    if (!m_cinfo.m_src.GetByte(out var temp))
                    {
                        return false;
                    }

                    bits[i] = (byte)temp;
                    count += bits[i];
                }

                length -= 1 + 16;

                m_cinfo.TraceMS(2, JMessageCode.JTRC_HUFFBITS, bits[1], bits[2], bits[3], bits[4], bits[5], bits[6], bits[7], bits[8]);
                m_cinfo.TraceMS(2, JMessageCode.JTRC_HUFFBITS, bits[9], bits[10], bits[11], bits[12], bits[13], bits[14], bits[15], bits[16]);

                /* Here we just do minimal validation of the counts to avoid walking
                 * off the end of our table space. huff_entropy_decoder will check more carefully.
                 */
                if (count > 256 || count > length)
                {
                    m_cinfo.ErrExit(JMessageCode.JERR_BAD_HUFF_TABLE);
                }

                for (var i = 0; i < count; i++)
                {
                    if (!m_cinfo.m_src.GetByte(out var temp))
                    {
                        return false;
                    }

                    huffval[i] = (byte)temp;
                }

                length -= count;

                JHuffmanTable htblptr;
                if ((index & 0x10) != 0)
                {
                    /* AC table definition */
                    index -= 0x10;
                    if (m_cinfo.m_ac_huff_tbl_ptrs[index] is null)
                    {
                        m_cinfo.m_ac_huff_tbl_ptrs[index] = new JHuffmanTable();
                    }

                    htblptr = m_cinfo.m_ac_huff_tbl_ptrs[index];
                }
                else
                {
                    /* DC table definition */
                    if (m_cinfo.m_dc_huff_tbl_ptrs[index] is null)
                    {
                        m_cinfo.m_dc_huff_tbl_ptrs[index] = new JHuffmanTable();
                    }

                    htblptr = m_cinfo.m_dc_huff_tbl_ptrs[index];
                }

                if (index < 0 || index >= JpegConstants.NUM_HUFF_TBLS)
                {
                    m_cinfo.ErrExit(JMessageCode.JERR_DHT_INDEX, index);
                }

                Buffer.BlockCopy(bits, 0, htblptr.Bits, 0, htblptr.Bits.Length);
                Buffer.BlockCopy(huffval, 0, htblptr.Huffval, 0, htblptr.Huffval.Length);
            }

            if (length != 0)
            {
                m_cinfo.ErrExit(JMessageCode.JERR_BAD_LENGTH);
            }

            return true;
        }

        /// <summary>
        /// Process a DQT marker
        /// </summary>
        private bool GetDQT()
        {
            if (!m_cinfo.m_src.GetTwoBytes(out var length))
            {
                return false;
            }

            length -= 2;
            while (length > 0)
            {
                length--;

                if (!m_cinfo.m_src.GetByte(out var n))
                {
                    return false;
                }

                var prec = n >> 4;
                n &= 0x0F;

                m_cinfo.TraceMS(1, JMessageCode.JTRC_DQT, n, prec);

                if (n >= JpegConstants.NUM_QUANT_TBLS)
                {
                    m_cinfo.ErrExit(JMessageCode.JERR_DQT_INDEX, n);
                }

                if (m_cinfo.m_quant_tbl_ptrs[n] is null)
                {
                    m_cinfo.m_quant_tbl_ptrs[n] = new JQuantTable();
                }

                var quant_ptr = m_cinfo.m_quant_tbl_ptrs[n];

                int count;
                if (prec != 0)
                {
                    if (length < JpegConstants.DCTSIZE2 * 2)
                    {
                        /* Initialize full table for safety. */
                        for (var i = 0; i < JpegConstants.DCTSIZE2; i++)
                        {
                            quant_ptr.quantBal[i] = 1;
                        }

                        count = length >> 1;
                    }
                    else
                    {
                        count = JpegConstants.DCTSIZE2;
                    }
                }
                else if (length < JpegConstants.DCTSIZE2)
                {
                    /* Initialize full table for safety. */
                    for (var i = 0; i < JpegConstants.DCTSIZE2; i++)
                    {
                        quant_ptr.quantBal[i] = 1;
                    }

                    count = length;
                }
                else
                {
                    count = JpegConstants.DCTSIZE2;
                }

                var natural_order = count switch
                {
                    (2 * 2) => JpegUtils.jpeg_natural_order2,
                    (3 * 3) => JpegUtils.jpeg_natural_order3,
                    (4 * 4) => JpegUtils.jpeg_natural_order4,
                    (5 * 5) => JpegUtils.jpeg_natural_order5,
                    (6 * 6) => JpegUtils.jpeg_natural_order6,
                    (7 * 7) => JpegUtils.jpeg_natural_order7,
                    _ => JpegUtils.jpeg_natural_order,
                };
                for (var i = 0; i < count; i++)
                {
                    int tmp;
                    if (prec != 0)
                    {
                        if (!m_cinfo.m_src.GetTwoBytes(out var temp))
                        {
                            return false;
                        }

                        tmp = temp;
                    }
                    else
                    {
                        if (!m_cinfo.m_src.GetByte(out var temp))
                        {
                            return false;
                        }

                        tmp = temp;
                    }

                    /* We convert the zigzag-order table to natural array order. */
                    quant_ptr.quantBal[natural_order[i]] = (short)tmp;
                }

                if (m_cinfo.jpgError.m_traceLevel >= 2)
                {
                    for (var i = 0; i < JpegConstants.DCTSIZE2; i += 8)
                    {
                        m_cinfo.TraceMS(2, JMessageCode.JTRC_QUANTVALS, quant_ptr.quantBal[i],
                            quant_ptr.quantBal[i + 1], quant_ptr.quantBal[i + 2],
                            quant_ptr.quantBal[i + 3], quant_ptr.quantBal[i + 4],
                            quant_ptr.quantBal[i + 5], quant_ptr.quantBal[i + 6], quant_ptr.quantBal[i + 7]);
                    }
                }

                length -= count;
                if (prec != 0)
                {
                    length -= count;
                }
            }

            if (length != 0)
            {
                m_cinfo.ErrExit(JMessageCode.JERR_BAD_LENGTH);
            }

            return true;
        }

        /// <summary>
        /// Process a DRI marker
        /// </summary>
        private bool GetDRI()
        {
            if (!m_cinfo.m_src.GetTwoBytes(out var length))
            {
                return false;
            }

            if (length != 4)
            {
                m_cinfo.ErrExit(JMessageCode.JERR_BAD_LENGTH);
            }

            if (!m_cinfo.m_src.GetTwoBytes(out var temp))
            {
                return false;
            }

            var tmp = temp;
            m_cinfo.TraceMS(1, JMessageCode.JTRC_DRI, tmp);
            m_cinfo.m_restart_interval = tmp;

            return true;
        }

        /// <summary>
        /// Process an LSE marker
        /// </summary>
        private bool GetLSE()
        {
            if (!m_cinfo.m_marker.m_saw_SOF)
            {
                m_cinfo.ErrExit(JMessageCode.JERR_SOF_BEFORE, "LSE");
            }

            if (m_cinfo.numComponents < 3)
            {
                m_cinfo.ErrExit(JMessageCode.JERR_CONVERSION_NOTIMPL);
            }

            if (!m_cinfo.m_src.GetTwoBytes(out var length))
            {
                return false;
            }

            if (length != 24)
            {
                m_cinfo.ErrExit(JMessageCode.JERR_BAD_LENGTH);
            }

            if (!m_cinfo.m_src.GetByte(out var tmp))
            {
                return false;
            }

            if (tmp != 0x0D)
            {
                /* ID inverse transform specification */
                m_cinfo.ErrExit(JMessageCode.JERR_UNKNOWN_MARKER, m_cinfo.m_unreadMarker);
            }

            if (!m_cinfo.m_src.GetTwoBytes(out tmp))
            {
                return false;
            }

            if (tmp != JpegConstants.MAXJSAMPLE)
            {
                /* MAXTRANS */
                m_cinfo.ErrExit(JMessageCode.JERR_CONVERSION_NOTIMPL);
            }

            if (!m_cinfo.m_src.GetByte(out tmp))
            {
                return false;
            }

            if (tmp != 3)
            {
                /* Nt=3 */
                m_cinfo.ErrExit(JMessageCode.JERR_CONVERSION_NOTIMPL);
            }

            if (!m_cinfo.m_src.GetByte(out var cid))
            {
                return false;
            }

            if (cid != m_cinfo.CompInfo[1].Component_id)
            {
                m_cinfo.ErrExit(JMessageCode.JERR_CONVERSION_NOTIMPL);
            }

            if (!m_cinfo.m_src.GetByte(out cid))
            {
                return false;
            }

            if (cid != m_cinfo.CompInfo[0].Component_id)
            {
                m_cinfo.ErrExit(JMessageCode.JERR_CONVERSION_NOTIMPL);
            }

            if (!m_cinfo.m_src.GetByte(out cid))
            {
                return false;
            }

            if (cid != m_cinfo.CompInfo[2].Component_id)
            {
                m_cinfo.ErrExit(JMessageCode.JERR_CONVERSION_NOTIMPL);
            }

            if (!m_cinfo.m_src.GetByte(out tmp))
            {
                return false;
            }

            if (tmp != 0x80)
            {
                /* F1: CENTER1=1, NORM1=0 */
                m_cinfo.ErrExit(JMessageCode.JERR_CONVERSION_NOTIMPL);
            }

            if (!m_cinfo.m_src.GetTwoBytes(out tmp))
            {
                return false;
            }

            if (tmp != 0)
            {
                /* A(1,1)=0 */
                m_cinfo.ErrExit(JMessageCode.JERR_CONVERSION_NOTIMPL);
            }

            if (!m_cinfo.m_src.GetTwoBytes(out tmp))
            {
                return false;
            }

            if (tmp != 0)
            {
                /* A(1,2)=0 */
                m_cinfo.ErrExit(JMessageCode.JERR_CONVERSION_NOTIMPL);
            }

            if (!m_cinfo.m_src.GetByte(out tmp))
            {
                return false;
            }

            if (tmp != 0)
            {
                /* F2: CENTER2=0, NORM2=0 */
                m_cinfo.ErrExit(JMessageCode.JERR_CONVERSION_NOTIMPL);
            }

            if (!m_cinfo.m_src.GetTwoBytes(out tmp))
            {
                return false;
            }

            if (tmp != 1)
            {
                /* A(2,1)=1 */
                m_cinfo.ErrExit(JMessageCode.JERR_CONVERSION_NOTIMPL);
            }

            if (!m_cinfo.m_src.GetTwoBytes(out tmp))
            {
                return false;
            }

            if (tmp != 0)
            {
                /* A(2,2)=0 */
                m_cinfo.ErrExit(JMessageCode.JERR_CONVERSION_NOTIMPL);
            }

            if (!m_cinfo.m_src.GetByte(out tmp))
            {
                return false;
            }

            if (tmp != 0)
            {
                /* F3: CENTER3=0, NORM3=0 */
                m_cinfo.ErrExit(JMessageCode.JERR_CONVERSION_NOTIMPL);
            }

            if (!m_cinfo.m_src.GetTwoBytes(out tmp))
            {
                return false;
            }

            if (tmp != 1)
            {
                /* A(3,1)=1 */
                m_cinfo.ErrExit(JMessageCode.JERR_CONVERSION_NOTIMPL);
            }

            if (!m_cinfo.m_src.GetTwoBytes(out tmp))
            {
                return false;
            }

            if (tmp != 0)
            {
                /* A(3,2)=0 */
                m_cinfo.ErrExit(JMessageCode.JERR_CONVERSION_NOTIMPL);
            }

            /* OK, valid transform that we can handle. */
            m_cinfo.color_transform = JColorTransform.JCT_SUBTRACT_GREEN;
            return true;
        }

        /// <summary>
        /// Like next_marker, but used to obtain the initial SOI marker.
        /// For this marker, we do not allow preceding garbage or fill; otherwise,
        /// we might well scan an entire input file before realizing it ain't JPEG.
        /// If an application wants to process non-JFIF files, it must seek to the
        /// SOI before calling the JPEG library.
        /// </summary>
        private bool FirstMarker()
        {
            if (!m_cinfo.m_src.GetByte(out var c))
            {
                return false;
            }

            if (!m_cinfo.m_src.GetByte(out var c2))
            {
                return false;
            }

            if (c != 0xFF || c2 != (int)JpegMarker.SOI)
            {
                m_cinfo.ErrExit(JMessageCode.JERR_NO_SOI, c, c2);
            }

            m_cinfo.m_unreadMarker = c2;
            return true;
        }
    }
}
