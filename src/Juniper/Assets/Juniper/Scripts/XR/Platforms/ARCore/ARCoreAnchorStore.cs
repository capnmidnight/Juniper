#if UNITY_XR_ARCORE
using System.Collections.Generic;
using GoogleARCore;
using UnityEngine;

namespace Juniper.Unity.Anchoring
{
    public class ARCoreAnchorStore : AbstractAnchorStore<Anchor>
    {
        /// <summary>
        /// The collection in which anchors are stored. This value's type changes depending on
        /// certain compliation flags.
        /// </summary>
        private readonly Dictionary<string, Anchor> anchorStore = new Dictionary<string, Anchor>(10);

        private ARCoreSession session;

        public void Start()
        {
            session = ComponentExt.FindAny<ARCoreSession>();
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

        protected override Anchor FindAnchor(GameObject gameObject)
        {
            return base.FindAnchor(gameObject?.transform?.parent?.gameObject);
        }

        protected override Anchor LoadAnchor(string ID)
        {
            return anchorStore[ID];
        }

        protected override Anchor CreateAnchor(string ID, GameObject gameObject)
        {
            var pose = new Pose(
                gameObject.transform.position,
                gameObject.transform.rotation);
            var anchor = Session.CreateAnchor(pose);

            anchorStore[ID] = anchor;

            return anchor;
        }

        protected override Anchor SynchronizeAnchor(string ID, GameObject gameObject)
        {
            var anchor = base.SynchronizeAnchor(ID, gameObject);
            if (anchor != null)
            {
                gameObject.transform.SetParent(anchor.transform, true);
            }
            return anchor;
        }

        protected override void DeleteAnchor(string ID, GameObject gameObject, Anchor anchor)
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
