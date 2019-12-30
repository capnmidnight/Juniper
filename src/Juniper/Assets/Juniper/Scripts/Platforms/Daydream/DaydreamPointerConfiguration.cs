#if UNITY_XR_GOOGLEVR_ANDROID

using Juniper.Input;

namespace Juniper.Input.Pointers.Motion
{
    public class DaydreamPointerConfiguration : AbstractMotionControllerConfiguration<GvrControllerHand, GvrControllerButton>
    {
        public DaydreamPointerConfiguration()
        {
            AddButton(VirtualTouchPadButton.Top, KeyCode.Mouse0);
            AddButton(VirtualTouchPadButton.Bottom, KeyCode.Mouse1);
            AddButton(GvrControllerButton.App, KeyCode.Escape);
        }

        public override GvrControllerHand? this[Hand hand]
        {
            get
            {
                if (hand == Hand.Left)
                {
                    return GvrControllerHand.Left;
                }
                else if (hand == Hand.Right)
                {
                    return GvrControllerHand.Right;
                }
                else
                {
                    return null;
                }
            }
        }
    }
}

#endif