using static System.Math;

namespace Juniper.World.GIS;

/// <summary>
/// A model of the Earth.
/// </summary>
public static class DatumWGS_84
{
    public const double FalseNorthing = 10000000.0;
    private const double invF = 298.257223563;
    public const double equatorialRadius = 6378137;
    public const double pointScaleFactor = 0.9996;
    public const double E0 = 500000;

    public const double flattening = 1 / invF;
    private const double flatteningComp = 1 - flattening;
    private const double n = flattening / (2 - flattening);
    public const double A = (equatorialRadius / (1 + n)) * (1 + (n * n / 4) + (n * n * n * n / 64));

    public static readonly double e = Sqrt(1 - (flatteningComp * flatteningComp));
    public static readonly double esq = 1 - (flatteningComp * flatteningComp);
    public static readonly double e0sq = e * e / (1 - (e * e));

    public static readonly double alpha1 = 1 - (esq * ((1.0 / 4.0) + (esq * ((3.0 / 64.0) + (5.0 * esq / 256.0)))));
    public static readonly double alpha2 = esq * ((3.0 / 8.0) + (esq * ((3.0 / 32.0) + (45.0 * esq / 1024.0))));
    public static readonly double alpha3 = esq * esq * ((15.0 / 256.0) + (esq * 45.0 / 1024.0));
    public static readonly double alpha4 = esq * esq * esq * (35.0 / 3072.0);

    public static readonly double[] beta = [
        (n / 2) - (2 * n * n / 3) + (37 * n * n * n / 96),
        (n * n / 48) + (n * n * n / 15),
        17 * n * n * n / 480
    ];

    public static readonly double[] delta = [
        (2 * n) - (2 * n * n / 3),
        (7 * n * n / 3) - (8 * n * n * n / 5),
        56 * n * n * n / 15
    ];
}