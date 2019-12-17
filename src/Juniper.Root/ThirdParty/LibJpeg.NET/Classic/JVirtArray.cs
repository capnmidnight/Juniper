/*
 * This file contains the JPEG system-independent memory management
 * routines.
 */

/*
 * About virtual array management:
 *
 * Full-image-sized buffers are handled as "virtual" arrays.  The array is still accessed a strip at a
 * time, but the memory manager must save the whole array for repeated
 * accesses.
 *
 * The Access method is responsible for making a specific strip area accessible.
 */

using System;
using System.Diagnostics;

namespace BitMiracle.LibJpeg.Classic
{
    /// <summary>
    /// JPEG virtual array.
    /// </summary>
    /// <typeparam name="T">The type of array's elements.</typeparam>
    /// <remarks>You can't create virtual array manually. For creation use methods
    /// <see cref="JpegCommonStruct.CreateSamplesArray"/> and
    /// <see cref="JpegCommonStruct.CreateBlocksArray"/>.
    /// </remarks>
    public class JVirtArray<T>
    {
        internal delegate T[][] Allocator(int width, int height);

        private readonly T[][] buffer;   /* => the in-memory buffer */

        /// <summary>
        /// Request a virtual 2-D array
        /// </summary>
        /// <param name="width">Width of array</param>
        /// <param name="height">Total virtual array height</param>
        /// <param name="allocator">The allocator.</param>
        internal JVirtArray(int width, int height, Allocator allocator)
        {
            ErrorProcessor = null;
            buffer = allocator(width, height);

            Debug.Assert(buffer != null);
        }

        /// <summary>
        /// Gets or sets the error processor.
        /// </summary>
        /// <value>The error processor.<br/>
        /// Default value: <c>null</c>
        /// </value>
        /// <remarks>Uses only for calling 
        /// <see cref="M:BitMiracle.LibJpeg.Classic.jpeg_common_struct.ERREXIT(BitMiracle.LibJpeg.Classic.J_MESSAGE_CODE)">jpeg_common_struct.ERREXIT</see>
        /// on error.</remarks>
        public JpegCommonStruct ErrorProcessor { get; set; }

        /// <summary>
        /// Access the part of a virtual array.
        /// </summary>
        /// <param name="startRow">The first row in required block.</param>
        /// <param name="numberOfRows">The number of required rows.</param>
        /// <returns>The required part of virtual array.</returns>
        public T[][] Access(int startRow, int numberOfRows)
        {
            /* debugging check */
            if (startRow + numberOfRows > buffer.Length)
            {
                if (ErrorProcessor != null)
                {
                    ErrorProcessor.ErrExit(JMessageCode.JERR_BAD_VIRTUAL_ACCESS);
                }
                else
                {
                    throw new InvalidOperationException("Bogus virtual array access");
                }
            }

            /* Return proper part of the buffer */
            var ret = new T[numberOfRows][];
            for (var i = 0; i < numberOfRows; i++)
            {
                ret[i] = buffer[startRow + i];
            }

            return ret;
        }
    }
}
