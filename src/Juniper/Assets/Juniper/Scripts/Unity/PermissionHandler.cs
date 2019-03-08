namespace Juniper.Unity
{
    public class PermissionHandler :
#if UNITY_IOS
        iOSPermissionHandler
#elif UNITY_XR_MAGICLEAP
        MagicLeapPermissionHandler
#else
        AbstractPermissionHandler
#endif
    {
    }
}
