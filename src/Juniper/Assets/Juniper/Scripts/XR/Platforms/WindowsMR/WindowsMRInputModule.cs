#if UNITY_XR_WINDOWSMR_METRO

namespace Juniper.Unity.Input
{
    public abstract class WindowsMRInputModule : AbstractUnifiedInputModule
    {
        public override bool Install(bool reset)
        {
            if (base.Install(reset))
            {
#if WINDOWSMR
                EnableMouse(true);
                EnableTouch(false);
                EnableGaze(false);
                EnableControllers(true);
                EnableHands(false);
#elif HOLOLENS
                EnableMouse(false);
                EnableTouch(false);
                EnableGaze(true);
                EnableControllers(false);
                EnableHands(true);
#endif
                return true;
            }

            return false;
        }
        /// <summary>
        /// Manages the input controllers, either motion controllers on WindowsMR headsets, or hand
        /// tracking on HoloLens.
        /// </summary>
        public override void UpdateModule()
        {
            base.UpdateModule();
            Pointers.Motion.MotionController.UpdateReadings();
        }
    }
}
#endif