#if UNITY_XR_OCULUS

using UnityEngine;

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
            AddButton(TRIGGER_BUTTON, KeyCode.Mouse0);
            AddButton(TOUCHPAD_BUTTON, KeyCode.Mouse1);
            AddButton(OVRInput.Button.Back, KeyCode.Escape);
        }

        public override OVRInput.Controller? this[Hand hand]
        {
            get
            {
                if (hand == Hand.Left)
                {
                    return Left;
                }
                else if (hand == Hand.Right)
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