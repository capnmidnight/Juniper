#if UNITY_XR_OCULUS
using Juniper.Input.Pointers.Motion;

namespace Juniper.Haptics
{
    public class OculusHaptics : AbstractHapticImmediateExpressor
    {
        OVRInput.Controller handID;

        public void Awake()
        {
            var controller = GetComponent<OculusMotionController>();
            handID = controller.NativeHandID.Value;
        }

        protected override void SetVibration(long milliseconds, float amplitude)
        {
            OVRInput.SetControllerVibration(0.5f, amplitude, handID);
        }
    }
}
#endif