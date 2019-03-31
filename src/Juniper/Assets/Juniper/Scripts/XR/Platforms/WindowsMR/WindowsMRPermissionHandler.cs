#if UNITY_XR_WINDOWSMR_METRO
using Juniper.Unity.Input.Speech;

using UnityEngine;

namespace Juniper.Unity.Permissions
{
    public abstract class WindowsMRPermissionHandler : AbstractPermissionHandler
    {
#if UNITY_EDITOR
        private void OnValidate()
        {
            UnityEditor.PlayerSettings.WSA.SetCapability(UnityEditor.PlayerSettings.WSACapability.InternetClient, true);
            UnityEditor.PlayerSettings.WSA.SetCapability(UnityEditor.PlayerSettings.WSACapability.WebCam, true);
            UnityEditor.PlayerSettings.WSA.SetCapability(UnityEditor.PlayerSettings.WSACapability.Bluetooth, true);
            UnityEditor.PlayerSettings.WSA.SetCapability(UnityEditor.PlayerSettings.WSACapability.Microphone, KeywordRecognizer.IsAvailable && ComponentExt.FindAny<KeywordRecognizer>() != null);
            UnityEditor.PlayerSettings.WSA.SetCapability(UnityEditor.PlayerSettings.WSACapability.Location, ComponentExt.FindAny<World.GPSLocation>() != null);
        }
#endif

        public override bool Install(bool reset)
        {
            if(base.Install(reset))
            {
                if (!reset && KeywordRecognizer.IsAvailable)
                {
                    var auth = UserAuthorization.WebCam;
                    var speech = ComponentExt.FindAny<KeywordRecognizer>();
                    if (speech != null)
                    {
                        auth |= UserAuthorization.Microphone;
                    }
                    Application.RequestUserAuthorization(auth);
                }

                return true;
            }

            return false;
        }
    }
}
#endif
