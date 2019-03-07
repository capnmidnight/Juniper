namespace Juniper.Unity.Ground
{
    /// <summary>
    /// Manages references to the ground, either 3D terrain in VR apps or detected planes and meshes
    /// in AR apps.
    /// </summary>
    public class Ground :
#if HOLOLENS
        HoloLensGround
#elif UNITY_XR_ARKIT
        ARKitGround
#elif UNITY_XR_MAGICLEAP
        MagicLeapGround
#elif UNITY_XR_ARCORE
        ARCoreGround
#elif UNITY_MODULES_TERRAIN
        StaticTerrain
#else
        NoTerrain
#endif
    {
    }
}
