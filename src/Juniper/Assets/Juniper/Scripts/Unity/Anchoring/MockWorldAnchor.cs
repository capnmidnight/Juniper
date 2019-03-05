using System;

using UnityEngine;

namespace Juniper.Unity.Anchoring
{
    /// <summary>
    /// Anchors provide a means to restore the position of objects between sessions. In some AR
    /// systems, they also provide a means of prioritizing the stability of the location of specific
    /// objects. This class is a default implementation for systems that do not have a native AR
    /// Anchor type.
    /// </summary>
    public class MockWorldAnchor : MonoBehaviour
    {
        /// <summary>
        /// The position, rotation, and scale to which the gameObject should be held.
        /// </summary>
        public Pose state;

        /// <summary>
        /// Reset the position, rotation, and scale of the object in case someone has tried to move it.
        /// </summary>
        public void Update()
        {
            transform.position = state.position;
            transform.rotation = state.rotation;
        }

        /// <summary>
        /// A serializable set of position, rotation, and scale values.
        /// </summary>
        [Serializable]
        public class Pose
        {
            /// <summary>
            /// The cartesion position of the pose.
            /// </summary>
            public Vector3 position;

            /// <summary>
            /// The orientation of the pose.
            /// </summary>
            public Quaternion rotation;
        }
    }
}
