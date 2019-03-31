using UnityEngine;

namespace Juniper.Unity.Input.Pointers
{
    public abstract class PointerDataCreator : MonoBehaviour
    {
        private static int COUNTER = 0;

        /// <summary>
        /// Unique pointer identifiers keep the pointer events cached in Unity's Event System.
        /// </summary>
        /// <value>The pointer identifier.</value>
        public int PointerDataID
        {
            get; private set;
        }

        public virtual void Awake()
        {
            PointerDataID = ++COUNTER;
        }
    }
}
