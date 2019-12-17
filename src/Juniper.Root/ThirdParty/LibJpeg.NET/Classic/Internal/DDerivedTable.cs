namespace BitMiracle.LibJpeg.Classic.Internal
{
    /// <summary>
    /// Derived data constructed for each Huffman table
    /// </summary>
    internal class DDerivedTable
    {
        /* Basic tables: (element [0] of each array is unused) */
        public int[] maxCode = new int[18];      /* largest code of length k (-1 if none) */
        /* (maxcode[17] is a sentinel to ensure jpeg_huff_decode terminates) */
        public int[] valOffset = new int[17];        /* huffval[] offset for codes of length k */
        /* valoffset[k] = huffval[] index of 1st symbol of code length k, less
        * the smallest code of length k; so given a code of length k, the
        * corresponding symbol is huffval[code + valoffset[k]]
        */

        /* Link to public Huffman table (needed only in jpeg_huff_decode) */
        public JHuffmanTable pub;

        /* Lookahead tables: indexed by the next HUFF_LOOKAHEAD bits of
        * the input data stream.  If the next Huffman code is no more
        * than HUFF_LOOKAHEAD bits long, we can obtain its length and
        * the corresponding symbol directly from these tables.
        */
        public int[] lookNBits = new int[1 << JpegConstants.HUFF_LOOKAHEAD]; /* # bits, or 0 if too long */
        public byte[] lookSym = new byte[1 << JpegConstants.HUFF_LOOKAHEAD]; /* symbol, or unused */
    }
}
