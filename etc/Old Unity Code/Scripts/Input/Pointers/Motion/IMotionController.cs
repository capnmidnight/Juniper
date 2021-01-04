using UnityEngine;

namespace Juniper.Input.Pointers.Motion
{
    public interface IMotionController : IHandedPointer
    {
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
