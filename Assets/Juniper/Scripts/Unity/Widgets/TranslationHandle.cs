using UnityEngine;

namespace Juniper.Widgets
{
    /// <summary>
    /// A draggable object that can be used to translate the position of another object.
    /// </summary>
    public class TranslationHandle : AbstractConstraintHandle
    {
        /// <summary>
        /// Calculates the amount of change that occured, given the handle's current and previous location.
        /// </summary>
        /// <returns>The change.</returns>
        protected override Vector3 GetChange() =>
            transform.localPosition - lastPosition;

        /// <summary>
        /// Performs the necessary translation change, depending on the constrained return value of
        /// <see cref="GetChange"/>
        /// </summary>
        /// <param name="v">V.</param>
        protected override void ApplyChange(Vector3 v)
        {
            target.transform.position += speed * (transform.rotation * v);
            transform.localPosition = lastPosition += (1 - speed) * v;
        }
    }
}
