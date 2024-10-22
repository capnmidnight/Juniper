namespace BitMiracle.LibJpeg.Classic.Internal;

/// <summary>
/// Bitreading working state within an MCU
/// </summary>
internal struct BitreadWorkingState
{
    public int getBuffer { get; set; }    /* current bit-extraction buffer */
    public int bitsLeft { get; set; }      /* # of unused bits in it */

    /* Pointer needed by jpeg_fill_bit_buffer. */
    public JpegDecompressStruct cinfo { get; set; }  /* back link to decompress master record */
}
