namespace Juniper.Unity.Permissions
{
    public class PermissionHandler :
#if UNITY_IOS
        iOSPermissionHandler
#elif UNITY_ANDROID && ANDROID_API_23_OR_GREATER
        AndroidPermissionHandler
#elif UNITY_XR_MAGICLEAP
        MagicLeapPermissionHandler
#elif UNIT_XR_WINDOWSMR
        WindowsMRPermissionHandler
#else
        AbstractPermissionHandler
#endif
    {
    }
}