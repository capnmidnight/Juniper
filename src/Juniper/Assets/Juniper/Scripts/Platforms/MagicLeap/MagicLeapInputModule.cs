#if UNITY_XR_MAGICLEAP
namespace Juniper.Input
{
    public class MagicLeapInputModule : AbstractUnifiedInputModule
    {
        public override void Install(bool reset)
        {
            base.Install(reset);

            if (!reset && mode == Mode.Auto)
            {
                mode = Mode.HeadsetAR;
            }
        }

        public override bool HasFloorPosition { get { return true; } }
    }
}
#endif
