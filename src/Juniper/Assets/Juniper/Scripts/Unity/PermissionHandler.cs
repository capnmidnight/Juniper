namespace Juniper.Unity
{
    public class PermissionHandler :
#if UNITY_IOS
        iOSPermissionHandler
#elif UNITY_ANDROID && ANDROID_API_23_OR_GREATER
        AndroidPermissionHandler
#elif UNITY_XR_MAGICLEAP
        MagicLeapPermissionHandler
#else
        AbstractPermissionHandler
#endif
    {
    }
}
