using Juniper.Units;

using UnityEngine;

namespace Juniper.Widgets
{
    /// <summary>
    /// Make one object's pose follow the pose of another object.
    /// </summary>
    public abstract class AbstractFollowSettings : MonoBehaviour
    {
        [Header("Position")]
        public CartesianAxisFlags FollowPosition = CartesianAxisFlags.XZ;

        public bool interpolatePosition = true;

        /// <summary>
        /// The maximum allowable deviation in distance from the target's position.
        /// </summary>
        public float FollowPositionThreshold = 0;

        public float speedConstant = 1;

        public float speedProportion = 0;

        /// <summary>
        /// The distance to maintain from the target object.
        /// </summary>
        public float Distance = 0;

        /// <summary>
        /// Follow the target's rotation in the X-Axis.
        /// </summary>
        [Header("Rotation")]
        public CartesianAxisFlags FollowRotation;

        public void Copy(AbstractFollowSettings other)
        {
            FollowPosition = other.FollowPosition;
            interpolatePosition = other.interpolatePosition;
            FollowPositionThreshold = other.FollowPositionThreshold;
            speedConstant = other.speedConstant;
            speedProportion = other.speedProportion;
            Distance = other.Distance;
            FollowRotation = other.FollowRotation;
        }
    }
}
