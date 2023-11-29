namespace Juniper.XR;

public static class SphereEncoding
{
    public enum SphereEncodingKind
    {
        None,
        Cubemap,
        CubemapStrips,
        EquiAngularCubemap,
        Equirectangular,
        HalfEquirectangular,
        Panoramic
    }

    public static readonly IReadOnlyDictionary<SphereEncodingKind, string> SphereEncodingNames;
    public static readonly IReadOnlyDictionary<string, SphereEncodingKind> NameSphereEncodings;

    static SphereEncoding()
    {
        SphereEncodingNames = new Dictionary<SphereEncodingKind, string>()
        {
            {SphereEncodingKind.None, "N/A"},
            {SphereEncodingKind.Cubemap, "Cubemap"},
            {SphereEncodingKind.CubemapStrips, "Cubemap Strips"},
            {SphereEncodingKind.EquiAngularCubemap, "Equi-Angular Cubemap (YouTube)"},
            {SphereEncodingKind.Equirectangular, "Equirectangular"},
            {SphereEncodingKind.HalfEquirectangular, "Half Equirectangular"},
            {SphereEncodingKind.Panoramic, "Panoramic"}
        };

        NameSphereEncodings = SphereEncodingNames.Invert();
    }
}
