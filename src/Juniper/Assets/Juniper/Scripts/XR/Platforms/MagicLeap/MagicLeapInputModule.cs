#if UNITY_XR_MAGICLEAP
namespace Juniper.Unity.Input
{
    public class MagicLeapInputModule : AbstractUnifiedInputModule
    {
        public override bool Install(bool reset)
        {
            if (base.Install(reset))
            {
                if (!reset && mode == Mode.Auto)
                {
                    mode = Mode.HeadsetAR;
                }

                return true;
            }

            return false;
        }
    }
}
#endif