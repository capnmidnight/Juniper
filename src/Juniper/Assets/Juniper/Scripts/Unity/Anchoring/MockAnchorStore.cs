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
            var anchor = gameObject.EnsureComponent<MockWorldAnchor>().Value;
            anchor.state = LoadValue<MockWorldAnchor.Pose>(ID);
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
            var anchor = gameObject.EnsureComponent<MockWorldAnchor>().Value;
            anchor.state = new MockWorldAnchor.Pose
            {
                position = t.position,
                rotation = t.rotation
            };
            SaveValue(ID, anchor.state);
            return anchor;
        }
    }
}
