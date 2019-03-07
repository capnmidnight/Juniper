#if UNITY_XR_WINDOWSMR_METRO && HOLOLENS
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Linq;
using UnityEngine.XR.WSA.Persistence;
using AnchorType = UnityEngine.XR.WSA.Persistence.WorldAnchor;
using UnityEngine;

namespace Juniper.Unity.Anchoring
{
    public abstract class HoloLensAnchorStore : AbstractAnchorStore<AnchorType>
    {
        /// <summary>
        /// The collection in which anchors are stored. This value's type changes depending on
        /// certain compliation flags.
        /// </summary>
        private WorldAnchorStore anchorStore;

        /// <summary>
        /// Acquires the <see cref="anchorStore"/>
        /// </summary>
        public void Awake()
        {
            WorldAnchorStore.GetAsync(new WorldAnchorStore.GetAsyncDelegate(store =>
                anchorStore = store));
        }

        public override bool HasAnchor(string anchorID)
        {
            return anchorStore != null && anchorStore.GetAllIds().Contains(anchorID);
        }

        protected override bool IsSaved(AnchorType anchor)
        {
            return base.IsSaved(anchor) || anchorStore == null;
        }

        protected override AnchorType LoadAnchor(string ID)
        {
            return anchorStore.Load(ID, gameObject);
        }

        protected override bool IsGood(AnchorType anchor)
        {
            return base.IsGood(anchor) && anchor.isLocated;
        }

        protected override void DeleteAnchor(string ID, GameObject gameObject, AnchorType anchor)
        {
            anchor.Destroy();
            anchorStore.Delete(ID);
        }
    }
}
#endif
