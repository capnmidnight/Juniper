using Juniper.Input;

using UnityEngine;

namespace Juniper.Input.Pointers.Motion
{
    public interface IMotionController : IHandedPointer
    {
        bool IsButtonPressed(VirtualTouchPadButton button);

        bool IsButtonDown(VirtualTouchPadButton button);

        bool IsButtonUp(VirtualTouchPadButton button);

        bool IsButtonDown(VirtualTriggerButton button);

        bool IsButtonPressed(VirtualTriggerButton button);

        bool IsButtonUp(VirtualTriggerButton button);

        float Trigger
        {
            get;
        }

        Vector2 RoundTouchPoint
        {
            get;
        }

        Vector2 SquareTouchPoint
        {
            get;
        }
    }
}
