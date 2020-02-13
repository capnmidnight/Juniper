#if UNITY_XR_WINDOWSMR_METRO

namespace Juniper.Input
{
    public abstract class WindowsMRInputModule : AbstractUnifiedInputModule
    {
        /// <summary>
        /// Manages the input controllers, either motion controllers on WindowsMR headsets, or hand
        /// tracking on HoloLens.
        /// </summary>
        public override void UpdateModule()
        {
            base.UpdateModule();
            var kind = Pointers.Motion.WindowsMRMotionController.UpdateReadings();

            if (mode == Mode.None)
            {
error-out: mind the input mode
                mode = kind == UnityEngine.XR.WSA.Input.InteractionSourceKind.Controller
                    ? Mode.StandingVR
                    : Mode.HeadsetAR;
            }
        }

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
