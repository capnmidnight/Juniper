namespace BitMiracle.LibJpeg.Classic;

/// <summary>
/// Dithering options for decompression.
/// </summary>
/// <seealso cref="JpegDecompressStruct.Dither_mode"/>
public enum JDitherMode
{
    /// <summary>
    /// No dithering: fast, very low quality
    /// </summary>
    JDITHER_NONE,

    /// <summary>
    /// Ordered dither: moderate speed and quality
    /// </summary>
    JDITHER_ORDERED,

    /// <summary>
    /// Floyd-Steinberg dither: slow, high quality
    /// </summary>
    JDITHER_FS
}
