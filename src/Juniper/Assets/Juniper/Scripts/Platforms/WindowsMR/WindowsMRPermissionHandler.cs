#if UNITY_XR_WINDOWSMR_METRO
using Juniper.Input.Speech;

using UnityEngine;

namespace Juniper.Permissions
{
    public abstract class WindowsMRPermissionHandler : AbstractPermissionHandler
    {
#if UNITY_EDITOR
        private void OnValidate()
        {
            UnityEditor.PlayerSettings.WSA.SetCapability(UnityEditor.PlayerSettings.WSACapability.InternetClient, true);
            UnityEditor.PlayerSettings.WSA.SetCapability(UnityEditor.PlayerSettings.WSACapability.WebCam, true);
            UnityEditor.PlayerSettings.WSA.SetCapability(UnityEditor.PlayerSettings.WSACapability.Bluetooth, true);
            UnityEditor.PlayerSettings.WSA.SetCapability(UnityEditor.PlayerSettings.WSACapability.Microphone, KeywordRecognizer.IsAvailable && Find.Any<KeywordRecognizer>() != null);
            UnityEditor.PlayerSettings.WSA.SetCapability(UnityEditor.PlayerSettings.WSACapability.Location, Find.Any<World.GPSLocation>() != null);
        }
#endif

        public override void Install(bool reset)
        {
            base.Install(reset);

            if (!reset && KeywordRecognizer.IsAvailable)
            {
                var auth = UserAuthorization.WebCam;
                if (Find.Any(out KeywordRecognizer speech))
                {
                    auth |= UserAuthorization.Microphone;
                }
                Application.RequestUserAuthorization(auth);
            }
        }
    }
}
#endif
