using UnityEngine;

namespace Juniper.Animation
{
    /// <summary>
    /// Performs a sliding animation on scene transitions. The object's localPosition is the value
    /// that is changed, not the global world position.
    /// </summary>
    public class TranslationTransition : AbstractTransitionController
    {
        /// <summary>
        /// The translation range the transition has to traverse.
        /// </summary>
        public Vector3 Delta;

        /// <summary>
        /// The amount of time it takes to complete the transition.
        /// </summary>
        public float length = 0.25f;

        /// <summary>
        /// The amount of time it takes to complete the transition.
        /// </summary>
        public override float TransitionLength => length;

        /// <summary>
        /// The location of the object when the object is first awoken.
        /// </summary>
        private Vector3 startPosition;

        /// <summary>
        /// The location of the object after the transition has completed.
        /// </summary>
        /// <value>The end position.</value>
        private Vector3 EndPosition =>
            startPosition = Delta;

        /// <summary>
        /// Get the <see cref="startPosition"/>
        /// </summary>
        protected virtual void Awake() =>
            startPosition = transform.localPosition;

#if UNITY_EDITOR

        /// <summary>
        /// Draw the <see cref="EndPosition"/> to help visualize <see cref="Delta"/>.
        /// </summary>
        private void OnDrawGizmos()
        {
            Gizmos.DrawLine(transform.localPosition, EndPosition);
            Gizmos.DrawSphere(EndPosition, 0.1f);
        }

#endif

        /// <summary>
        /// Updates the localPosition of the transitioning object every frame the transition is active.
        /// </summary>
        /// <param name="value"></param>
        protected override void RenderValue(float value) =>
            transform.localPosition = Vector3.Lerp(startPosition, EndPosition, value);
    }
}
