using Juniper.Haptics;

using UnityEngine;

namespace Juniper.Input.Pointers.Motion
{
    public class NoMotionControllerConfiguration : AbstractMotionControllerConfiguration<Unary, KeyCode>
    {
        public override Unary? this[Hands hand] => null;
    }

    public abstract class NoMotionController : AbstractMotionController<Unary, KeyCode, NoMotionControllerConfiguration, NoHaptics>
    {
        public override bool IsConnected => false;

        public override Vector2 SquareTouchPoint => Vector2.zero;

        public override Vector2 RoundTouchPoint => Vector2.zero;

        public override bool IsDominantHand => false;

        public override bool IsButtonDown(KeyCode button) => false;

        public override bool IsButtonPressed(KeyCode button) => false;

        public override bool IsButtonUp(KeyCode button) => false;

        protected override bool TouchPadTouched => false;

        protected override bool TouchPadTouchedDown => false;

        protected override bool TouchPadTouchedUp => false;

        protected override bool TouchPadPressed => false;

        protected override bool TouchPadPressedDown => false;

        protected override bool TouchPadPressedUp => false;
    }
}
