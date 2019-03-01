using UnityEngine;

namespace Juniper.Widgets
{
    /// <summary>
    /// A constraint handle that is used to scale the size of a target object.
    /// </summary>
    public class ScaleHandle : AbstractConstraintHandle
    {
        /// <summary>
        /// Calculate the amount of scale for a given transform position.
        /// </summary>
        /// <returns>The change.</returns>
        protected override Vector3 GetChange() =>
            transform.localPosition - lastPosition;

        /// <summary>
        /// Scale the target object according to the provided vector.
        /// </summary>
        /// <param name="v">V.</param>
        protected override void ApplyChange(Vector3 v)
        {
            target.transform.localScale += speed * v;
            transform.localPosition = lastPosition += (1 - speed) * v;
        }
    }
}
