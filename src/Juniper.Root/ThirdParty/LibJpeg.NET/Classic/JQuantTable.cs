namespace BitMiracle.LibJpeg.Classic
{
    /// <summary>
    /// DCT coefficient quantization tables.
    /// </summary>
    public class JQuantTable
    {
        /* This array gives the coefficient quantizers in natural array order
         * (not the zigzag order in which they are stored in a JPEG DQT marker).
         * CAUTION: IJG versions prior to v6a kept this array in zigzag order.
         */
        internal readonly short[] quantBal = new short[JpegConstants.DCTSIZE2];  /* quantization step for each coefficient */

        internal JQuantTable()
        {
        }

        /// <summary>
        /// Gets or sets a value indicating whether the table has been output to file.
        /// </summary>
        /// <value>It's initialized <c>false</c> when the table is created, and set
        /// <c>true</c> when it's been output to the file. You could suppress output of a table by setting this to <c>true</c>.
        /// </value>
        /// <remarks>This property is used only during compression.</remarks>
        /// <seealso cref="JpegCompressStruct.JpegSuppressTables"/>
        public bool SentTable { get; set; }
    }
}
