using Juniper.Units;
using Juniper.Display;

using UnityEngine;

namespace Juniper.Widgets
{
    public class FollowMainCamera : MonoBehaviour
    {
        public float followDistance;
        private FollowObject f;

        public void Start()
        {
            // TODO: figure out what is best for different systems. This is just a placeholder for
            // now that works well in the Editor and on Daydream, but may not be suitable for other modalities.
            var f = this.Ensure<FollowObject>();

            f.Value.followObject = DisplayManager.MainCamera.transform;
            f.Value.FollowPosition = CartesianAxisFlags.XYZ;
            f.Value.Distance = followDistance;

            if(f.IsNew)
            {
#if STANDARD_DISPLAY
                f.Value.interpolate = false;
                f.Value.FollowThreshold = 0f;
                f.Value.FollowRotation = CartesianAxisFlags.XY;
                f.Value.RotationThreshold = Vector3.zero;
#else
                f.Value.interpolate = true;
                f.Value.FollowThreshold = 0.5f;
                f.Value.FollowRotation = CartesianAxisFlags.Y;
                f.Value.RotationThreshold = 10 * Vector3.up;
                f.Value.maxSpeed = 5;
                f.Value.maxRotationRate = 75;
#endif
            }

            this.f = f;
            this.f.Skip();
        }

        public void OnEnable()
        {
            if(f != null)
            {
                f.Skip();
            }
        }

        public void Update()
        {
            if (f != null && 
                followDistance != f.Distance)
            {
                var delta = Mathf.Abs(followDistance - f.Distance);
                var p = delta / followDistance;
                if (p > 0.01f)
                {
                    f.Distance = Mathf.Lerp(f.Distance, followDistance, 0.5f);
                }
                else
                {
                    f.Distance = followDistance;
                }
            }
        }
    }
}
