#if GOOGLEVR
using UnityEngine;

namespace Juniper.Unity.Display
{
    public class DaydreamDisplayManager : AbstractDisplayManager
    {
        public override void Install(bool reset)
        {
            reset &= Application.isEditor;

            base.Install(reset);

            this.EnsureComponent<GvrHeadset>();

#if UNITY_MODULES_AUDIO && RESONANCE
            goog.stereoSpeakerModeEnabled = Application.isEditor;
#endif
        }

        public override void Uninstall()
        {
            this.RemoveComponent<GvrHeadset>();

            base.Uninstall();
        }
    }
}
#endif
