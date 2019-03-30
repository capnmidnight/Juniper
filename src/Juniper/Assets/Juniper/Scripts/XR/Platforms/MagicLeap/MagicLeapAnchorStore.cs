#if UNITY_XR_MAGICLEAP
using System.Collections.Generic;

using UnityEngine;

using AnchorType = UnityEngine.XR.MagicLeap.MLPersistentBehavior;

namespace Juniper.Unity.Anchoring
{
    public abstract class MagicLeapAnchorStore : AbstractAnchorStore<AnchorType>
    {
        /// <summary>
        /// The collection in which anchors are stored. This value's type changes depending on
        /// certain compliation flags.
        /// </summary>
        private readonly Dictionary<string, AnchorType> anchorStore = new Dictionary<string, AnchorType>();

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
            return anchorStore[ID];
        }

        protected override AnchorType CreateAnchor(string ID, GameObject gameObject)
        {
            var anchor = gameObject.AddComponent<AnchorType>();
            anchor.UniqueId = ID;
            anchorStore.Add(ID, anchor);
            return anchor;
        }

        protected override AnchorType SynchronizeAnchor(string ID, GameObject gameObject)
        {
            var anchor = base.SynchronizeAnchor(ID, gameObject);
            if (anchor != null)
            {
                gameObject.transform.SetParent(anchor.transform, true);
            }
            return anchor;
        }

        protected override void DeleteAnchor(string ID, GameObject gameObject, AnchorType anchor)
        {
            gameObject.transform.SetParent(anchor.transform.parent, true);
            if (anchor.transform.childCount == 0)
            {
                anchorStore.Remove(ID);
                anchor.gameObject.Destroy();
            }
        }
    }
}
#endif