using UnityEngine;

namespace Juniper.Animation
{
    /// <summary>
    /// Performs a shrinking/growing animation on scene transitions.
    /// </summary>
    [DisallowMultipleComponent]
    public class AlphaTransition : AbstractTransitionController
    {
        /// <summary>
        /// The minimum opacity of the object, where 0 = completely transparent.
        /// </summary>
        public float minAlpha = 0;

        /// <summary>
        /// The maximum opacity of the object, where 1 = completely opaque.
        /// </summary>
        public float maxAlpha = 1;

        /// <summary>
        /// The amount of time it takes to complete the transition.
        /// </summary>
        public float length = 0.25f;

        /// <summary>
        /// The amount of time it takes to complete the transition.
        /// </summary>
        public override float TransitionLength
        {
            get
            {
                return length;
            }
        }

        /// <summary>
        /// The scale range the transition has to traverse.
        /// </summary>
        protected float DeltaAlpha
        {
            get
            {
                return maxAlpha - minAlpha;
            }
        }

        /// <summary>
        /// Updates the size of the transitioning object every frame the transition is active.
        /// </summary>
        /// <param name="value"></param>
        protected override void RenderValue(float value)
        {
            if (transitionMaterial == null)
            {
                transitionMaterial = GetComponent<Renderer>()?.GetMaterial();
            }

            if (transitionMaterial != null)
            {
                var o = Mathf.Clamp01(minAlpha + value * DeltaAlpha);
                transitionMaterial.SetFloat("_Alpha", o);
            }
        }

        /// <summary>
        /// The material on which the opacity is being set.
        /// </summary>
        private Material transitionMaterial;
    }
}
