#if UNITY_XR_WINDOWSMR_METRO && WINDOWSMR
using UnityEngine.XR.WSA.Input;

using UnityInput = UnityEngine.Input;
using InputButton = UnityEngine.EventSystems.PointerEventData.InputButton;

using System.Linq;

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

    /// <summary>
    /// A motion controller or hand-tracking. Currently only implements WindowsMR.
    /// </summary>
    public abstract class WindowsMRMotionController : AbstractWindowsMRDevice<WindowsMRMotionControllerConfiguration, HapticsType>
    {
        private uint ControllerID;

        public override Hands Hand
        {
            get
            {
                return base.Hand;
            }

            set
            {
                base.Hand = value;
                ControllerID = (from state in InteractionManager.GetCurrentReading()
                                where state.source.handedness == NativeHandID
                                select state.source.id)
                            .FirstOrDefault();
            }
        }

        public override void Awake()
        {
            base.Awake();

#if !UNITY_EDITOR
            Haptics.ControllerID = ControllerID;
#endif
        }

        public override float Trigger
        {
            get
            {
                return UnityInput.GetAxisRaw(IsLeftHand ? "Joystick9" : "Joystick10");
            }
        }
    }
}
#endif
