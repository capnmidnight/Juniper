using Juniper.XR;

using UnityEngine;

namespace Juniper.Input.Pointers.Motion
{
    public class NoHandTrackerConfiguration : AbstractHandTrackerConfiguration<Unary, KeyCode> {
        public override Unary? this[Hands hand] => null;
    }

    public abstract class NoHandTracker : AbstractHandTracker<Unary, KeyCode, NoHandTrackerConfiguration>
    {
        public override bool IsConnected => false;

        public override bool IsDominantHand => false;

        public override bool IsButtonDown(KeyCode button) => false;

        public override bool IsButtonPressed(KeyCode button) => false;

        public override bool IsButtonUp(KeyCode button) => false;

        protected override void InternalUpdate() {}
    }
}
