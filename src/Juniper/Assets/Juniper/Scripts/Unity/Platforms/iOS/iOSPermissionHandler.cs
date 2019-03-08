#if UNITY_IOS
using UnityEngine;

namespace Juniper.Unity
{
    public abstract class iOSPermissionHandler : AbstractPermissionHandler
    {
        public override bool Install(bool reset)
        {
            var baseInstall = base.Install(reset);

            if (!reset)
            {
                var auth = UserAuthorization.WebCam;
                var speech = ComponentExt.FindAny<Input.Speech.KeywordRecognizer>();
                if (speech != null && speech.IsAvailable)
                {
                    auth |= UserAuthorization.Microphone;
                }
                Application.RequestUserAuthorization(auth);
            }

            return baseInstall;
        }
    }
}
#endif