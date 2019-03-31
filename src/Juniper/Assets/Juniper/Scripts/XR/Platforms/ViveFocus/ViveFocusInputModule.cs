#if WAVEVR

namespace Juniper.Unity.Input
{
    public abstract class ViveFocusInputModule : AbstractUnifiedInputModule
    {
        public override bool Install(bool reset)
        {
            if (base.Install(reset))
            {
                if (!reset && mode == Mode.Auto)
                {
                    mode = Mode.StandingVR;
                }

                return true;
            }

            return false;
        }
    }
}
#endif
