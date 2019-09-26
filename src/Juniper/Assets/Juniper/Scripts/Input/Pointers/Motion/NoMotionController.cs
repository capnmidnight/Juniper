using Juniper.Input;
using Juniper.Haptics;

using UnityEngine;

namespace Juniper.Input.Pointers.Motion
{
    public abstract class NoMotionController : AbstractMotionController<Unary, Unary, NoMotionControllerConfiguration>
    {
        public override bool IsConnected
        {
            get
            {
                return false;
            }
        }

        public override Vector2 SquareTouchPoint
        {
            get
            {
                return Vector2.zero;
            }
        }

        public override Vector2 RoundTouchPoint
        {
            get
            {
                return Vector2.zero;
            }
        }

        public override bool IsDominantHand
        {
            get
            {
                return false;
            }
        }

        public override bool IsButtonDown(Unary button)
        {
            return false;
        }

        public override bool IsButtonPressed(Unary button)
        {
            return false;
        }

        public override bool IsButtonUp(Unary button)
        {
            return false;
        }

        protected override bool TouchPadTouched
        {
            get
            {
                return false;
            }
        }

        protected override bool TouchPadTouchedDown
        {
            get
            {
                return false;
            }
        }

        protected override bool TouchPadTouchedUp
        {
            get
            {
                return false;
            }
        }

        protected override bool TouchPadPressed
        {
            get
            {
                return false;
            }
        }

        protected override bool TouchPadPressedDown
        {
            get
            {
                return false;
            }
        }

        protected override bool TouchPadPressedUp
        {
            get
            {
                return false;
            }
        }

        public override float Trigger
        {
            get;
        }

        protected override AbstractHapticDevice MakeHapticsDevice()
        {
            return this.Ensure<NoHaptics>();
        }
    }
}
