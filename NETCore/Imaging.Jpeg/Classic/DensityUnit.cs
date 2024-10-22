namespace BitMiracle.LibJpeg.Classic;

/// <summary>
/// The unit of density.
/// </summary>
/// <seealso cref="JpegCompressStruct.Density_unit"/>
/// <seealso cref="JpegDecompressStruct.DensityUnit"/>
public enum DensityUnit
{
    /// <summary>
    /// Unknown density
    /// </summary>
    Unknown = 0,

    /// <summary>
    /// Dots/inch
    /// </summary>
    DotsInch = 1,

    /// <summary>
    /// Dots/cm
    /// </summary>
    DotsCm = 2
}
