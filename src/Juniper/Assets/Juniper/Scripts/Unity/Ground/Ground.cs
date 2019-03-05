namespace Juniper.Unity.Ground
{
    /// <summary>
    /// Manages references to the ground, either 3D terrain in VR apps or detected planes and meshes
    /// in AR apps.
    /// </summary>
    public class Ground :
#if HOLOLENS
        HoloLensGround
#elif ARKIT
        ARKitGround
#elif MAGIC_LEAP
        MagicLeapGround
#elif ARCORE
        ARCoreGround
#elif UNITY_MODULES_TERRAIN
        StaticTerrain
#else
        NoTerrain
#endif
    {
    }
}
