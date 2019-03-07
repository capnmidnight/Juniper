using UnityEngine;

#if UNITY_XR_MAGICLEAP

using UnityEngine.XR.MagicLeap;

#endif

namespace Juniper.Unity
{
    public class PermissionHandler : MonoBehaviour, IInstallable
    {
        /// <summary>
        /// Perform system-specific authorization requests, such as GPS on Android, or Microphone on iOS.
        /// </summary>
        public void Awake()
        {
            Install(false);
#if UNITY_IOS
            if(Application.isPlaying)
            {
                var auth = UserAuthorization.WebCam;
                var speech = ComponentExt.FindAny<Input.Speech.KeywordRecognizer>();
                if (speech != null && speech.IsAvailable)
                {
                    auth |= UserAuthorization.Microphone;
                }
                Application.RequestUserAuthorization(auth);
            }
#endif
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

        public void Install(bool reset)
        {
#if UNITY_XR_MAGICLEAP
            this.EnsureComponent<PrivilegeRequester>();
#endif
        }

        public void Uninstall()
        {
#if UNITY_XR_MAGICLEAP
            this.RemoveComponent<PrivilegeRequester>();
#endif
        }
    }
}
