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
        public override float TransitionLength
        {
            get
            {
                return length;
            }
        }

        /// <summary>
        /// The location of the object when the object is first awoken.
        /// </summary>
        [HideInNormalInspector]
        [SerializeField]
        private Vector3 StartPosition;

        /// <summary>
        /// The location of the object after the transition has completed.
        /// </summary>
        /// <value>The end position.</value>
        [HideInNormalInspector]
        [SerializeField]
        private Vector3 EndPosition;

#if UNITY_EDITOR

        private void OnValidate()
        {
            if (Tween.IsContinuous(tween))
            {
                StartPosition = transform.localPosition;
                EndPosition = StartPosition + Delta;
            }
            else
            {
                EndPosition = transform.localPosition;
                StartPosition = EndPosition - Delta;
            }
        }

        /// <summary>
        /// Draw the <see cref="EndPosition"/> to help visualize <see cref="Delta"/>.
        /// </summary>
        private void OnDrawGizmos()
        {
            var start = transform.localToWorldMatrix.MultiplyPoint(StartPosition);
            var end = transform.localToWorldMatrix.MultiplyPoint(EndPosition);
            Gizmos.DrawLine(start, end);
            Gizmos.DrawSphere(end, 0.1f);
            if(Tween.IsContinuous(tween))
            {
                end = transform.localToWorldMatrix.MultiplyPoint(StartPosition - Delta);
                Gizmos.DrawLine(start, end);
                Gizmos.DrawSphere(end, 0.1f);
            }
        }

#endif

        /// <summary>
        /// Updates the localPosition of the transitioning object every frame the transition is active.
        /// </summary>
        /// <param name="value"></param>
        protected override void RenderValue(float value)
        {
            transform.localPosition = Vector3.LerpUnclamped(StartPosition, EndPosition, value);
        }
    }
}
