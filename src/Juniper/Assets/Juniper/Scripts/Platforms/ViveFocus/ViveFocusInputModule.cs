#if WAVEVR

namespace Juniper.Input
{
    public abstract class ViveFocusInputModule : AbstractUnifiedInputModule
    {
        public override bool HasFloorPosition
        {
            get
            {
                return true;
            }
        }

        public override InputModes DefaultInputMode
        {
            get
            {
                return InputModes.StandingVR;
            }
        }
    }
}
#endif
