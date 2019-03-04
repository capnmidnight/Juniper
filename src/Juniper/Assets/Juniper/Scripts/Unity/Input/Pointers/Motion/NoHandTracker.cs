using UnityEngine;

namespace Juniper.Input.Pointers.Motion
{
    public class NoHandTrackerConfiguration : AbstractHandTrackerConfiguration<Unary, KeyCode>
    {
        public override Unary? this[Hands hand]
        {
            get
            {
                return null;
            }
        }
    }

    public abstract class NoHandTracker : AbstractHandTracker<Unary, KeyCode, NoHandTrackerConfiguration>
    {
        public override bool IsConnected
        {
            get
            {
                return false;
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

        protected override void InternalUpdate()
        {
        }
    }
}
