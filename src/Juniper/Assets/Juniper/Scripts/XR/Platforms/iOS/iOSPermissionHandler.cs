#if UNITY_IOS
using UnityEngine;

namespace Juniper.Unity.Permissions
{
    public abstract class iOSPermissionHandler : AbstractPermissionHandler
    {
        public override void Install(bool reset)
        {
            base.Install(reset);

            if (!reset)
            {
                var auth = UserAuthorization.WebCam;
                var speech = ComponentExt.FindAny<Input.Speech.IKeywordRecognizer>();
                if (speech != null && speech.IsAvailable)
                {
                    auth |= UserAuthorization.Microphone;
                }
                Application.RequestUserAuthorization(auth);
            }
        }
    }
}
#endif
