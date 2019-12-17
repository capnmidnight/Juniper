namespace BitMiracle.LibJpeg.Classic
{
    /// <summary>
    /// Huffman coding table.
    /// </summary>
    public class JHuffmanTable
    {
        internal JHuffmanTable()
        {
        }

        internal byte[] Bits { get; } = new byte[17];

        internal byte[] Huffval { get; } = new byte[256];

        /// <summary>
        /// Gets or sets a value indicating whether the table has been output to file.
        /// </summary>
        /// <value>It's initialized <c>false</c> when the table is created, and set 
        /// <c>true</c> when it's been output to the file. You could suppress output 
        /// of a table by setting this to <c>true</c>.
        /// </value>
        /// <remarks>This property is used only during compression. It's initialized
        /// <c>false</c> when the table is created, and set <c>true</c> when it's been
        /// output to the file. You could suppress output of a table by setting this to
        /// <c>true</c>. (See jpeg_suppress_tables for an example.)</remarks>
        /// <seealso cref="JpegCompressStruct.JpegSuppressTables"/>
        public bool SentTable { get; set; }
    }
}
