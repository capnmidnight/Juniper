using Juniper.Input;

using UnityEngine;

namespace Juniper.Input.Pointers.Motion
{

    public abstract class NoHandTracker : AbstractHandTracker<Unary, Unary, NoHandTrackerConfiguration>
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

        protected override void InternalUpdate()
        {
        }
    }
}
