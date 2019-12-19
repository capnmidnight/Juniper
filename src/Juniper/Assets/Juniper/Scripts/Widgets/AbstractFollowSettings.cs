using System.Linq;
using Juniper.Units;
using Juniper.XR;

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

        public float maxSpeed = 1;

        /// <summary>
        /// The distance to maintain from the target object.
        /// </summary>
        public float Distance = 0;

        /// <summary>
        /// Follow the target's rotation in the X-Axis.
        /// </summary>
        [Header("Rotation")]
        public CartesianAxisFlags FollowRotation;

        public bool interpolateRotation = true;

        /// <summary>
        /// The maximum allowable deviation in angles from the target's rotation..
        /// </summary>
        public Vector3 FollowRotationThreshold = 10 * Vector3.up;

        public float maxRotationRate = 10;

        public void Copy(AbstractFollowSettings other)
        {
            FollowPosition = other.FollowPosition;
            interpolatePosition = other.interpolatePosition;
            FollowPositionThreshold = other.FollowPositionThreshold;
            maxSpeed = other.maxSpeed;
            Distance = other.Distance;
            FollowRotation = other.FollowRotation;
            interpolateRotation = other.interpolateRotation;
            FollowRotationThreshold = other.FollowRotationThreshold;
            maxRotationRate = other.maxRotationRate;
        }
    }
}
