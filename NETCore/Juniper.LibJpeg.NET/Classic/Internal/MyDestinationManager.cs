/*
 * This file contains compression data destination routines for the case of
 * emitting JPEG data to memory or to a file (or any stdio stream).
 * While these routines are sufficient for most applications,
 * some will want to use a different destination manager.
 * IMPORTANT: we assume that fwrite() will correctly transcribe an array of
 * bytes into 8-bit-wide elements on external storage.  If char is wider
 * than 8 bits on your machine, you may need to do some tweaking.
 */

using System;
using System.IO;

namespace BitMiracle.LibJpeg.Classic.Internal
{
    /// <summary>
    /// Expanded data destination object for output to Stream
    /// </summary>
    internal class MyDestinationManager : JpegDestinationManager
    {
        private const int OUTPUT_BUF_SIZE = 4096;   /* choose an efficiently fwrite'able size */

        private readonly JpegCompressStruct m_cinfo;

        private readonly Stream m_outfile;      /* target stream */
        private byte[] m_buffer;     /* start of buffer */

        public MyDestinationManager(JpegCompressStruct cinfo, Stream alreadyOpenFile)
        {
            m_cinfo = cinfo;
            m_outfile = alreadyOpenFile;
        }

        /// <summary>
        /// Initialize destination --- called by jpeg_start_compress
        /// before any data is actually written.
        /// </summary>
        public override void InitDestination()
        {
            /* Allocate the output buffer --- it will be released when done with image */
            m_buffer = new byte[OUTPUT_BUF_SIZE];
            InitInternalBuffer(m_buffer, 0);
        }

        /// <summary>
        /// <para>Empty the output buffer --- called whenever buffer fills up.</para>
        /// <para>
        /// In typical applications, this should write the entire output buffer
        /// (ignoring the current state of next_output_byte and free_in_buffer),
        /// reset the pointer and count to the start of the buffer, and return true
        /// indicating that the buffer has been dumped.
        /// </para>
        /// <para>
        /// In applications that need to be able to suspend compression due to output
        /// overrun, a false return indicates that the buffer cannot be emptied now.
        /// In this situation, the compressor will return to its caller (possibly with
        /// an indication that it has not accepted all the supplied scanlines).  The
        /// application should resume compression after it has made more room in the
        /// output buffer.  Note that there are substantial restrictions on the use of
        /// suspension --- see the documentation.
        /// </para>
        /// <para>
        /// When suspending, the compressor will back up to a convenient restart point
        /// (typically the start of the current MCU). next_output_byte and free_in_buffer
        /// indicate where the restart point will be if the current call returns false.
        /// Data beyond this point will be regenerated after resumption, so do not
        /// write it out when emptying the buffer externally.
        /// </para>
        /// </summary>
        public override bool EmptyOutputBuffer()
        {
            WriteBuffer(m_buffer.Length);
            InitInternalBuffer(m_buffer, 0);
            return true;
        }

        /// <summary>
        /// <para>
        /// Terminate destination --- called by jpeg_finish_compress
        /// after all data has been written.  Usually needs to flush buffer.
        /// </para>
        /// <para>
        /// NB: *not* called by jpeg_abort or jpeg_destroy; surrounding
        /// application must deal with any cleanup that should happen even
        /// for error exit.
        /// </para>
        /// </summary>
        public override void TermDestination()
        {
            var datacount = m_buffer.Length - FreeInBuffer;

            /* Write any data remaining in the buffer */
            if (datacount > 0)
            {
                WriteBuffer(datacount);
            }

            m_outfile.Flush();
        }

        private void WriteBuffer(int dataCount)
        {
            try
            {
                m_outfile.Write(m_buffer, 0, dataCount);
            }
            catch (IOException e)
            {
                m_cinfo.TraceMS(0, JMessageCode.JERR_FILE_WRITE, e.Message);
                m_cinfo.ErrExit(JMessageCode.JERR_FILE_WRITE);
            }
            catch (NotSupportedException e)
            {
                m_cinfo.TraceMS(0, JMessageCode.JERR_FILE_WRITE, e.Message);
                m_cinfo.ErrExit(JMessageCode.JERR_FILE_WRITE);
            }
            catch (ObjectDisposedException e)
            {
                m_cinfo.TraceMS(0, JMessageCode.JERR_FILE_WRITE, e.Message);
                m_cinfo.ErrExit(JMessageCode.JERR_FILE_WRITE);
            }
        }
    }
}
