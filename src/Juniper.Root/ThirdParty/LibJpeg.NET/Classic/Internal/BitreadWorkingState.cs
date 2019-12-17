namespace BitMiracle.LibJpeg.Classic.Internal
{
    /// <summary>
    /// Bitreading working state within an MCU
    /// </summary>
    internal struct BitreadWorkingState
    {
        public int getBuffer;    /* current bit-extraction buffer */
        public int bitsLeft;      /* # of unused bits in it */

        /* Pointer needed by jpeg_fill_bit_buffer. */
        public JpegDecompressStruct cinfo;  /* back link to decompress master record */
    }
}
