#if UNITY_XR_MAGICLEAP
namespace Juniper.Input
{
    public class MagicLeapInputModule : AbstractUnifiedInputModule
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
                return InputModes.HeadsetAR;
            }
        }
    }
}
#endif
