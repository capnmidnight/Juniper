using Newtonsoft.Json;
using System;
using System.Linq;

using UnityEngine;

namespace Juniper.Anchoring
{
    /// <summary>
    /// A store for points in space that the system will restore on second-run.
    /// </summary>
    public abstract class AbstractAnchorStore<AnchorType>
        : MonoBehaviour
        where AnchorType : MonoBehaviour
    {
        /// <summary>
        /// A prefix to add to names to make IDs unique to AbstractAnchorStore.
        /// </summary>
        private static readonly string ID_PREFIX = typeof(AbstractAnchorStore<AnchorType>).FullName + "::";

        /// <summary>
        /// Makes an identifier for storing values in that are unique to AbstractAnchorStore
        /// so that AbstractAnchorStore can delete them later, when the user requests all anchors
        /// be deleted.
        /// </summary>
        /// <param name="name">A controllable part of the name, to be modified to be unique in PlayerPrefs.</param>
        /// <returns>A name in PlayerPrefs unique to AbstractAnchorStore.</returns>
        protected static string MakeID(string name)
        {
            return ID_PREFIX + name;
        }

        /// <summary>
        /// The place where the names of all of the saved values are stored.
        /// </summary>
        private static readonly string KEY_LIST_KEY = MakeID("KEY_LIST");

        /// <summary>
        /// An empty array for returning without creating the empty array all the time.
        /// </summary>
        private static readonly string[] EMPTY_STRING_ARRAY = new string[0];

        /// <summary>
        /// A list of all keys for all objects stored in PlayerPrefs along with the anchor store.
        /// </summary>
        private static string[] NameList
        {
            get
            {
                if (PlayerPrefs.HasKey(KEY_LIST_KEY))
                {

                    return JsonConvert.DeserializeObject<string[]>(PlayerPrefs.GetString(KEY_LIST_KEY));
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
                    PlayerPrefs.DeleteKey(KEY_LIST_KEY);
                }
                else
                {
                    PlayerPrefs.SetString(KEY_LIST_KEY, JsonConvert.SerializeObject(value));
                }

                PlayerPrefs.Save();
            }
        }

        protected static bool HasValue(string name)
        {
            return PlayerPrefs.HasKey(MakeID(name));
        }

        /// <summary>
        /// Saves a value to PlayerPrefs.
        /// </summary>
        /// <param name="name">The name of the object to save.</param>
        /// <param name="value">The value to save.</param>
        protected static void SaveValue(string name, object value)
        {
            PlayerPrefs.SetString(MakeID(name), JsonConvert.SerializeObject(value));

            var names = NameList;
            if (names.Contains(name))
            {
                PlayerPrefs.Save();
            }
            else
            {
                NameList = names.Append(name).ToArray();
            }
        }

        /// <summary>
        /// Gets a value back out of PlayerPrefs.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        protected static T LoadValue<T>(string name)
        {
            var key = MakeID(name);
            if (PlayerPrefs.HasKey(key))
            {
                return JsonConvert.DeserializeObject<T>(PlayerPrefs.GetString(key));
            }
            else
            {
                return default(T);
            }
        }

        /// <summary>
        /// Removes a value from the PlayerPrefs.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        protected static bool DeleteValue(string name)
        {
            var key = MakeID(name);
            if (PlayerPrefs.HasKey(key))
            {
                PlayerPrefs.DeleteKey(key);
                NameList = NameList.Where(k => k != name).ToArray();
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Delete saved anchors, if there are any.
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

            var anchor = FindAnchor(gameObject);
            if (!IsSaved(anchor))
            {
                anchor = SaveAnchor(ID, gameObject);
            }

            return IsGood(anchor);
        }

        protected virtual AnchorType SaveAnchor(string ID, GameObject gameObject)
        {
            var scaleID = ID + "-scale";
            AnchorType anchor;
            if (HasAnchor(ID))
            {
                anchor = LoadAnchor(ID);
                if (HasValue(scaleID))
                {
                    gameObject.transform.localScale = LoadValue<Vector3>(scaleID);
                }
            }
            else
            {
                anchor = CreateAnchor(ID, gameObject);
                SaveValue(scaleID, gameObject.transform.localScale);
            }

            return anchor;
        }

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
        /// <param name="ID">Identifier.</param>
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

        protected virtual void DeleteAnchor(string ID, GameObject gameObject, AnchorType anchor)
        {
            DeleteValue(ID);
            anchor.Destroy();
        }

        protected virtual AnchorType FindAnchor(GameObject gameObject)
        {
            return gameObject?.GetComponent<AnchorType>();
        }

        protected virtual bool IsSaved(AnchorType anchor)
        {
            return anchor != null;
        }

        /// <summary>
        /// Anchors provide a means to restore the position of objects between sessions. In some AR
        /// systems, they also provide a means of prioritizing the stability of the location of
        /// specific objects. This method checks to see if a given anchor exists in the anchor data
        /// store. By default, Unity's PlayerPrefs KV store is used.
        /// </summary>
        /// <returns><c>true</c>, if anchor exists, <c>false</c> otherwise.</returns>
        /// <param name="anchorID">Anchor identifier.</param>
        public abstract bool HasAnchor(string anchorID);

        protected abstract AnchorType CreateAnchor(string ID, GameObject gameObject);

        protected abstract AnchorType LoadAnchor(string ID);
    }

}
