#if UNITY_MODULES_ANIMATION

using System;
using System.Collections;

using UnityEngine;

namespace Juniper.Animation
{
    /// <summary>
    /// Abstract over Unity's Animator class for 3D models. Requires an Animator object on the
    /// current gameObject. Only one such component is allowed on a gameObject at a time.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Animator))]
    public class UnityAnimator : AbstractAnimator
    {
        /// <summary>
        /// Check to see if the current animation has the desired clip.
        /// </summary>
        /// <param name="animator"></param>
        /// <param name="name">    </param>
        /// <returns></returns>
        public static bool AnimatorHasState(Animator animator, string name)
        {
            if (animator != null
                && animator.runtimeAnimatorController != null
                && animator.runtimeAnimatorController.animationClips != null)
            {
                foreach (var clip in animator.runtimeAnimatorController.animationClips)
                {
                    if (clip.name == name)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Get the <see cref="Animator"/> on the current gameObject.
        /// </summary>
        public void Awake()
        {
            animator = GetComponent<Animator>();
        }

        /// <summary>
        /// Returns true in child classes to indicate they support the given state message.
        /// </summary>
        /// <param name="name">The animation state to check for</param>
        /// <returns>True or False</returns>
        public override bool HasState(string name)
        {
            return AnimatorHasState(animator, name);
        }

        /// <summary>
        /// Execute an animation and return an IEnumerator that can be used to yield in a coroutine
        /// for asynchronous execution.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public override IEnumerator PlayCoroutine(string name)
        {
            if (animator != null
                && animator.runtimeAnimatorController != null)
            {
                animator.Play(name);
                var animationState = animator.GetCurrentAnimatorStateInfo(0);
                var len = animationState.length;
                yield return Wait.Seconds(len);

                animationState = animator.GetCurrentAnimatorStateInfo(0);
                yield return Wait.Seconds(animationState.length - len);
            }
        }

        /// <summary>
        /// The animator over which we are abstracting.
        /// </summary>
        private Animator animator;
    }
}

#endif