#if UNITY_XR_WINDOWSMR_METRO && WINDOWSMR
using UnityEngine.XR.WSA.Input;

using InputButton = UnityEngine.EventSystems.PointerEventData.InputButton;

using Juniper.Input;

#if UNITY_EDITOR
using HapticsType = Juniper.Unity.Haptics.NoHaptics;
#else
using HapticsType = Juniper.Unity.Haptics.WindowsMRHaptics;
#endif

namespace Juniper.Unity.Input.Pointers.Motion
{
    public class WindowsMRMotionControllerConfiguration : AbstractMotionControllerConfiguration<InteractionSourceHandedness, WindowsMRButtons>
    {
        public WindowsMRMotionControllerConfiguration()
        {
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