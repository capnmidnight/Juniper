namespace BitMiracle.LibJpeg;


/// <summary>
/// DCT/IDCT algorithm options.
/// </summary>
internal enum DCTMethod
{
    IntegerSlow,     /* slow but accurate integer algorithm */
    IntegerFast,     /* faster, less accurate integer method */
    Float            /* floating-point: accurate, fast on fast HW */
}
