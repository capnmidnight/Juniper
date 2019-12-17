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

        internal int traceLevel;
        internal int numWarnings;

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
            get { return traceLevel; }
            set { traceLevel = value; }
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
            get { return numWarnings; }
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
                if (numWarnings == 0 || traceLevel >= 3)
                {
                    OutputMessage();
                }

                /* Always count warnings in num_warnings. */
                numWarnings++;
            }
            else
            {
                /* It's a trace message.  Show it if trace_level >= msg_level. */
                if (traceLevel >= msg_level)
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
            numWarnings = 0;

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
            switch ((JMessageCode)code)
            {
                /* For maintenance convenience, list is alphabetical by message code name */
                case JMessageCode.JERR_BAD_BUFFER_MODE:
                    return "Bogus buffer control mode";
                case JMessageCode.JERR_BAD_COMPONENT_ID:
                    return "Invalid component ID {0} in SOS";
                case JMessageCode.JERR_BAD_CROP_SPEC:
                    return "Invalid crop request";
                case JMessageCode.JERR_BAD_DCT_COEF:
                    return "DCT coefficient out of range";
                case JMessageCode.JERR_BAD_DCTSIZE:
                    return "DCT scaled output block size {0}x{1} not supported";
                case JMessageCode.JERR_BAD_DROP_SAMPLING:
                    return "Component index {0}: mismatching sampling ratio {1}:{2}, {3}:{4}, {5}";
                case JMessageCode.JERR_BAD_HUFF_TABLE:
                    return "Bogus Huffman table definition";
                case JMessageCode.JERR_BAD_IN_COLORSPACE:
                    return "Bogus input colorspace";
                case JMessageCode.JERR_BAD_J_COLORSPACE:
                    return "Bogus JPEG colorspace";
                case JMessageCode.JERR_BAD_LENGTH:
                    return "Bogus marker length";
                case JMessageCode.JERR_BAD_MCU_SIZE:
                    return "Sampling factors too large for interleaved scan";
                case JMessageCode.JERR_BAD_PRECISION:
                    return "Unsupported JPEG data precision {0}";
                case JMessageCode.JERR_BAD_PROGRESSION:
                    return "Invalid progressive parameters Ss={0} Se={1} Ah={2} Al={3}";
                case JMessageCode.JERR_BAD_PROG_SCRIPT:
                    return "Invalid progressive parameters at scan script entry {0}";
                case JMessageCode.JERR_BAD_SAMPLING:
                    return "Bogus sampling factors";
                case JMessageCode.JERR_BAD_SCAN_SCRIPT:
                    return "Invalid scan script at entry {0}";
                case JMessageCode.JERR_BAD_STATE:
                    return "Improper call to JPEG library in state {0}";
                case JMessageCode.JERR_BAD_VIRTUAL_ACCESS:
                    return "Bogus virtual array access";
                case JMessageCode.JERR_BUFFER_SIZE:
                    return "Buffer passed to JPEG library is too small";
                case JMessageCode.JERR_CANT_SUSPEND:
                    return "Suspension not allowed here";
                case JMessageCode.JERR_CCIR601_NOTIMPL:
                    return "CCIR601 sampling not implemented yet";
                case JMessageCode.JERR_COMPONENT_COUNT:
                    return "Too many color components: {0}, max {1}";
                case JMessageCode.JERR_CONVERSION_NOTIMPL:
                    return "Unsupported color conversion request";
                case JMessageCode.JERR_DAC_INDEX:
                    return "Bogus DAC index {0}";
                case JMessageCode.JERR_DAC_VALUE:
                    return "Bogus DAC value 0x{0}";
                case JMessageCode.JERR_DHT_INDEX:
                    return "Bogus DHT index {0}";
                case JMessageCode.JERR_DQT_INDEX:
                    return "Bogus DQT index {0}";
                case JMessageCode.JERR_EMPTY_IMAGE:
                    return "Empty JPEG image (DNL not supported)";
                case JMessageCode.JERR_EOI_EXPECTED:
                    return "Didn't expect more than one scan";
                case JMessageCode.JERR_FILE_WRITE:
                    return "Output file write error --- out of disk space?";
                case JMessageCode.JERR_FRACT_SAMPLE_NOTIMPL:
                    return "Fractional sampling not implemented yet";
                case JMessageCode.JERR_HUFF_CLEN_OVERFLOW:
                    return "Huffman code size table overflow";
                case JMessageCode.JERR_HUFF_MISSING_CODE:
                    return "Missing Huffman code table entry";
                case JMessageCode.JERR_IMAGE_TOO_BIG:
                    return "Maximum supported image dimension is {0} pixels";
                case JMessageCode.JERR_INPUT_EMPTY:
                    return "Empty input file";
                case JMessageCode.JERR_INPUT_EOF:
                    return "Premature end of input file";
                case JMessageCode.JERR_MISMATCHED_QUANT_TABLE:
                    return "Cannot transcode due to multiple use of quantization table {0}";
                case JMessageCode.JERR_MISSING_DATA:
                    return "Scan script does not transmit all data";
                case JMessageCode.JERR_MODE_CHANGE:
                    return "Invalid color quantization mode change";
                case JMessageCode.JERR_NOTIMPL:
                    return "Not implemented yet";
                case JMessageCode.JERR_NOT_COMPILED:
                    return "Requested feature was omitted at compile time";
                case JMessageCode.JERR_NO_ARITH_TABLE:
                    return "Arithmetic table 0x{0:X2} was not defined";
                case JMessageCode.JERR_NO_HUFF_TABLE:
                    return "Huffman table 0x{0:X2} was not defined";
                case JMessageCode.JERR_NO_IMAGE:
                    return "JPEG datastream contains no image";
                case JMessageCode.JERR_NO_QUANT_TABLE:
                    return "Quantization table 0x{0:X2} was not defined";
                case JMessageCode.JERR_NO_SOI:
                    return "Not a JPEG file: starts with 0x{0:X2} 0x{1:X2}";
                case JMessageCode.JERR_OUT_OF_MEMORY:
                    return "Insufficient memory (case {0})";
                case JMessageCode.JERR_QUANT_COMPONENTS:
                    return "Cannot quantize more than {0} color components";
                case JMessageCode.JERR_QUANT_FEW_COLORS:
                    return "Cannot quantize to fewer than {0} colors";
                case JMessageCode.JERR_QUANT_MANY_COLORS:
                    return "Cannot quantize to more than {0} colors";
                case JMessageCode.JERR_SOF_BEFORE:
                    return "Invalid JPEG file structure: {0} before SOF";
                case JMessageCode.JERR_SOF_DUPLICATE:
                    return "Invalid JPEG file structure: two SOF markers";
                case JMessageCode.JERR_SOF_NO_SOS:
                    return "Invalid JPEG file structure: missing SOS marker";
                case JMessageCode.JERR_SOF_UNSUPPORTED:
                    return "Unsupported JPEG process: SOF type 0x{0:X2}";
                case JMessageCode.JERR_SOI_DUPLICATE:
                    return "Invalid JPEG file structure: two SOI markers";
                case JMessageCode.JERR_SOS_NO_SOF:
                    return "Invalid JPEG file structure: SOS before SOF";
                case JMessageCode.JERR_TOO_LITTLE_DATA:
                    return "Application transferred too few scanlines";
                case JMessageCode.JERR_UNKNOWN_MARKER:
                    return "Unsupported marker type 0x{0:X2}";
                case JMessageCode.JERR_WIDTH_OVERFLOW:
                    return "Image too wide for this implementation";
                case JMessageCode.JTRC_16BIT_TABLES:
                    return "Caution: quantization tables are too coarse for baseline JPEG";
                case JMessageCode.JTRC_ADOBE:
                    return "Adobe APP14 marker: version {0}, flags 0x{1:X4} 0x{2:X4}, transform {3}";
                case JMessageCode.JTRC_APP0:
                    return "Unknown APP0 marker (not JFIF), length {0}";
                case JMessageCode.JTRC_APP14:
                    return "Unknown APP14 marker (not Adobe), length {0}";
                case JMessageCode.JTRC_DAC:
                    return "Define Arithmetic Table 0x{0:X2}: 0x{1:X2}";
                case JMessageCode.JTRC_DHT:
                    return "Define Huffman Table 0x{0:X2}";
                case JMessageCode.JTRC_DQT:
                    return "Define Quantization Table {0} precision {1}";
                case JMessageCode.JTRC_DRI:
                    return "Define Restart Interval {0}";
                case JMessageCode.JTRC_EOI:
                    return "End Of Image";
                case JMessageCode.JTRC_HUFFBITS:
                    return "        {0:D3} {1:D3} {2:D3} {3:D3} {4:D3} {5:D3} {6:D3} {7:D3}";
                case JMessageCode.JTRC_JFIF:
                    return "JFIF APP0 marker: version {0}.{1:D2}, density {2}x{3}  {4}";
                case JMessageCode.JTRC_JFIF_BADTHUMBNAILSIZE:
                    return "Warning: thumbnail image size does not match data length {0}";
                case JMessageCode.JTRC_JFIF_EXTENSION:
                    return "JFIF extension marker: type 0x{0:X2}, length {1}";
                case JMessageCode.JTRC_JFIF_THUMBNAIL:
                    return "    with {0} x {1} thumbnail image";
                case JMessageCode.JTRC_MISC_MARKER:
                    return "Miscellaneous marker 0x{0:X2}, length {1}";
                case JMessageCode.JTRC_PARMLESS_MARKER:
                    return "Unexpected marker 0x{0:X2}";
                case JMessageCode.JTRC_QUANTVALS:
                    return "        {0:D4} {1:D4} {2:D4} {3:D4} {4:D4} {5:D4} {6:D4} {7:D4}";
                case JMessageCode.JTRC_QUANT_3_NCOLORS:
                    return "Quantizing to {0} = {1}*{2}*{3} colors";
                case JMessageCode.JTRC_QUANT_NCOLORS:
                    return "Quantizing to {0} colors";
                case JMessageCode.JTRC_QUANT_SELECTED:
                    return "Selected {0} colors for quantization";
                case JMessageCode.JTRC_RECOVERY_ACTION:
                    return "At marker 0x{0:X2}, recovery action {1}";
                case JMessageCode.JTRC_RST:
                    return "RST{0}";
                case JMessageCode.JTRC_SMOOTH_NOTIMPL:
                    return "Smoothing not supported with nonstandard sampling ratios";
                case JMessageCode.JTRC_SOF:
                    return "Start Of Frame 0x{0:X2}: width={1}, height={2}, components={3}";
                case JMessageCode.JTRC_SOF_COMPONENT:
                    return "    Component {0}: {1}hx{2}v q={3}";
                case JMessageCode.JTRC_SOI:
                    return "Start of Image";
                case JMessageCode.JTRC_SOS:
                    return "Start Of Scan: {0} components";
                case JMessageCode.JTRC_SOS_COMPONENT:
                    return "    Component {0}: dc={1} ac={2}";
                case JMessageCode.JTRC_SOS_PARAMS:
                    return "  Ss={0}, Se={1}, Ah={2}, Al={3}";
                case JMessageCode.JTRC_THUMB_JPEG:
                    return "JFIF extension marker: JPEG-compressed thumbnail image, length {0}";
                case JMessageCode.JTRC_THUMB_PALETTE:
                    return "JFIF extension marker: palette thumbnail image, length {0}";
                case JMessageCode.JTRC_THUMB_RGB:
                    return "JFIF extension marker: RGB thumbnail image, length {0}";
                case JMessageCode.JTRC_UNKNOWN_IDS:
                    return "Unrecognized component IDs {0} {1} {2}, assuming YCbCr";
                case JMessageCode.JWRN_ADOBE_XFORM:
                    return "Unknown Adobe color transform code {0}";
                case JMessageCode.JWRN_ARITH_BAD_CODE:
                    return "Corrupt JPEG data: bad arithmetic code";
                case JMessageCode.JWRN_BOGUS_PROGRESSION:
                    return "Inconsistent progression sequence for component {0} coefficient {1}";
                case JMessageCode.JWRN_EXTRANEOUS_DATA:
                    return "Corrupt JPEG data: {0} extraneous bytes before marker 0x{1:X2}";
                case JMessageCode.JWRN_HIT_MARKER:
                    return "Corrupt JPEG data: premature end of data segment";
                case JMessageCode.JWRN_HUFF_BAD_CODE:
                    return "Corrupt JPEG data: bad Huffman code";
                case JMessageCode.JWRN_JFIF_MAJOR:
                    return "Warning: unknown JFIF revision number {0}.{1:D2}";
                case JMessageCode.JWRN_JPEG_EOF:
                    return "Premature end of JPEG file";
                case JMessageCode.JWRN_MUST_RESYNC:
                    return "Corrupt JPEG data: found marker 0x{0:X2} instead of RST{1}";
                case JMessageCode.JWRN_NOT_SEQUENTIAL:
                    return "Invalid SOS parameters for sequential JPEG";
                case JMessageCode.JWRN_TOO_MUCH_DATA:
                    return "Application transferred too many scanlines";
                case JMessageCode.JMSG_UNKNOWNMSGCODE:
                    return "Unknown message code (possibly it is an error from application)";

                default:
                    return "Bogus message code {0}";
            }
        }
    }
}
