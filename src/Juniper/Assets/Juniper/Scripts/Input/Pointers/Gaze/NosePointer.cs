using Juniper.Haptics;

using UnityEngine;

namespace Juniper.Input.Pointers.Gaze
{
    public abstract class NosePointer :
        AbstractGazePointer
    {
        public override Vector3 WorldPoint
        {
            get
            {
                return WorldFromViewport(VIEWPORT_MIDPOINT);
            }
        }
    }
}
