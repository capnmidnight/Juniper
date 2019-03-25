using UnityEngine;

namespace Juniper.Unity.Anchoring
{
    /// <summary>
    /// An anchor store that stores all anchors in the default data store for AbstractAnchorStore.
    /// </summary>
    public abstract class MockAnchorStore : AbstractAnchorStore<MockWorldAnchor>
    {
        /// <summary>
        /// Returns true if the anchorID is already in the anchor store.
        /// </summary>
        /// <param name="ID">The anchor ID to search for.</param>
        /// <returns>True if the anchor exists in the anchor store, false otherwise.</returns>
        public override bool HasAnchor(string ID)
        {
            return HasValue(ID);
        }

        /// <summary>
        /// Loads an anchor out of the default data store.
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        protected override MockWorldAnchor LoadAnchor(string ID)
        {
            var anchor = gameObject.EnsureComponent<MockWorldAnchor>();
            var arr = LoadValue<float[]>(ID);
            anchor.Value.state = new Pose(
                new Vector3(arr[0], arr[1], arr[2]),
                new Quaternion(arr[4], arr[5], arr[6], arr[7]));
            return anchor;
        }

        /// <summary>
        /// Creates an anchor and saves it in the default anchor store.
        /// </summary>
        /// <param name="ID">The name of the anchor to create and save</param>
        /// <param name="gameObject">The object to be anchored</param>
        /// <returns>The new anchor</returns>
        protected override MockWorldAnchor CreateAnchor(string ID, GameObject gameObject)
        {
            var t = gameObject.transform;
            var anchor = gameObject.EnsureComponent<MockWorldAnchor>();
            anchor.Value.state = new Pose
            {
                position = t.position,
                rotation = t.rotation
            };
            var arr = new float[]
            {
                t.position.x, t.position.y, t.position.z,
                t.rotation.x, t.rotation.y, t.rotation.z, t.rotation.w
            };
            SaveValue(ID, arr);
            return anchor;
        }
    }
}
