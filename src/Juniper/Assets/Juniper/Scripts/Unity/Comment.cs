using UnityEngine;

namespace Juniper.Unity
{
    /// <summary>
    /// Attach this component to gameObjects solely for the purpose of leaving a comment in the Unity
    /// Editor on that gameObject.
    /// </summary>
    public class Comment : MonoBehaviour
    {
        /// <summary>
        /// The comment to leave on the gameObject.
        /// </summary>
        [TextArea]
        public string comment;
    }
}