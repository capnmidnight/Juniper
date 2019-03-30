using Juniper.Input;
using Juniper.Unity.Haptics;

using UnityEngine;

namespace Juniper.Unity.Input.Pointers.Motion
{
    public class NoMotionControllerConfiguration : AbstractMotionControllerConfiguration<Unary, KeyCode>
    {
        public override Unary? this[Hands hand]
        {
            get
            {
                return null;
            }
        }
    }

    public abstract class NoMotionController : AbstractMotionController<Unary, KeyCode, NoMotionControllerConfiguration, NoHaptics>
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

        public override bool IsButtonDown(KeyCode button)
        {
            return false;
        }

        public override bool IsButtonPressed(KeyCode button)
        {
            return false;
        }

        public override bool IsButtonUp(KeyCode button)
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
    }
}