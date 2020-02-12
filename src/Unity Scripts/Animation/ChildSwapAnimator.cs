using System.Collections;

using UnityEngine;

namespace Juniper.Animation
{
    /// <summary>
    /// An animator for UIs that swap out child components as visible/invisible. Only one such
    /// component is allowed on a gameObject at a time.
    /// </summary>
    [DisallowMultipleComponent]
    public class ChildSwapAnimator : AbstractAnimator
    {
        /// <summary>
        /// The names of the child transforms to swap around.
        /// </summary>
        public string[] stateNames;

        /// <summary>
        /// Returns true if a transform named <paramref name="name"/> is a child of this transform.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public override bool HasState(string name)
        {
            return transform.Find(name) != null;
        }

        /// <summary>
        /// Sets the child transform named in <paramref name="name"/> to Active, while all other
        /// child transforms named in <see cref="stateNames"/> are set to Inactive.
        /// </summary>
        /// <param name="name"></param>
        /// <returns>An enumerator suitable for using in a Unity coroutine.</returns>
        public override IEnumerator PlayCoroutine(string name)
        {
            foreach (var key in stateNames)
            {
                var obj = this.Query(key);
                if (obj != null)
                {
                    obj.SetActive(key == name);
                }
            }
            yield return null;
        }
    }
}
