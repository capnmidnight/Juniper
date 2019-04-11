#if UNITY_XR_OCULUS

namespace Juniper.Unity.Input
{
    public abstract class OculusInputModule : AbstractUnifiedInputModule
    {
        public override void Install(bool reset)
        {
            base.Install(reset);

            if (!reset && mode == Mode.Auto)
            {
#if UNITY_XR_OCULUS_ANDROID
                mode = Mode.SeatedVR;
#else
                mode = Mode.StandingVR;
#endif
            }
        }
    }
}
#endif
