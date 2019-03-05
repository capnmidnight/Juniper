using UnityEngine;

namespace Juniper.Unity.Widgets
{
    /// <summary>
    /// A draggable object that can be used to update the rotation of another object.
    /// </summary>
    public class RotationHandle : AbstractConstraintHandle
    {
        /// <summary>
        /// Get the amount of rotation that needs to occur to update the dragging handle from its
        /// previous position to its current position, as a set of Euler angles.
        /// </summary>
        /// <returns>The change.</returns>
        protected override Vector3 GetChange()
        {
            return Quaternion.FromToRotation(lastPosition, transform.localPosition).eulerAngles;
        }

        /// <summary>
        /// Applies the constrained rotation from <see cref="GetChange"/> to the target object.
        /// </summary>
        /// <param name="v">V.</param>
        protected override void ApplyChange(Vector3 v)
        {
            var q = Quaternion.Euler(v);
            var r = Quaternion.Slerp(Quaternion.identity, q, speed);
            var s = Quaternion.Slerp(Quaternion.identity, q, (1 - speed));
            target.transform.rotation *= r;
            transform.localPosition = lastPosition = s * lastPosition;
        }
    }
}
