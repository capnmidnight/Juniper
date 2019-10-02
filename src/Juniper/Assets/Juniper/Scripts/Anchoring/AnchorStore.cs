using System;
using System.Linq;

using Juniper.IO;

using UnityEngine;

namespace Juniper.Anchoring
{
    /// <summary>
    /// Anchors provide a means to restore the position of objects between sessions. In some AR
    /// systems, they also provide a means of prioritizing the stability of the location of specific
    /// objects. This component creates the native anchor type for any given subsystem, and save it
    /// to its native anchor store.
    /// </summary>
    [DisallowMultipleComponent]
    public class AnchorStore :
#if UNITY_XR_ARCORE
        ARCoreAnchorStore
#elif UNITY_XR_ARKIT
        ARKitAnchorStore
#elif UNITY_XR_MAGICLEAP
        MagicLeapAnchorStore
#elif UNITY_XR_WINDOWSMR_METRO
        WindowsMRAnchorStore
#else
        MockAnchorStore
#endif
    {
    }

    /// <summary>
    /// A store for points in space that the system will restore on second-run. By default, Unity's
    /// PlayerPrefs KV store is used.
    /// </summary>
    public abstract class AbstractAnchorStore<AnchorType>
        : MonoBehaviour
        where AnchorType : Component
    {
        /// <summary>
        /// A prefix to add to names to make IDs unique to AbstractAnchorStore.
        /// </summary>
        private static readonly string ID_PREFIX = typeof(AbstractAnchorStore<AnchorType>).FullName + "::";

        /// <summary>
        /// Makes an identifier for storing values in that are unique to AbstractAnchorStore so that
        /// AbstractAnchorStore can delete them later, when the user requests all anchors be deleted.
        /// </summary>
        /// <param name="name">
        /// A controllable part of the name, to be modified to be unique in the data store.
        /// </param>
        /// <returns>A name in the data store unique to AbstractAnchorStore.</returns>
        protected static string MakeID(string name)
        {
            return ID_PREFIX + name;
        }

        /// <summary>
        /// The place where the names of all of the saved values are stored.
        /// </summary>
        private const string KEY_LIST_KEY = "KEY_LIST";

        /// <summary>
        /// Check to see if a given name exists in the data store.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        protected static bool HasValue(string name)
        {
            return PlayerPrefs.HasKey(MakeID(name));
        }

        /// <summary>
        /// Saves a value to the data store.
        /// </summary>
        /// <param name="name"> The name of the object to save.</param>
        /// <param name="value">The value to save.</param>
        protected static void SaveValue<T>(string name, T value)
        {
            var json = new JsonFactory<T>();
            PlayerPrefs.SetString(MakeID(name), json.ToString(value));

            var names = NameList;
            if (!names.Contains(name))
            {
                var n = names.ToList();
                n.Add(name);
                NameList = n.ToArray();
            }

            PlayerPrefs.Save();
        }

        /// <summary>
        /// Gets a value back out of data store.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        protected static T LoadValue<T>(string name)
        {
            var key = MakeID(name);
            if (PlayerPrefs.HasKey(key))
            {
                try
                {
                    var json = new JsonFactory<T>();
                    return json.Parse(PlayerPrefs.GetString(key));
                }
                catch
                {
                    DeleteValue(name);
                }
            }

            return default;
        }

        /// <summary>
        /// Removes a value from the data store.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        protected static bool DeleteValue(string name)
        {
            var key = MakeID(name);
            if (PlayerPrefs.HasKey(key))
            {
                PlayerPrefs.DeleteKey(key);
                NameList = NameList.Remove(name);
                PlayerPrefs.Save();
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// An empty array for returning without creating the empty array all the time.
        /// </summary>
        private static readonly string[] EMPTY_STRING_ARRAY = new string[0];

        /// <summary>
        /// A list of all keys for all objects stored in the data store along with the anchor store.
        /// </summary>
        private static string[] NameList
        {
            get
            {
                if (HasValue(KEY_LIST_KEY))
                {
                    return LoadValue<string[]>(KEY_LIST_KEY);
                }
                else
                {
                    return EMPTY_STRING_ARRAY;
                }
            }

            set
            {
                if (value == null)
                {
                    DeleteValue(KEY_LIST_KEY);
                }
                else
                {
                    SaveValue(KEY_LIST_KEY, value);
                }
            }
        }

        /// <summary>
        /// Delete all saved anchors, if there are any.
        /// </summary>
        public virtual void ResetData()
        {
            foreach (var name in NameList)
            {
                DeleteValue(name);
            }

            NameList = null;
        }

        /// <summary>
        /// Anchors provide a means to restore the position of objects between sessions. In some AR
        /// systems, they also provide a means of prioritizing the stability of the location of
        /// specific objects. This method creates the native anchor type for any given subsystem, and
        /// save it to its native anchor store.
        /// </summary>
        /// <returns>
        /// <c>true</c>, if anchor existed on the object, existed in the data-store, or was
        /// successfully created, <c>false</c> otherwise.
        /// </returns>
        /// <param name="ID">        Identifier.</param>
        /// <param name="gameObject">Game object.</param>
        public bool DropAnchor(string ID, GameObject gameObject)
        {
            if (string.IsNullOrEmpty(ID))
            {
                throw new ArgumentException("Parameter must have a value.", nameof(ID));
            }

            var anchor = FindAnchor(gameObject);
            if (!IsSaved(anchor))
            {
                anchor = SynchronizeAnchor(ID, gameObject);
            }

            return IsGood(anchor);
        }

        /// <summary>
        /// Looks to see if the anchor was already in the data store, then loads it along with the
        /// objects scale if it was, or creates it and saves the scale if it wasn't.
        /// </summary>
        /// <param name="ID">        The name of the object to serialize</param>
        /// <param name="gameObject">The object that is being anchored</param>
        /// <returns>The synchronized anchor</returns>
        protected virtual AnchorType SynchronizeAnchor(string ID, GameObject gameObject)
        {
            var scaleID = ID + "-scale";
            AnchorType anchor;
            if (HasAnchor(ID))
            {
                anchor = LoadAnchor(ID);
                if (HasValue(scaleID))
                {
                    var arr = LoadValue<float[]>(scaleID);
                    gameObject.transform.localScale = new Vector3(arr[0], arr[1], arr[2]);
                }
            }
            else
            {
                anchor = CreateAnchor(ID, gameObject);
                var arr = new float[]
                {
                    gameObject.transform.localScale.x, gameObject.transform.localScale.y, gameObject.transform.localScale.z
                };
                SaveValue(scaleID, arr);
            }

            return anchor;
        }

        /// <summary>
        /// Checks to see if an anchor is valid.
        /// </summary>
        /// <param name="anchor"></param>
        /// <returns></returns>
        protected virtual bool IsGood(AnchorType anchor)
        {
            return anchor != null;
        }

        /// <summary>
        /// Anchors provide a means to restore the position of objects between sessions. In some AR
        /// systems, they also provide a means of prioritizing the stability of the location of
        /// specific objects. This method removes the anchor from the gameObject and delete it out of
        /// their native anchor store.
        /// </summary>
        /// <param name="ID">        Identifier.</param>
        /// <param name="gameObject">Game object.</param>
        public void WeighAnchor(string ID, GameObject gameObject)
        {
            var anchor = FindAnchor(gameObject);
            if (anchor != null)
            {
                DeleteAnchor(ID, gameObject, anchor);
                DeleteValue(ID + "-scale");
            }
        }

        /// <summary>
        /// Removes an anchor along with its scale from the data store.
        /// </summary>
        /// <param name="ID">        The name of the anchor to remove</param>
        /// <param name="gameObject">The object that was anchored</param>
        /// <param name="anchor">    The anchor to destroy</param>
        protected virtual void DeleteAnchor(string ID, GameObject gameObject, AnchorType anchor)
        {
            DeleteValue(ID);
            anchor.DestroyImmediate();
        }

        /// <summary>
        /// Finds any anchor components on the given object
        /// </summary>
        /// <param name="gameObject">The object that might have an anchor on it</param>
        /// <returns></returns>
        protected virtual AnchorType FindAnchor(GameObject gameObject)
        {
            return gameObject?.GetComponent<AnchorType>();
        }

        /// <summary>
        /// Checks to see if an anchor has been saved in the data store.
        /// </summary>
        /// <param name="anchor">The anchor to check against the data store</param>
        /// <returns>True, if the anchor has already been saved, false otherwise</returns>
        protected virtual bool IsSaved(AnchorType anchor)
        {
            return anchor != null;
        }

        /// <summary>
        /// Anchors provide a means to restore the position of objects between sessions. In some AR
        /// systems, they also provide a means of prioritizing the stability of the location of
        /// specific objects. This method checks to see if a given anchor exists in the anchor data store.
        /// </summary>
        /// <returns><c>true</c>, if anchor exists, <c>false</c> otherwise.</returns>
        /// <param name="anchorID">Anchor identifier.</param>
        public abstract bool HasAnchor(string anchorID);

        /// <summary>
        /// Creates a new anchor.
        /// </summary>
        /// <param name="ID">        The name of the anchor to create.</param>
        /// <param name="gameObject">The object to be anchored.</param>
        /// <returns>The new anchor.</returns>
        protected abstract AnchorType CreateAnchor(string ID, GameObject gameObject);

        /// <summary>
        /// Loads an anchor out of a platform-specific anchor data store.
        /// </summary>
        /// <param name="ID">The name of the anchor to load.</param>
        /// <returns>The persisted anchor.</returns>
        protected abstract AnchorType LoadAnchor(string ID);
    }
}