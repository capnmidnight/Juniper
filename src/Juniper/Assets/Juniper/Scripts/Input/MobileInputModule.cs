namespace Juniper.Input
{
    public abstract class MobileInputModule : AbstractUnifiedInputModule
    {
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

        public override InputMode DefaultInputMode
        {
            get
            {
                return InputMode.Touchscreen;
            }
        }
    }
}
