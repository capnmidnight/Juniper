using UnityEngine;

namespace Juniper.Unity.Permissions
{
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
