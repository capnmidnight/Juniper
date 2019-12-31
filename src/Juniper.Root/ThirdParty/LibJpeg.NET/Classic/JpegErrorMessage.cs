/*
 * This file contains simple error-reporting and trace-message routines.
 * Many applications will want to override some or all of these routines.
 *
 * These routines are used by both the compression and decompression code.
 */

using System;
using System.Globalization;

#if NETSTANDARD
using Console = System.Diagnostics.Debug;
#endif

namespace BitMiracle.LibJpeg.Classic
{
    /// <summary>
    /// Contains simple error-reporting and trace-message routines.
    /// </summary>
    /// <remarks>This class is used by both the compression and decompression code.</remarks>
    /// <seealso href="41dc1a3b-0dea-4594-87d2-c213ab1049e1.htm" target="_self">Error handling</seealso>
    public class JpegErrorMessage
    {
        // The message ID code and any parameters are saved in fields below. 
        internal int msgCode;
        internal object[] msgParam;

        internal int m_traceLevel;
        internal int m_numWarnings;

        /// <summary>
        /// Initializes a new instance of the <see cref="JpegErrorMessage"/> class.
        /// </summary>
        public JpegErrorMessage()
        {
        }

        /// <summary>
        /// Gets or sets the maximum message level that will be displayed.
        /// </summary>
        /// <value>Values are:
        /// -1: recoverable corrupt-data warning, may want to abort.<br/>
        /// 0: important advisory messages (always display to user).<br/>
        /// 1: first level of tracing detail.<br/>
        /// 2, 3, ...: successively more detailed tracing messages.
        /// </value>
        /// <seealso cref="JpegErrorMessage.EmitMessage"/>
        public int TraceLevel
        {
            get { return m_traceLevel; }
            set { m_traceLevel = value; }
        }

        /// <summary>
        /// Gets the number of corrupt-data warnings.
        /// </summary>
        /// <value>The num_warnings.</value>
        /// <remarks>For recoverable corrupt-data errors, we emit a warning message, but keep going 
        /// unless <see cref="JpegErrorMessage.EmitMessage">emit_message</see> chooses to abort. 
        /// <c>emit_message</c> should count warnings in <c>Num_warnings</c>. The surrounding application 
        /// can check for bad data by seeing if <c>Num_warnings</c> is nonzero at the end of processing.</remarks>
        public int NumWarnings
        {
            get { return m_numWarnings; }
        }

        /// <summary>
        /// Receives control for a fatal error.
        /// </summary>
        /// <remarks>This method calls <see cref="JpegErrorMessage.OutputMessage">output_message</see> 
        /// and then throws an exception.</remarks>
        /// <seealso href="41dc1a3b-0dea-4594-87d2-c213ab1049e1.htm" target="_self">Error handling</seealso>
        public virtual void ErrorExit()
        {
            // Always display the message
            OutputMessage();

            var buffer = FormatMessage();
            throw new Exception(buffer);
        }

        /// <summary>
        /// Conditionally emit a trace or warning message.
        /// </summary>
        /// <param name="msg_level">The message severity level.<br/>
        /// Values are:<br/>
        /// -1: recoverable corrupt-data warning, may want to abort.<br/>
        /// 0: important advisory messages (always display to user).<br/>
        /// 1: first level of tracing detail.<br/>
        /// 2, 3, ...: successively more detailed tracing messages.
        /// </param>
        /// <remarks><para>
        /// The main reason for overriding this method would be to abort on warnings.
        /// This method calls <see cref="JpegErrorMessage.OutputMessage">output_message</see> for message showing.<br/>
        /// </para>
        /// <para>
        /// An application might override this method if it wanted to abort on 
        /// warnings or change the policy about which messages to display.
        /// </para>
        /// </remarks>
        /// <seealso href="41dc1a3b-0dea-4594-87d2-c213ab1049e1.htm" target="_self">Error handling</seealso>
        public virtual void EmitMessage(int msg_level)
        {
            if (msg_level < 0)
            {
                /* It's a warning message.  Since corrupt files may generate many warnings,
                 * the policy implemented here is to show only the first warning,
                 * unless trace_level >= 3.
                 */
                if (m_numWarnings == 0 || m_traceLevel >= 3)
                {
                    OutputMessage();
                }

                /* Always count warnings in num_warnings. */
                m_numWarnings++;
            }
            else
            {
                /* It's a trace message.  Show it if trace_level >= msg_level. */
                if (m_traceLevel >= msg_level)
                {
                    OutputMessage();
                }
            }
        }

        /// <summary>
        /// Actual output of any JPEG message.
        /// </summary>
        /// <remarks>Override this to send messages somewhere other than Console. 
        /// Note that this method does not know how to generate a message, only where to send it.
        /// For extending a generation of messages see <see cref="JpegErrorMessage.FormatMessage">format_message</see>.
        /// </remarks>
        /// <seealso href="41dc1a3b-0dea-4594-87d2-c213ab1049e1.htm" target="_self">Error handling</seealso>
        public virtual void OutputMessage()
        {
            // Create the message
            var buffer = FormatMessage();

            // Send it to console, adding a newline */
            Console.WriteLine(buffer);
        }

        /// <summary>
        /// Constructs a readable error message string.
        /// </summary>
        /// <remarks>This method is called by <see cref="JpegErrorMessage.OutputMessage">output_message</see>.
        /// Few applications should need to override this method. One possible reason for doing so is to 
        /// implement dynamic switching of error message language.</remarks>
        /// <returns>The formatted message</returns>
        /// <seealso href="41dc1a3b-0dea-4594-87d2-c213ab1049e1.htm" target="_self">Error handling</seealso>
        public virtual string FormatMessage()
        {
            var msgtext = GetMessageText(msgCode);
            if (msgtext == null)
            {
                msgParam = new object[] { msgCode };
                msgtext = GetMessageText(0);
            }

            /* Format the message into the passed buffer */
            return string.Format(CultureInfo.CurrentCulture, msgtext, msgParam);
        }

        /// <summary>
        /// Resets error manager to initial state.
        /// </summary>
        /// <remarks>This is called during compression startup to reset trace/error
        /// processing to default state. An application might possibly want to
        /// override this method if it has additional error processing state.
        /// </remarks>
        public virtual void ResetErrorMessage()
        {
            m_numWarnings = 0;

            /* trace_level is not reset since it is an application-supplied parameter */

            // may be useful as a flag for "no error"
            msgCode = 0;
        }

        /// <summary>
        /// Gets the actual message texts.
        /// </summary>
        /// <param name="code">The message code. See <see cref="JMessageCode"/> for details.</param>
        /// <returns>The message text associated with <c>code</c>.</returns>
        /// <remarks>It may be useful for an application to add its own message texts that are handled 
        /// by the same mechanism. You can override <c>GetMessageText</c> for this purpose. If you number 
        /// the addon messages beginning at 1000 or so, you won't have to worry about conflicts 
        /// with the library's built-in messages.
        /// </remarks>
        /// <seealso cref="JMessageCode"/>
        /// <seealso href="41dc1a3b-0dea-4594-87d2-c213ab1049e1.htm" target="_self">Error handling</seealso>
        protected virtual string GetMessageText(int code)
        {
            return code switch
            {
                /* For maintenance convenience, list is alphabetical by message code name */
                JMessageCode.JERR_BAD_BUFFER_MODE => "Bogus buffer control mode",
                JMessageCode.JERR_BAD_COMPONENT_ID => "Invalid component ID {0} in SOS",
                JMessageCode.JERR_BAD_CROP_SPEC => "Invalid crop request",
                JMessageCode.JERR_BAD_DCT_COEF => "DCT coefficient out of range",
                JMessageCode.JERR_BAD_DCTSIZE => "DCT scaled output block size {0}x{1} not supported",
                JMessageCode.JERR_BAD_DROP_SAMPLING => "Component index {0}: mismatching sampling ratio {1}:{2}, {3}:{4}, {5}",
                JMessageCode.JERR_BAD_HUFF_TABLE => "Bogus Huffman table definition",
                JMessageCode.JERR_BAD_IN_COLORSPACE => "Bogus input colorspace",
                JMessageCode.JERR_BAD_J_COLORSPACE => "Bogus JPEG colorspace",
                JMessageCode.JERR_BAD_LENGTH => "Bogus marker length",
                JMessageCode.JERR_BAD_MCU_SIZE => "Sampling factors too large for interleaved scan",
                JMessageCode.JERR_BAD_PRECISION => "Unsupported JPEG data precision {0}",
                JMessageCode.JERR_BAD_PROGRESSION => "Invalid progressive parameters Ss={0} Se={1} Ah={2} Al={3}",
                JMessageCode.JERR_BAD_PROG_SCRIPT => "Invalid progressive parameters at scan script entry {0}",
                JMessageCode.JERR_BAD_SAMPLING => "Bogus sampling factors",
                JMessageCode.JERR_BAD_SCAN_SCRIPT => "Invalid scan script at entry {0}",
                JMessageCode.JERR_BAD_STATE => "Improper call to JPEG library in state {0}",
                JMessageCode.JERR_BAD_VIRTUAL_ACCESS => "Bogus virtual array access",
                JMessageCode.JERR_BUFFER_SIZE => "Buffer passed to JPEG library is too small",
                JMessageCode.JERR_CANT_SUSPEND => "Suspension not allowed here",
                JMessageCode.JERR_CCIR601_NOTIMPL => "CCIR601 sampling not implemented yet",
                JMessageCode.JERR_COMPONENT_COUNT => "Too many color components: {0}, max {1}",
                JMessageCode.JERR_CONVERSION_NOTIMPL => "Unsupported color conversion request",
                JMessageCode.JERR_DAC_INDEX => "Bogus DAC index {0}",
                JMessageCode.JERR_DAC_VALUE => "Bogus DAC value 0x{0}",
                JMessageCode.JERR_DHT_INDEX => "Bogus DHT index {0}",
                JMessageCode.JERR_DQT_INDEX => "Bogus DQT index {0}",
                JMessageCode.JERR_EMPTY_IMAGE => "Empty JPEG image (DNL not supported)",
                JMessageCode.JERR_EOI_EXPECTED => "Didn't expect more than one scan",
                JMessageCode.JERR_FILE_WRITE => "Output file write error --- out of disk space?",
                JMessageCode.JERR_FRACT_SAMPLE_NOTIMPL => "Fractional sampling not implemented yet",
                JMessageCode.JERR_HUFF_CLEN_OVERFLOW => "Huffman code size table overflow",
                JMessageCode.JERR_HUFF_MISSING_CODE => "Missing Huffman code table entry",
                JMessageCode.JERR_IMAGE_TOO_BIG => "Maximum supported image dimension is {0} pixels",
                JMessageCode.JERR_INPUT_EMPTY => "Empty input file",
                JMessageCode.JERR_INPUT_EOF => "Premature end of input file",
                JMessageCode.JERR_MISMATCHED_QUANT_TABLE => "Cannot transcode due to multiple use of quantization table {0}",
                JMessageCode.JERR_MISSING_DATA => "Scan script does not transmit all data",
                JMessageCode.JERR_MODE_CHANGE => "Invalid color quantization mode change",
                JMessageCode.JERR_NOTIMPL => "Not implemented yet",
                JMessageCode.JERR_NOT_COMPILED => "Requested feature was omitted at compile time",
                JMessageCode.JERR_NO_ARITH_TABLE => "Arithmetic table 0x{0:X2} was not defined",
                JMessageCode.JERR_NO_HUFF_TABLE => "Huffman table 0x{0:X2} was not defined",
                JMessageCode.JERR_NO_IMAGE => "JPEG datastream contains no image",
                JMessageCode.JERR_NO_QUANT_TABLE => "Quantization table 0x{0:X2} was not defined",
                JMessageCode.JERR_NO_SOI => "Not a JPEG file: starts with 0x{0:X2} 0x{1:X2}",
                JMessageCode.JERR_OUT_OF_MEMORY => "Insufficient memory (case {0})",
                JMessageCode.JERR_QUANT_COMPONENTS => "Cannot quantize more than {0} color components",
                JMessageCode.JERR_QUANT_FEW_COLORS => "Cannot quantize to fewer than {0} colors",
                JMessageCode.JERR_QUANT_MANY_COLORS => "Cannot quantize to more than {0} colors",
                JMessageCode.JERR_SOF_BEFORE => "Invalid JPEG file structure: {0} before SOF",
                JMessageCode.JERR_SOF_DUPLICATE => "Invalid JPEG file structure: two SOF markers",
                JMessageCode.JERR_SOF_NO_SOS => "Invalid JPEG file structure: missing SOS marker",
                JMessageCode.JERR_SOF_UNSUPPORTED => "Unsupported JPEG process: SOF type 0x{0:X2}",
                JMessageCode.JERR_SOI_DUPLICATE => "Invalid JPEG file structure: two SOI markers",
                JMessageCode.JERR_SOS_NO_SOF => "Invalid JPEG file structure: SOS before SOF",
                JMessageCode.JERR_TOO_LITTLE_DATA => "Application transferred too few scanlines",
                JMessageCode.JERR_UNKNOWN_MARKER => "Unsupported marker type 0x{0:X2}",
                JMessageCode.JERR_WIDTH_OVERFLOW => "Image too wide for this implementation",
                JMessageCode.JTRC_16BIT_TABLES => "Caution: quantization tables are too coarse for baseline JPEG",
                JMessageCode.JTRC_ADOBE => "Adobe APP14 marker: version {0}, flags 0x{1:X4} 0x{2:X4}, transform {3}",
                JMessageCode.JTRC_APP0 => "Unknown APP0 marker (not JFIF), length {0}",
                JMessageCode.JTRC_APP14 => "Unknown APP14 marker (not Adobe), length {0}",
                JMessageCode.JTRC_DAC => "Define Arithmetic Table 0x{0:X2}: 0x{1:X2}",
                JMessageCode.JTRC_DHT => "Define Huffman Table 0x{0:X2}",
                JMessageCode.JTRC_DQT => "Define Quantization Table {0} precision {1}",
                JMessageCode.JTRC_DRI => "Define Restart Interval {0}",
                JMessageCode.JTRC_EOI => "End Of Image",
                JMessageCode.JTRC_HUFFBITS => "        {0:D3} {1:D3} {2:D3} {3:D3} {4:D3} {5:D3} {6:D3} {7:D3}",
                JMessageCode.JTRC_JFIF => "JFIF APP0 marker: version {0}.{1:D2}, density {2}x{3}  {4}",
                JMessageCode.JTRC_JFIF_BADTHUMBNAILSIZE => "Warning: thumbnail image size does not match data length {0}",
                JMessageCode.JTRC_JFIF_EXTENSION => "JFIF extension marker: type 0x{0:X2}, length {1}",
                JMessageCode.JTRC_JFIF_THUMBNAIL => "    with {0} x {1} thumbnail image",
                JMessageCode.JTRC_MISC_MARKER => "Miscellaneous marker 0x{0:X2}, length {1}",
                JMessageCode.JTRC_PARMLESS_MARKER => "Unexpected marker 0x{0:X2}",
                JMessageCode.JTRC_QUANTVALS => "        {0:D4} {1:D4} {2:D4} {3:D4} {4:D4} {5:D4} {6:D4} {7:D4}",
                JMessageCode.JTRC_QUANT_3_NCOLORS => "Quantizing to {0} = {1}*{2}*{3} colors",
                JMessageCode.JTRC_QUANT_NCOLORS => "Quantizing to {0} colors",
                JMessageCode.JTRC_QUANT_SELECTED => "Selected {0} colors for quantization",
                JMessageCode.JTRC_RECOVERY_ACTION => "At marker 0x{0:X2}, recovery action {1}",
                JMessageCode.JTRC_RST => "RST{0}",
                JMessageCode.JTRC_SMOOTH_NOTIMPL => "Smoothing not supported with nonstandard sampling ratios",
                JMessageCode.JTRC_SOF => "Start Of Frame 0x{0:X2}: width={1}, height={2}, components={3}",
                JMessageCode.JTRC_SOF_COMPONENT => "    Component {0}: {1}hx{2}v q={3}",
                JMessageCode.JTRC_SOI => "Start of Image",
                JMessageCode.JTRC_SOS => "Start Of Scan: {0} components",
                JMessageCode.JTRC_SOS_COMPONENT => "    Component {0}: dc={1} ac={2}",
                JMessageCode.JTRC_SOS_PARAMS => "  Ss={0}, Se={1}, Ah={2}, Al={3}",
                JMessageCode.JTRC_THUMB_JPEG => "JFIF extension marker: JPEG-compressed thumbnail image, length {0}",
                JMessageCode.JTRC_THUMB_PALETTE => "JFIF extension marker: palette thumbnail image, length {0}",
                JMessageCode.JTRC_THUMB_RGB => "JFIF extension marker: RGB thumbnail image, length {0}",
                JMessageCode.JTRC_UNKNOWN_IDS => "Unrecognized component IDs {0} {1} {2}, assuming YCbCr",
                JMessageCode.JWRN_ADOBE_XFORM => "Unknown Adobe color transform code {0}",
                JMessageCode.JWRN_ARITH_BAD_CODE => "Corrupt JPEG data: bad arithmetic code",
                JMessageCode.JWRN_BOGUS_PROGRESSION => "Inconsistent progression sequence for component {0} coefficient {1}",
                JMessageCode.JWRN_EXTRANEOUS_DATA => "Corrupt JPEG data: {0} extraneous bytes before marker 0x{1:X2}",
                JMessageCode.JWRN_HIT_MARKER => "Corrupt JPEG data: premature end of data segment",
                JMessageCode.JWRN_HUFF_BAD_CODE => "Corrupt JPEG data: bad Huffman code",
                JMessageCode.JWRN_JFIF_MAJOR => "Warning: unknown JFIF revision number {0}.{1:D2}",
                JMessageCode.JWRN_JPEG_EOF => "Premature end of JPEG file",
                JMessageCode.JWRN_MUST_RESYNC => "Corrupt JPEG data: found marker 0x{0:X2} instead of RST{1}",
                JMessageCode.JWRN_NOT_SEQUENTIAL => "Invalid SOS parameters for sequential JPEG",
                JMessageCode.JWRN_TOO_MUCH_DATA => "Application transferred too many scanlines",
                JMessageCode.JMSG_UNKNOWNMSGCODE => "Unknown message code (possibly it is an error from application)",
                _ => "Bogus message code {0}",
            };
        }
    }
}
