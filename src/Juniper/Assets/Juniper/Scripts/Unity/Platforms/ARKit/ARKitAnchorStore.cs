#if ARKIT
using UnityEngine;
using AnchorType = UnityEngine.XR.iOS.UnityARUserAnchorComponent;

namespace Juniper.Unity.Anchoring
{
    public abstract class ARKitAnchorStore : AbstractAnchorStore<AnchorType>
    {
        /// <summary>
        /// The collection in which anchors are stored. This value's type changes depending on
        /// certain compliation flags.
        /// </summary>
        private UnityARAnchorManager anchorStore;

        /// <summary>
        /// Acquires the <see cref="anchorStore"/>
        /// </summary>
        public void Awake()
        {
            anchorStore = new UnityARAnchorManager();
        }

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

        protected override AnchorType FindAnchor(GameObject gameObject)
        {
            return base.FindAnchor(gameObject?.transform?.parent?.gameObject);
        }

        protected override AnchorType LoadAnchor(string ID)
        {
            return anchorStore[ID];
        }


        protected virtual AnchorType CreateAnchor(string ID, GameObject gameObject)
        {
            var anchor = gameObject.AddComponent<UnityARUserAnchorComponent>();
            anchorStore.Add(ID, anchor);
            return anchor;
        }

        protected override AnchorType SaveAnchor(string ID, GameObject gameObject)
        {
            var anchor = base.SaveAnchor(ID, gameObject);
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