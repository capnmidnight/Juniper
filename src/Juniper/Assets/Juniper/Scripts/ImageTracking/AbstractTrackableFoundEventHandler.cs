using System;
using System.Linq;

using Juniper.Display;

using UnityEngine;
using UnityEngine.Events;

namespace Juniper.ImageTracking
{
    public abstract class AbstractTrackableFoundEventHandler : MonoBehaviour
    {
        /// <summary>
        /// This value is set to true when the target has been found.
        /// </summary>
        public bool Found;

        /// <summary>
        /// Whether or not <see cref="Found"/> was set on the previous frame, to make it easier to
        /// detect changes to it.
        /// </summary>
        private bool? wasFound;

        /// <summary>
        /// This value is set to true when the user is at a safe distance from the target.
        /// </summary>
        public bool Safe = true;

        /// <summary>
        /// Whether or not <see cref="Safe"/> was set on the previou frame.
        /// </summary>
        private bool wasSafe = true;

        /// <summary>
        /// This value is set to true when the target has gone from explicit, direct tracking to
        /// extended tracking.
        /// </summary>
        public bool Extended;

        /// <summary>
        /// A pose for where 3D objects that represent the target should be attached. The attachment
        /// point resets the object's location, orientation, and scale.
        /// </summary>
        public Transform attachmentPoint;

        /// <summary>
        /// The minimum distance in meters from the target at which to consider the user "safe".
        /// </summary>
        public float safeDistance;

        /// <summary>
        /// A Unity Event that fires when the tracking state of the target changes. If you are wiring
        /// up events in the Unity Editor, you should prefer this version. If you are adding event
        /// listeners programmatically, you should prefer <see cref="TrackingStateChange"/>.
        /// </summary>
        public BooleanEvent onTrackingStateChange = new BooleanEvent();

        /// <summary>
        /// A .NET Event that fires when the tracking state of the target changes. If you are adding
        /// event listeners programmatically, you should prefer this version. If you are wiring up
        /// events in the Unity Editor, you should prefer <see cref="onTrackingStateChange"/>.
        /// </summary>
        public event EventHandler<TrackingStateChangeEventArgs> TrackingStateChange;

        /// <summary>
        /// A Unity Event that fires when the target is first found.
        /// </summary>
        public UnityEvent onFound = new UnityEvent();

        /// <summary>
        /// A Unity Event that fires when the target is been completely lost.
        /// </summary>
        public UnityEvent onLost = new UnityEvent();

        /// <summary>
        /// A Unity Event that fires when the user gets too close to the target.
        /// </summary>
        public UnityEvent onProximity = new UnityEvent();

        /// <summary>
        /// A Unity Event that fires when the user returns to a safe distance from the target.
        /// </summary>
        public UnityEvent onSafe = new UnityEvent();

        /// <summary>
        /// The tag that the attachment point is expected to have.
        /// </summary>
        private const string TAG_NAME = "Tracked Object Attachment Point";

        /// <summary>
        /// The name that the attachment point is expected to have.
        /// </summary>
        private const string TRANSFORM_NAME = "Attachment Point";

        /// <summary>
        /// Finds the attachment point for augmentations on the trackable object. An attachment point
        /// is a definition of a space, it sets a default origin, rotation, and scale for objects
        /// attached to it.
        /// </summary>
        public void OnValidate()
        {
            if (attachmentPoint == null)
            {
                attachmentPoint = transform.Children().FirstOrDefault(t =>
                    t.CompareTag(TAG_NAME))
                        ?? transform.Children().FirstOrDefault(t =>
                            t.name == TRANSFORM_NAME)
                        ?? transform;
            }
        }

        /// <summary>
        /// When the trackable is found, we assume it's at a safe space, and then wait for the safety
        /// checker to kick in.
        /// </summary>
        public void OnEnable()
        {
            wasSafe = Safe = true;
        }

        /// <summary>
        /// Checks the tracking status of the target and fires any appropriate change/found/lost events.
        /// </summary>
        public virtual void Update()
        {
            OnFoundChanged();
            SafetyCheck();
        }

        /// <summary>
        /// Check to see if the user is standing at a comfortable distance from the object.
        /// </summary>
        private void SafetyCheck()
        {
            if (Found)
            {
                var camT = DisplayManager.MainCamera.transform;
                var delta = transform.position - camT.position;
                // we only care about the over-ground distance.
                delta.y = 0;

                //Add 10% to the safe distance to return to safety so we don't jitter
                // over the edge.
                Safe = Safe && delta.magnitude >= safeDistance
                    || !Safe && delta.magnitude >= safeDistance * 1.1f;

                OnSafeOrProximity();
            }
        }

        /// <summary>
        /// If the value of <see cref="Safe"/> has changed, fires the <see cref="onSafe"/> or <see
        /// cref="onProximity"/> event.
        /// </summary>
        private void OnSafeOrProximity()
        {
            if (Safe != wasSafe)
            {
                if (Safe)
                {
                    onSafe?.Invoke();
                }
                else
                {
                    onProximity?.Invoke();
                }

                wasSafe = Safe;
            }
        }

        /// <summary>
        /// If the value of <see cref="Found"/> has changed, records the time at which the object was
        /// found, sets the visibility of the object, and fires the <see cref="onFound"/>, <see
        /// cref="onLost"/>, <see cref="onTrackingStateChange"/>, or the <see
        /// cref="TrackingStateChange"/> events.
        /// </summary>
        private void OnFoundChanged()
        {
            if (Found != wasFound)
            {
                this.SetVisible(Found);
                OnTrackingStateChanged();
                OnLostOrFound();
                wasFound = Found;
            }
        }

        /// <summary>
        /// Fires the <see cref="onLost"/> or <see cref="onFound"/> events.
        /// </summary>
        private void OnLostOrFound()
        {
            if (wasFound != null)
            {
                if (Found)
                {
                    onFound?.Invoke();
                }
                else
                {
                    onLost?.Invoke();
                }
            }
        }

        /// <summary>
        /// Firest the <see cref="onTrackingStateChange"/> and <see cref="TrackingStateChange"/> events.
        /// </summary>
        private void OnTrackingStateChanged()
        {
            onTrackingStateChange?.Invoke(Found);
            TrackingStateChange?.Invoke(this, new TrackingStateChangeEventArgs(this, Found));
        }

        /// <summary>
        /// Forces the onFound event to fire.
        /// </summary>
        [ContextMenu("Force 'Found' event")]
        public void ForceFound()
        {
            wasFound = false;
            Found = true;
            Extended = false;
        }

        /// <summary>
        /// Forces the onLost event to fire.
        /// </summary>
        [ContextMenu("Force 'Lost' event")]
        public void ForceLost()
        {
            wasFound = true;
            Found = false;
            Extended = false;
        }

        /// <summary>
        /// When this component is disabled, also disable the tracking of the target and fire the
        /// change event handlers.
        /// </summary>
        public void OnDisable()
        {
            ForceLost();
            Update();
        }

        /// <summary>
        /// Registers for Vuforia's trackable event handlers.
        /// </summary>
        public virtual void Awake()
        {
            if (ComponentExt.FindAny(out TrackerKeeper keeper))
            {
                keeper.Register(this);
            }
        }
    }
}
