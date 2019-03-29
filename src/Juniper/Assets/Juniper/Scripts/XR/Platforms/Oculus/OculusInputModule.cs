#if UNITY_XR_OCULUS

namespace Juniper.Unity.Input
{
    public abstract class OculusInputModule : AbstractUnifiedInputModule
    {
        public override bool Install(bool reset)
        {
            if (base.Install(reset))
            {
                EnableMouse(true);
                EnableTouch(false);
                EnableGaze(false);
                EnableControllers(true);
                EnableHands(false);

                return true;
            }

            return false;
        }
    }
}
#endif