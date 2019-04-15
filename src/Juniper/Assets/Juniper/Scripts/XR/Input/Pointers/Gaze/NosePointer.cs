using Juniper.Unity.Haptics;

using UnityEngine;

namespace Juniper.Unity.Input.Pointers.Gaze
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
