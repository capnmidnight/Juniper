namespace Juniper.Input
{
    public class AbstractMobileInputModule : AbstractUnifiedInputModule
    {
        public override void Install(bool reset)
        {
            base.Install(reset);

            if (!reset && mode == InputMode.Auto)
            {
                mode = InputMode.Touchscreen;
            }
        }

        public override bool HasFloorPosition
        {
            get
            {
#if UNITY_XR_ARCORE || UNITY_XR_ARKIT
                return true;
#else
                return false;
#endif
            }
        }
    }
}
