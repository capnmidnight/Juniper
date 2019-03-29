#if UNITY_XR_WINDOWSMR_METRO

namespace Juniper.Unity.Input
{
    public abstract class WindowsMRInputModule : AbstractUnifiedInputModule
    {
        public override bool Install(bool reset)
        {
            if (base.Install(reset))
            {
                if(!reset && mode == Mode.Auto)
                {
#if WINDOWSMR
                    mode = Mode.StandingVR;
#elif HOLOLENS
                    mode = Mode.HeadsetAR;
#endif
                }
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