#if WAVEVR

using Juniper.Input;
using Juniper.Haptics;

using UnityEngine;

using wvr;

using InputButton = UnityEngine.EventSystems.PointerEventData.InputButton;

namespace Juniper.Input.Pointers.Motion
{
    public class ViveFocusMotionControllerConfiguration : AbstractMotionControllerConfiguration<WVR_DeviceType, WVR_InputId>
    {
        public ViveFocusMotionControllerConfiguration()
        {
            AddButton(VirtualTouchPadButton.Top, KeyCode.Mouse0);
            AddButton(VirtualTouchPadButton.Bottom, KeyCode.Mouse1);
            AddButton(WVR_InputId.WVR_InputId_Alias1_Menu, KeyCode.Escape);
        }

        public override WVR_DeviceType? this[Hands hand]
        {
            get
            {
                if (hand == Hands.Left)
                {
                    return WVR_DeviceType.WVR_DeviceType_Controller_Left;
                }
                else if (hand == Hands.Right)
                {
                    return WVR_DeviceType.WVR_DeviceType_Controller_Right;
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