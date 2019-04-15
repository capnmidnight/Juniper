#if UNITY_XR_WINDOWSMR_METRO
using UnityEngine.XR.WSA.Input;

using InputButton = UnityEngine.EventSystems.PointerEventData.InputButton;

using Juniper.Input;

namespace Juniper.Unity.Input.Pointers.Motion
{
    public class WindowsMRMotionControllerConfiguration : AbstractMotionControllerConfiguration<InteractionSourceHandedness, WindowsMRButtons>
    {
        public WindowsMRMotionControllerConfiguration()
        {
            AddButton(WindowsMRButtons.AirTap, InputButton.Left);
            AddButton(WindowsMRButtons.Select, InputButton.Left);
            AddButton(WindowsMRButtons.Touchpad, InputButton.Right);
            AddButton(WindowsMRButtons.App, InputButton.Middle);
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