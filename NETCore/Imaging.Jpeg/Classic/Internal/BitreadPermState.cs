namespace BitMiracle.LibJpeg.Classic.Internal;

/// <summary>
/// Bitreading state saved across MCUs
/// </summary>
internal struct BitreadPermState
{
    public int getBuffer { get; set; }    /* current bit-extraction buffer */
    public int bitsLeft { get; set; }      /* # of unused bits in it */
}
