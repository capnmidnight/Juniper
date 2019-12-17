/*
 * This file contains application interface routines that are used for both
 * compression and decompression.
 */

using System;
using System.Globalization;
using System.Reflection;

namespace BitMiracle.LibJpeg.Classic
{
    /// <summary>Base class for both JPEG compressor and decompresor.</summary>
    /// <remarks>
    /// Routines that are to be used by both halves of the library are declared
    /// to receive an instance of this class. There are no actual instances of 
    /// <see cref="JpegCommonStruct"/>, only of <see cref="JpegCompressStruct"/> 
    /// and <see cref="JpegDecompressStruct"/>
    /// </remarks>
    public abstract class JpegCommonStruct
    {
        internal enum JpegState
        {
            DESTROYED = 0,
            CSTATE_START = 100,     /* after create_compress */
            CSTATE_SCANNING = 101,  /* start_compress done, write_scanlines OK */
            CSTATE_RAW_OK = 102,    /* start_compress done, write_raw_data OK */
            CSTATE_WRCOEFS = 103,   /* jpeg_write_coefficients done */
            DSTATE_START = 200,     /* after create_decompress */
            DSTATE_INHEADER = 201,  /* reading header markers, no SOS yet */
            DSTATE_READY = 202,     /* found SOS, ready for start_decompress */
            DSTATE_PRELOAD = 203,   /* reading multiscan file in start_decompress*/
            DSTATE_PRESCAN = 204,   /* performing dummy pass for 2-pass quant */
            DSTATE_SCANNING = 205,  /* start_decompress done, read_scanlines OK */
            DSTATE_RAW_OK = 206,    /* start_decompress done, read_raw_data OK */
            DSTATE_BUFIMAGE = 207,  /* expecting jpeg_start_output */
            DSTATE_BUFPOST = 208,   /* looking for SOS/EOI in jpeg_finish_output */
            DSTATE_RDCOEFS = 209,   /* reading file in jpeg_read_coefficients */
            DSTATE_STOPPING = 210   /* looking for EOI in jpeg_finish_decompress */
        }

        // Error handler module
        internal JpegErrorMessage err;

        // Progress monitor, or null if none
        internal JpegProgressManager progress;

        internal JpegState globalState;     /* For checking call sequence validity */

        /// <summary>
        /// Base constructor.
        /// </summary>
        /// <seealso cref="JpegCompressStruct"/>
        /// <seealso cref="JpegDecompressStruct"/>
        protected JpegCommonStruct() : this(new JpegErrorMessage())
        {
        }

        /// <summary>
        /// Base constructor.
        /// </summary>
        /// <param name="errorManager">The error manager.</param>
        /// <seealso cref="JpegCompressStruct"/>
        /// <seealso cref="JpegDecompressStruct"/>
        protected JpegCommonStruct(JpegErrorMessage errorManager)
        {
            Err = errorManager;
        }

        /// <summary>
        /// Gets a value indicating whether this instance is Jpeg decompressor.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this is Jpeg decompressor; otherwise, <c>false</c>.
        /// </value>
        public abstract bool IsDecompressor
        {
            get;
        }

        /// <summary>
        /// Progress monitor.
        /// </summary>
        /// <value>The progress manager.</value>
        /// <remarks>Default value: <c>null</c>.</remarks>
        public JpegProgressManager Progress
        {
            get
            {
                return progress;
            }
            set
            {
                progress = value ?? throw new ArgumentNullException(nameof(value));
            }
        }

        /// <summary>
        /// Error handler module.
        /// </summary>
        /// <value>The error manager.</value>
        /// <seealso href="41dc1a3b-0dea-4594-87d2-c213ab1049e1.htm" target="_self">Error handling</seealso>
        public JpegErrorMessage Err
        {
            get
            {
                return err;
            }
            set
            {
                err = value ?? throw new ArgumentNullException(nameof(value));
            }
        }

        /// <summary>
        /// Gets the version of LibJpeg.
        /// </summary>
        /// <value>The version of LibJpeg.</value>
        public static string Version
        {
            get
            {
#if !NETSTANDARD
                var assembly = Assembly.GetExecutingAssembly();
#else
                Assembly assembly = typeof(jpeg_common_struct).GetTypeInfo().Assembly;
#endif

                var assemblyName = new AssemblyName(assembly.FullName);

                var version = assemblyName.Version;
                var versionString = version.Major.ToString(CultureInfo.InvariantCulture) +
                    "." + version.Minor.ToString(CultureInfo.InvariantCulture);

                versionString += "." + version.Build.ToString(CultureInfo.InvariantCulture);
                versionString += "." + version.Revision.ToString(CultureInfo.InvariantCulture);

                return versionString;
            }
        }

        /// <summary>
        /// Gets the LibJpeg's copyright.
        /// </summary>
        /// <value>The copyright.</value>
        public static string Copyright
        {
            get
            {
                return "Copyright (C) 2008-2018, Bit Miracle";
            }
        }

        /// <summary>
        /// Creates the array of samples.
        /// </summary>
        /// <param name="samplesPerRow">The number of samples in row.</param>
        /// <param name="numberOfRows">The number of rows.</param>
        /// <returns>The array of samples.</returns>
        public static JVirtArray<byte> CreateSamplesArray(int samplesPerRow, int numberOfRows)
        {
            return new JVirtArray<byte>(samplesPerRow, numberOfRows, AllocJpegSamples);
        }

        /// <summary>
        /// Creates the array of blocks.
        /// </summary>
        /// <param name="blocksPerRow">The number of blocks in row.</param>
        /// <param name="numberOfRows">The number of rows.</param>
        /// <returns>The array of blocks.</returns>
        /// <seealso cref="JBlock"/>
        public static JVirtArray<JBlock> CreateBlocksArray(int blocksPerRow, int numberOfRows)
        {
            return new JVirtArray<JBlock>(blocksPerRow, numberOfRows, AllocJpegBlocks);
        }

        /// <summary>
        /// Creates 2-D sample array.
        /// </summary>
        /// <param name="samplesPerRow">The number of samples per row.</param>
        /// <param name="numberOfRows">The number of rows.</param>
        /// <returns>The array of samples.</returns>
        public static byte[][] AllocJpegSamples(int samplesPerRow, int numberOfRows)
        {
            var result = new byte[numberOfRows][];
            for (var i = 0; i < numberOfRows; ++i)
            {
                result[i] = new byte[samplesPerRow];
            }

            return result;
        }

        // Creation of 2-D block arrays.
        private static JBlock[][] AllocJpegBlocks(int blocksPerRow, int numberOfRows)
        {
            var result = new JBlock[numberOfRows][];
            for (var i = 0; i < numberOfRows; ++i)
            {
                result[i] = new JBlock[blocksPerRow];
                for (var j = 0; j < blocksPerRow; ++j)
                {
                    result[i][j] = new JBlock();
                }
            }
            return result;
        }

        // Generic versions of jpeg_abort and jpeg_destroy that work on either
        // flavor of JPEG object.  These may be more convenient in some places.

        /// <summary>
        /// <para>
        /// Abort processing of a JPEG compression or decompression operation,
        /// but don't destroy the object itself.
        /// </para>
        /// <para>
        /// Closing a data source or destination, if necessary, is the 
        /// application's responsibility.
        /// </para>
        /// </summary>
        public void JpegAbort()
        {
            /* Reset overall state for possible reuse of object */
            if (IsDecompressor)
            {
                globalState = JpegState.DSTATE_START;

                /* Try to keep application from accessing now-deleted marker list.
                 * A bit kludgy to do it here, but this is the most central place.
                 */
                if (this is JpegDecompressStruct s)
                {
                    s.m_marker_list = null;
                }
            }
            else
            {
                globalState = JpegState.CSTATE_START;
            }
        }

        /// <summary>
        /// <para>Destruction of a JPEG object. </para>
        /// <para>
        /// Closing a data source or destination, if necessary, is the 
        /// application's responsibility.
        /// </para>
        /// </summary>
        public void JpegDestroy()
        {
            // mark it destroyed
            globalState = JpegState.DESTROYED;
        }

        // Fatal errors (print message and exit)

        /// <summary>
        /// Used for fatal errors (print message and exit).
        /// </summary>
        /// <param name="code">The message code.</param>
        public void ErrExit(JMessageCode code)
        {
            ErrExit((int)code);
        }

        /// <summary>
        /// Used for fatal errors (print message and exit).
        /// </summary>
        /// <param name="code">The message code.</param>
        /// <param name="args">The parameters of message.</param>
        public void ErrExit(JMessageCode code, params object[] args)
        {
            ErrExit((int)code, args);
        }

        /// <summary>
        /// Used for fatal errors (print message and exit).
        /// </summary>
        /// <param name="code">The message code.</param>
        /// <param name="args">The parameters of message.</param>
        public void ErrExit(int code, params object[] args)
        {
            err.msgCode = code;
            err.msgParam = args;
            err.ErrorExit();
        }

        // Nonfatal errors (we can keep going, but the data is probably corrupt)

        /// <summary>
        /// Used for non-fatal errors (we can keep going, but the data is probably corrupt).
        /// </summary>
        /// <param name="code">The message code.</param>
        public void WarnMS(JMessageCode code)
        {
            WarnMS((int)code);
        }

        /// <summary>
        /// Used for non-fatal errors (we can keep going, but the data is probably corrupt).
        /// </summary>
        /// <param name="code">The message code.</param>
        /// <param name="args">The parameters of message.</param>
        public void WarnMS(JMessageCode code, params object[] args)
        {
            WarnMS((int)code, args);
        }

        /// <summary>
        /// Used for non-fatal errors (we can keep going, but the data is probably corrupt).
        /// </summary>
        /// <param name="code">The message code.</param>
        /// <param name="args">The parameters of message.</param>
        public void WarnMS(int code, params object[] args)
        {
            err.msgCode = code;
            err.msgParam = args;
            err.EmitMessage(-1);
        }

        // Informational/debugging messages

        /// <summary>
        /// Shows informational and debugging messages.
        /// </summary>
        /// <param name="lvl">See <see cref="JpegErrorMessage.EmitMessage"/> for description.</param>
        /// <param name="code">The message code.</param>
        /// <seealso cref="JpegErrorMessage.EmitMessage"/>
        public void TraceMS(int lvl, JMessageCode code)
        {
            TraceMS(lvl, (int)code);
        }

        /// <summary>
        /// Shows informational and debugging messages.
        /// </summary>
        /// <param name="lvl">See <see cref="JpegErrorMessage.EmitMessage"/> for description.</param>
        /// <param name="code">The message code.</param>
        /// <param name="args">The parameters of message.</param>
        /// <seealso cref="JpegErrorMessage.EmitMessage"/>
        public void TraceMS(int lvl, JMessageCode code, params object[] args)
        {
            TraceMS(lvl, (int)code, args);
        }

        /// <summary>
        /// Shows informational and debugging messages.
        /// </summary>
        /// <param name="lvl">See <see cref="JpegErrorMessage.EmitMessage"/> for description.</param>
        /// <param name="code">The message code.</param>
        /// <param name="args">The parameters of message.</param>
        /// <seealso cref="JpegErrorMessage.EmitMessage"/>
        public void TraceMS(int lvl, int code, params object[] args)
        {
            err.msgCode = code;
            err.msgParam = args;
            err.EmitMessage(lvl);
        }
    }
}