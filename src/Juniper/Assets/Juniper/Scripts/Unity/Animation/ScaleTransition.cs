using UnityEngine;

namespace Juniper.Animation
{
    /// <summary>
    /// Performs a shrinking/growing animation on scene transitions.
    /// </summary>
    public class ScaleTransition : AbstractTransitionController
    {
        /// <summary>
        /// The minimum size of the object, when it has completely exited.
        /// </summary>
        public float minScale = 0;

        /// <summary>
        /// The maximum size of the object, when it has finished entering.
        /// </summary>
        public float maxScale = 1;

        /// <summary>
        /// The amount of time it takes to complete the transition.
        /// </summary>
        public float length = 0.25f;

        /// <summary>
        /// The amount of time it takes to complete the transition.
        /// </summary>
        public override float TransitionLength => length;

        /// <summary>
        /// Updates the size of the transitioning object every frame the transition is active.
        /// </summary>
        /// <param name="value"></param>
        protected override void RenderValue(float value)
        {
            var s = Mathf.Max(0, minScale + value * DeltaScale);
            transform.localScale = s * Vector3.one;
        }

        /// <summary>
        /// The scale range the transition has to traverse.
        /// </summary>
        private float DeltaScale => maxScale - minScale;
    }
}
