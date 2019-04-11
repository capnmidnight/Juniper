using UnityEngine;

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

    /// <summary>
    /// Perform system-specific authorization requests, such as GPS on Android, or Microphone on iOS.
    /// </summary>
    public abstract class AbstractPermissionHandler : MonoBehaviour, IInstallable
    {
        public void Awake()
        {
            Install(false);
        }

        public virtual void Reinstall()
        {
            Install(true);
        }

#if UNITY_EDITOR

        public void Reset()
        {
            Reinstall();
        }

#endif

        public virtual bool Install(bool reset)
        {
            return true;
        }

        public virtual void Uninstall()
        {
        }
    }
}
