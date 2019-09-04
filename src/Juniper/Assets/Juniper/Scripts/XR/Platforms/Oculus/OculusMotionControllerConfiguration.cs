#if UNITY_XR_OCULUS

using Juniper.Input;
using Juniper.Haptics;

using UnityEngine;

using InputButton = UnityEngine.EventSystems.PointerEventData.InputButton;

namespace Juniper.Input.Pointers.Motion
{
    public class OculusMotionControllerConfiguration : AbstractMotionControllerConfiguration<OVRInput.Controller, OVRInput.Button>
    {
        internal const OVRInput.Controller Left =
#if UNITY_ANDROID
            OVRInput.Controller.LTrackedRemote;

#else
            OVRInput.Controller.LTouch;
#endif

        internal const OVRInput.Controller Right =
#if UNITY_ANDROID
            OVRInput.Controller.RTrackedRemote;

#else
            OVRInput.Controller.RTouch;
#endif

        internal const OVRInput.Axis2D TOUCHPAD_AXIS = OVRInput.Axis2D.PrimaryTouchpad | OVRInput.Axis2D.SecondaryTouchpad;
        internal const OVRInput.Axis1D TRIGGER_AXIS = OVRInput.Axis1D.PrimaryIndexTrigger | OVRInput.Axis1D.SecondaryIndexTrigger;
        internal const OVRInput.Touch TOUCHPAD_TOUCH = OVRInput.Touch.PrimaryTouchpad | OVRInput.Touch.SecondaryTouchpad;
        internal const OVRInput.Button TOUCHPAD_BUTTON = OVRInput.Button.PrimaryTouchpad | OVRInput.Button.SecondaryTouchpad;
        internal const OVRInput.Button TRIGGER_BUTTON = OVRInput.Button.PrimaryIndexTrigger | OVRInput.Button.SecondaryIndexTrigger;

        public OculusMotionControllerConfiguration()
        {
            AddButton(TOUCHPAD_BUTTON, InputButton.Left);
            AddButton(TRIGGER_BUTTON, InputButton.Right);
            AddButton(OVRInput.Button.Back, InputButton.Middle);
        }

        public override OVRInput.Controller? this[Hands hand]
        {
            get
            {
                if (hand == Hands.Left)
                {
                    return Left;
                }
                else if (hand == Hands.Right)
                {
                    return Right;
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