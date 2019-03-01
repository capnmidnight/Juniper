using System;
using System.Collections.Generic;

using Newtonsoft.Json;

using UnityEngine;

#if ARCORE
using GoogleARCore;
using AnchorType = GoogleARCore.Anchor;
#elif ARKIT
using UnityEngine.XR.iOS;
using AnchorType = UnityEngine.XR.iOS.UnityARUserAnchorComponent;
#elif HOLOLENS
using System.Linq;
using UnityEngine.XR.WSA.Persistence;
using AnchorType = UnityEngine.XR.WSA.Persistence.WorldAnchor;
#elif MAGIC_LEAP
using UnityEngine.XR.MagicLeap;
using AnchorType = UnityEngine.XR.MagicLeap.MLPersistentBehavior;
#else
using AnchorType = Juniper.XR.MockWorldAnchor;
#endif

namespace Juniper.Anchoring
{
    /// <summary>
    /// Anchors provide a means to restore the position of objects between sessions. In some AR
    /// systems, they also provide a means of prioritizing the stability of the location of specific
    /// objects. This component creates the native anchor type for any given subsystem, and save it
    /// to its native anchor store.
    /// </summary>
    [DisallowMultipleComponent]
    public class AnchorStore : MonoBehaviour
    {
        /// <summary>
        /// The collection in which anchors are stored. This value's type changes depending on
        /// certain compliation flags.
        /// </summary>
#if ARKIT
        private UnityARAnchorManager anchorManager;
#endif

#if HOLOLENS
        private WorldAnchorStore anchorStore;
#else
        private readonly Dictionary<string, AnchorType> anchorStore = new Dictionary<string, AnchorType>();
#endif

        /// <summary>
        /// Acquires the <see cref="anchorStore"/>
        /// </summary>
        public void Awake()
        {
#if ARKIT
            anchorManager = new UnityARAnchorManager();
#elif HOLOLENS
            WorldAnchorStore.GetAsync(new WorldAnchorStore.GetAsyncDelegate(store =>
                anchorStore = store));
#endif
        }

        /// <summary>
        /// Delete saved anchors, if there are any.
        /// </summary>
        public void ResetData()
        {
            PlayerPrefs.DeleteAll();
            anchorStore.Clear();
        }

        /// <summary>
        /// Anchors provide a means to restore the position of objects between sessions. In some AR
        /// systems, they also provide a means of prioritizing the stability of the location of
        /// specific objects. This method checks to see if a given anchor exists in the anchor data
        /// store. By default, Unity's PlayerPrefs KV store is used.
        /// </summary>
        /// <returns><c>true</c>, if anchor exists, <c>false</c> otherwise.</returns>
        /// <param name="anchorID">Anchor identifier.</param>
        public bool HasAnchor(string anchorID) =>
#if HOLOLENS
            anchorStore != null && anchorStore.GetAllIds().Contains(anchorID);
#else
            anchorStore.ContainsKey(anchorID);
#endif

        /// <summary>
        /// Anchors provide a means to restore the position of objects between sessions. In some AR
        /// systems, they also provide a means of prioritizing the stability of the location of
        /// specific objects. This method creates the native anchor type for any given subsystem, and
        /// save it to its native anchor store.
        /// </summary>
        /// <returns>
        /// <c>true</c>, if anchor existed on the object, existed in the datastore, or was
        /// successfully created, <c>false</c> otherwise.
        /// </returns>
        /// <param name="ID">Identifier.</param>
        /// <param name="gameObject">Game object.</param>
        public bool DropAnchor(string ID, GameObject gameObject)
        {
            if (string.IsNullOrEmpty(ID))
            {
                throw new ArgumentException("Parameter must have a value.", nameof(ID));
            }

            var scaleID = ID + "-scale";

            var anchor = GetAnchor(gameObject);
            if (anchor == null
#if HOLOLENS
                && anchorStore != null
#endif
               )
            {
                if (HasAnchor(ID))
                {
#if ARCORE || ARKIT || MAGIC_LEAP
                    anchor = anchorStore[ID];
#elif HOLOLENS
                    anchor = anchorStore.Load(ID, gameObject);
#else
                    var anchorStr = PlayerPrefs.GetString(ID);
                    anchor = gameObject.AddComponent<MockWorldAnchor>();

                    var anchorState = JsonConvert.DeserializeObject<MockWorldAnchor.Pose>(anchorStr);
                    anchor.state = anchorState;
#endif

                    if (PlayerPrefs.HasKey(scaleID))
                    {
                        gameObject.transform.localScale = JsonConvert.DeserializeObject<Vector3>(PlayerPrefs.GetString(scaleID));
                    }
                }
                else
                {
#if ARCORE
                    var pose = new Pose(
                        gameObject.transform.position,
                        gameObject.transform.rotation);
                    anchor = Session.CreateAnchor(pose);
                    anchorStore.Add(ID, anchor);
#elif ARKIT
                    anchor = gameObject.AddComponent<UnityARUserAnchorComponent>();
                    anchorStore.Add(ID, anchor);
#elif HOLOLENS
                    anchor = gameObject.AddComponent<UnityEngine.XR.WSA.WorldAnchor>();
                    anchorStore.Save(ID, anchor);
#elif MAGIC_LEAP
                    anchor = gameObject.AddComponent<MLPersistentBehavior>();
                    anchor.UniqueId = ID;
                    anchorStore.Add(ID, anchor);
#else
                    var t = gameObject.transform;
                    anchor = gameObject.AddComponent<MockWorldAnchor>();
                    anchor.state = new MockWorldAnchor.Pose
                    {
                        position = t.position,
                        rotation = t.rotation
                    };
                    PlayerPrefs.SetString(ID, JsonConvert.SerializeObject(anchor.state));
#endif

                    PlayerPrefs.SetString(scaleID, JsonConvert.SerializeObject(gameObject.transform.localScale));
                }

#if ARCORE || ARKIT || MAGIC_LEAP
                if (anchor != null)
                {
                    gameObject.transform.SetParent(anchor.transform, true);
                }
#endif
            }

#if HOLOLENS
            return anchor?.isLocated == true;
#else
            return anchor != null;
#endif
        }

        /// <summary>
        /// Anchors provide a means to restore the position of objects between sessions. In some AR
        /// systems, they also provide a means of prioritizing the stability of the location of
        /// specific objects. This method removes the anchor from the gameObject and delete it out of
        /// their native anchor store.
        /// </summary>
        /// <param name="ID">Identifier.</param>
        /// <param name="gameObject">Game object.</param>
        public void WeighAnchor(string ID, GameObject gameObject)
        {
            var anchor = GetAnchor(gameObject);
            if (anchor != null)
            {
#if ARCORE || ARKIT || MAGIC_LEAP
                gameObject.transform.SetParent(anchor.transform.parent, true);
                if (anchor.transform.childCount == 0)
                {
                    anchorStore.Remove(ID);
                    anchor.gameObject.Destroy();
                }
#elif HOLOLENS
                anchor.Destroy();
                anchorStore.Delete(ID);
#else
                PlayerPrefs.DeleteKey(ID);
                anchor.Destroy();
#endif

                PlayerPrefs.DeleteKey(ID + "-scale");
            }
        }

        private static AnchorType GetAnchor(GameObject gameObject)
        {
#if ARCORE || ARKIT
            gameObject = gameObject.transform.parent?.gameObject;
#endif
            return gameObject?.GetComponent<AnchorType>();
        }
    }
}
