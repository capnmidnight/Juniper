#if WAVEVR

namespace Juniper.Unity.Input
{
    public abstract class ViveFocusInputModule : AbstractUnifiedInputModule
    {
        public override void Install(bool reset)
        {
            base.Install(reset);

            if (!reset && mode == Mode.Auto)
            {
                mode = Mode.StandingVR;
            }
        }
    }
}
#endif
