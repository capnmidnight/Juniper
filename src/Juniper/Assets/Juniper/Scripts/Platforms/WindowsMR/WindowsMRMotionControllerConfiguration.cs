#if UNITY_XR_WINDOWSMR_METRO
using UnityEngine;
using UnityEngine.XR.WSA.Input;

namespace Juniper.Input.Pointers.Motion
{
    public class WindowsMRMotionControllerConfiguration : AbstractMotionControllerConfiguration<InteractionSourceHandedness, WindowsMRButtons>
    {
        public WindowsMRMotionControllerConfiguration()
        {
            AddButton(WindowsMRButtons.AirTap, KeyCode.Mouse0);
            AddButton(WindowsMRButtons.Select, KeyCode.Mouse0);
            AddButton(WindowsMRButtons.Touchpad, KeyCode.Mouse1);
            AddButton(WindowsMRButtons.App, KeyCode.Escape);
        }

        public override InteractionSourceHandedness? this[Hands hand]
        {
            get
            {
                if (hand == Hands.Left)
                {
                    return InteractionSourceHandedness.Left;
                }
                else if (hand == Hands.Right)
                {
                    return InteractionSourceHandedness.Right;
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