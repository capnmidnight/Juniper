#if UNITY_XR_WINDOWSMR_METRO && WINDOWSMR

#if UNITY_EDITOR
using HapticsType = Juniper.Unity.Haptics.NoHaptics;
#else
using HapticsType = Juniper.Unity.Haptics.WindowsMRHaptics;
#endif

namespace Juniper.Unity.Input.Pointers.Motion
{
    /// <summary>
    /// A motion controller or hand-tracking. Currently only implements WindowsMR.
    /// </summary>
    public abstract class WindowsMRMotionController : AbstractWindowsMRDevice<WindowsMRMotionControllerConfiguration, HapticsType>
    {
#if !UNITY_EDITOR
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

            Haptics.ControllerID = ControllerID;
        }
#endif

        protected override bool TouchPadTouched { get { return InputState.touchpadTouched; } }

        protected override bool TouchPadTouchedDown { get { return TouchPadTouched && !lastInputState.touchpadTouched; } }

        protected override bool TouchPadTouchedUp { get { return !TouchPadTouched && lastInputState.touchpadTouched; } }

        protected override bool TouchPadPressed { get { return InputState.touchpadPressed; } }

        protected override bool TouchPadPressedDown { get { return TouchPadPressed && !lastInputState.touchpadPressed; } }

        protected override bool TouchPadPressedUp { get { return !TouchPadPressed && lastInputState.touchpadPressed; } }

        public override Vector2 SquareTouchPoint { get { return InputState.touchpadPosition; } }

        public override Vector2 RoundTouchPoint { get { return SquareTouchPoint.Square2Round(); } }

        public override float Trigger
        {
            get
            {
                return InputState.selectPressedAmount;
            }
        }
    }
}
#endif
