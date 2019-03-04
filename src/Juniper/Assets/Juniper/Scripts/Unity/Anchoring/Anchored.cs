using System;
using System.Collections.Generic;

using UnityEngine;

namespace Juniper.Anchoring
{
    /// <summary>
    /// Creates a fixed point of reference in space. Some AR subsystems are able to prioritize the
    /// tracking of these points to make sure they always appear to be in the same spot in the real world.
    /// </summary>
    public class Anchored : MonoBehaviour
    {
        /// <summary>
        /// The name of the anchor, for storing in the anchor store.
        /// </summary>
        public string anchorID;

        /// <summary>
        /// Returns true when the anchor store is valid and contains the <see cref="anchorID"/>.
        /// </summary>
        /// <value><c>true</c> if has anchor; otherwise, <c>false</c>.</value>
        public bool HasAnchor
        {
            get
            {
                return nkar != null && nkar.HasAnchor(anchorID);
            }
        }

        /// <summary>
        /// Record the starting position, and construct an <see cref="anchorID"/> from the full path
        /// of the gameObject, if an anchorID was not specified.
        /// </summary>
        public void Awake()
        {
            startPosition = transform.localPosition;
            startScale = transform.localScale;
            startRotation = transform.localRotation;

            if (string.IsNullOrEmpty(anchorID))
            {
                anchorID = gameObject.FullName();
            }
        }

        /// <summary>
        /// If an anchor was requested, but the object has not yet been anchored, requests the anchor
        /// store to create a new anchor. If the anchor exists, and removing the anchor has been
        /// requested, requests the anchor store delete the anchor. /// If the anchored state goes
        /// from unanchored to anchored, empties any actions out of the action queue and clears out
        /// the queue.
        /// </summary>
        public void Update()
        {
            if (!isAnchored && shouldAnchor)
            {
                DropAnchor();
            }
            else if (isAnchored && !shouldAnchor)
            {
                WeighAnchor();
            }

            if (isAnchored && !wasAnchored && q != null)
            {
                while (q.Count > 0)
                {
                    var act = q.Dequeue();
                    act();
                }

                q = null;
            }

            wasAnchored = isAnchored;
        }

        /// <summary>
        /// Provide a callback that will be called if the object is already anchored, or put into a
        /// queue to be executed at a later time when the object does become anchored.
        /// </summary>
        /// <returns><c>true</c>, if anchored was whened, <c>false</c> otherwise.</returns>
        /// <param name="act">Act.</param>
        public bool WhenAnchored(Action act)
        {
            if (isAnchored)
            {
                act();
                return true;
            }
            else
            {
                if (q == null)
                {
                    q = new Queue<Action>();
                }

                q.Enqueue(act);
                return false;
            }
        }

        /// <summary>
        /// Reset the object back to its origin state.
        /// </summary>
        public void ResetAnchor()
        {
            if (isActiveAndEnabled)
            {
                WeighAnchor();
                transform.localPosition = startPosition;
                transform.localScale = startScale != Vector3.zero ? startScale : Vector3.one;
                transform.localRotation = startRotation;
            }
        }

        /// <summary>
        /// Request the anchor be removed.
        /// </summary>
        public void WeighAnchor()
        {
            shouldAnchor = false;
            if (nkar != null)
            {
                nkar.WeighAnchor(anchorID, gameObject);
                isAnchored = false;
            }
        }

        /// <summary>
        /// Request an anchor be created.
        /// </summary>
        public void DropAnchor()
        {
            shouldAnchor = true;
            if (nkar != null)
            {
                isAnchored = nkar.DropAnchor(anchorID, gameObject);
            }
        }

        /// <summary>
        /// The position the object had when it first came alive.
        /// </summary>
        private Vector3 startPosition;

        /// <summary>
        /// The scale the object had when it frist came alive.
        /// </summary>
        private Vector3 startScale;

        /// <summary>
        /// The rotation the object had when it first came alive.
        /// </summary>
        private Quaternion startRotation;

        /// <summary>
        /// Set to true when <see cref="DropAnchor"/> is called, and false when <see
        /// cref="WeighAnchor"/> is called, indicating that the object is awaiting a valid anchor
        /// store and to be anchored in space or removed.
        /// </summary>
        private bool shouldAnchor = true;

        /// <summary>
        /// Set to true when the requested anchor has successfully been stored in an anchor store.
        /// </summary>
        private bool isAnchored;

        /// <summary>
        /// Records the value of <see cref="isAnchored"/> from the previous frame, to facilitate
        /// checking for changes.
        /// </summary>
        private bool wasAnchored;

        /// <summary>
        /// A queue of actions to perform once the object is anchored.
        /// </summary>
        private Queue<Action> q;

        /// <summary>
        /// The anchor store.
        /// </summary>
        private AnchorStore nkar;

        /// <summary>
        /// Finds the anchor store.
        /// </summary>
        public void Start()
        {
            nkar = ComponentExt.FindAny<AnchorStore>();
        }
    }
}
