#if GOOGLEVR

using Juniper.Input;

using InputButton = UnityEngine.EventSystems.PointerEventData.InputButton;

namespace Juniper.Input.Pointers.Motion
{
    public class DaydreamPointerConfiguration : AbstractMotionControllerConfiguration<GvrControllerHand, GvrControllerButton>
    {
        public DaydreamPointerConfiguration()
        {
            AddButton(VirtualTouchPadButton.Top, InputButton.Left);
            AddButton(VirtualTouchPadButton.Bottom, InputButton.Right);
            AddButton(GvrControllerButton.App, InputButton.Middle);
        }

        public override GvrControllerHand? this[Hands hand]
        {
            get
            {
                if (hand == Hands.Left)
                {
                    return GvrControllerHand.Left;
                }
                else if (hand == Hands.Right)
                {
                    return GvrControllerHand.Right;
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