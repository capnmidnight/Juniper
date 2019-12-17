namespace BitMiracle.LibJpeg.Classic.Internal
{
    /// <summary>
    /// Bitreading state saved across MCUs
    /// </summary>
    internal struct BitreadPermState
    {
        public int getBuffer;    /* current bit-extraction buffer */
        public int bitsLeft;      /* # of unused bits in it */
    }
}
