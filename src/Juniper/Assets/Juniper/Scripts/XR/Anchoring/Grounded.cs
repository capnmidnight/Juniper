using System;

using Juniper.Widgets;

using UnityEngine;

namespace Juniper.Anchoring
{
    /// <summary>
    /// This component searches for the ground above or below the gameObject it's attached to. When a
    /// mesh tagged as the "Terrain" is found, with horizontal surfaces, the gameObject is moved to
    /// the ground and kept there. Use this when you have objects that should rest of the ground, but
    /// you're not sure where the ground will be.
    /// </summary>
    public class Grounded : MonoBehaviour
    {
        /// <summary>
        /// Where the current gameObject has been ordered to find.
        /// </summary>
        private Vector3? groundPoint;

#if UNITY_MODULES_PHYSICS

        /// <summary>
        /// If the object being grounded is also controlled by a draggable constraint handle, then we
        /// will need to check if the draggable is being held and disengage any attempts to ground
        /// the gameObject.
        /// </summary>
        private Draggable dragger;

        /// <summary>
        /// The physics body for this object, used to figure out the affect of gravity on the object.
        /// </summary>
        private Rigidbody body;

        /// <summary>
        /// The saved "useGravity" value from the <see cref="body"/> object, so that it can be
        /// restored to its original value after the draggable handle has been removed.
        /// </summary>
        private bool wasUsingGravity;

        /// <summary>
        /// The saved "isKinematic" value from the <see cref="body"/> object, so that it can be
        /// restored to its original value after the draggable handle has been removed.
        /// </summary>
        private bool wasKinematic;

        /// <summary>
        /// The raycasting mask for the ground mesh.
        /// </summary>
        private int GroundMask;

        /// <summary>
        /// The time stamp of when the last ground test was performed.
        /// </summary>
        private float lastTime;

        /// <summary>
        /// A flag indicating the grounded object is already frozen in place.
        /// </summary>
        private bool frozen = false;

        /// <summary>
        /// Get the physics components of the gameObject, find information about the ground mesh, and
        /// freeze the object in place until the ground is found.
        /// </summary>
        public void Awake()
        {
            body = GetComponentInChildren<Rigidbody>();
            dragger = GetComponent<Draggable>();
            GroundMask = LayerMask.GetMask("Terrain");
            lastTime = Time.unscaledTime;
            Freeze();
        }

        /// <summary>
        /// Freezing the object turns off any physics and dragging capability that might have been on
        /// the object.
        /// </summary>
        private void Freeze()
        {
            if (body != null && !frozen)
            {
                frozen = true;
                if (dragger != null)
                {
                    dragger.enabled = false;
                }

                wasUsingGravity = body.useGravity;
                wasKinematic = body.isKinematic;
                body.useGravity = false;
                body.isKinematic = true;
            }
        }

        /// <summary>
        /// Restore physics and draggable capabilities.
        /// </summary>
        private void Unfreeze()
        {
            if (body != null && frozen)
            {
                frozen = false;
                body.useGravity = wasUsingGravity;
                body.isKinematic = wasKinematic;
                body.velocity = Vector3.zero;

                if (dragger)
                {
                    dragger.enabled = true;
                }
            }
        }

        /// <summary>
        /// If the test timeout has passed, raycast for the ground mesh under and over the object. If
        /// it's found, move the object to that point and unfreeze it.
        /// </summary>
        public void Update()
        {
            if (Time.unscaledTime - lastTime > testTimeout)
            {
                lastTime = Time.unscaledTime;
                groundPoint = GetGroundUnderObject();
                if (groundPoint == null)
                {
                    Freeze();
                }
                else
                {
                    transform.position = groundPoint.Value;
                    Unfreeze();
                    onGroundFound?.Invoke();
                }
            }

            isFrozen = frozen;
            grounded = groundPoint != null;
        }

        /// <summary>
        /// Search above and below the object to find the ground mesh.
        /// </summary>
        /// <returns>The ground under object.</returns>
        private Vector3? GetGroundUnderObject()
        {
            var center = transform.position;
            var rays = new[]
            {
                new Ray(center + 0.1f * Vector3.up, -transform.up),
                new Ray(center + 0.1f * Vector3.down, transform.up)
            };

            foreach (var ray in rays)
            {
                if (Physics.Raycast(ray, out var target, 10, GroundMask))
                {
                    var test = Vector3.Dot(Vector3.up, target.normal);
                    var isHorizontal = test >= horizontalnessThreshold;
                    if (isHorizontal)
                    {
                        return target.point;
                    }
                }
            }
            return null;
        }

#endif

        /// <summary>
        /// How many seconds to wait in between ground tests, to avoid pegging the system with ground tests.
        /// </summary>
        public float testTimeout = 0.25f;

        /// <summary>
        /// The minimum amount of horizontal-ness to consider for the "ground".
        /// </summary>
        public float horizontalnessThreshold = 0.85f;

        /// <summary>
        /// Display in the editor whether or not a ground point is currently available.
        /// </summary>
        [ReadOnly]
        public bool grounded = false;

        /// <summary>
        /// Display in the editor whether or not the object is frozen.
        /// </summary>
        [ReadOnly]
        public bool isFrozen = false;

        private event Action onGroundFound;

        public event Action GroundFound
        {
            add
            {
                onGroundFound += value;
                if (groundPoint != null)
                {
                    value.Invoke();
                }
            }

            remove
            {
                onGroundFound -= value;
            }
        }
    }
}