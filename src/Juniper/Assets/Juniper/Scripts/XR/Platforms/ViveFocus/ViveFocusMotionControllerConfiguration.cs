#if WAVEVR

using Juniper.Input;
using Juniper.Unity.Haptics;

using UnityEngine;

using wvr;

using InputButton = UnityEngine.EventSystems.PointerEventData.InputButton;

namespace Juniper.Unity.Input.Pointers.Motion
{
    public class ViveFocusMotionControllerConfiguration : AbstractMotionControllerConfiguration<WVR_DeviceType, WVR_InputId>
    {
        public ViveFocusMotionControllerConfiguration()
        {
            AddButton(VirtualTouchPadButton.Top, InputButton.Left);
            AddButton(VirtualTouchPadButton.Bottom, InputButton.Left);
            AddButton(WVR_InputId.WVR_InputId_Alias1_Menu, InputButton.Middle);
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