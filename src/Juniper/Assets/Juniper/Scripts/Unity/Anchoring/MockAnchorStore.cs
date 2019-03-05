using System.Collections.Generic;

using UnityEngine;

using AnchorType = Juniper.Unity.Anchoring.MockWorldAnchor;

namespace Juniper.Unity.Anchoring
{
    public abstract class MockAnchorStore : AbstractAnchorStore<AnchorType>
    {
        private Dictionary<string, AnchorType> anchorStore = new Dictionary<string, AnchorType>();

        public override bool HasAnchor(string anchorID)
        {
            return anchorStore.ContainsKey(anchorID);
        }

        /// <summary>
        /// Delete saved anchors, if there are any.
        /// </summary>
        public override void ResetData()
        {
            base.ResetData();

            anchorStore.Clear();
        }

        protected override AnchorType LoadAnchor(string ID)
        {
            var anchor = gameObject.AddComponent<MockWorldAnchor>();
            anchor.state = LoadValue<MockWorldAnchor.Pose>(ID);
            return anchor;
        }


        protected override AnchorType CreateAnchor(string ID, GameObject gameObject)
        {
            var t = gameObject.transform;
            var anchor = gameObject.AddComponent<MockWorldAnchor>();
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
