using System.Collections;

using UnityEngine;

namespace Juniper.Animation
{
    /// <summary>
    /// An abstraction layer over different ways of showing UI state. Only one such component is
    /// allowed on a gameObject at a time.
    /// </summary>
    [DisallowMultipleComponent]
    public abstract class AbstractAnimator : MonoBehaviour
    {
        /// <summary>
        /// Creates the right Animator subclass for a given gameObject. If the gameObject has a Unity
        /// Animator on it, GetAnimator will create a <see cref="UnityAnimator"/>. If the gameObject
        /// has child transforms that match the Interaction system's state names, then GetAnimator
        /// will create a <see cref="ChildSwapAnimator"/>.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="parts"></param>
        /// <returns></returns>
        public static AbstractAnimator GetAnimator(GameObject parent, string[] parts)
        {
            var animator = parent.GetComponent<AbstractAnimator>();
            var defaultState = parts[0];
            if (animator == null)
            {
#if UNITY_MODULES_ANIMATION
                var anim = parent.GetComponent<Animator>();
                if (anim != null && UnityAnimator.AnimatorHasState(anim, defaultState))
                {
                    animator = parent.EnsureComponent<UnityAnimator>().Value;
                }
                else
#endif
                if (parent.transform.childCount > 0 && parent.transform.Find(defaultState))
                {
                    var swapper = parent.EnsureComponent<ChildSwapAnimator>().Value;
                    swapper.stateNames = parts;
                    animator = swapper;
                }
            }

            return animator;
        }

        /// <summary>
        /// Execute an animation and return an IEnumerator that can be used to yield in a coroutine
        /// for asynchronous execution.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public abstract IEnumerator PlayCoroutine(string name);

        /// <summary>
        /// Play an animation immediately, with no chance to wait for its completion.
        /// </summary>
        /// <param name="name"></param>
        public void Play(string name)
        {
            StartCoroutine(PlayCoroutine(name));
        }

        /// <summary>
        /// Returns true in child classes to indicate they support the given state message.
        /// </summary>
        /// <param name="name">The animation state to check for</param>
        /// <returns>True or False</returns>
        public abstract bool HasState(string name);
    }
}
