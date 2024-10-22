namespace BitMiracle.LibJpeg;

/// <summary>
/// Dithering options for decompression.
/// </summary>
internal enum DitherMode
{
    None,               /* no dithering */
    Ordered,            /* simple ordered dither */
    FloydSteinberg      /* Floyd-Steinberg error diffusion dither */
}
