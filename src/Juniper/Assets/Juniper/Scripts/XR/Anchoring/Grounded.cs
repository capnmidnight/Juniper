using System;
using System.Collections.Generic;

using Juniper.Unity.Widgets;

using UnityEngine;

namespace Juniper.Unity.Anchoring
{
    /// <summary>
    /// This component searches for the ground above or below the gameObject it's attached to. When a
    /// mesh tagged as the "Ground" is found, with horizontal surfaces, the gameObject is moved to
    /// the ground and kept there. Use this when you have objects that should rest of the ground, but
    /// you're not sure where the ground will be.
    /// </summary>
    public class Grounded : MonoBehaviour
    {
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
        /// Where the current gameObject has been ordered to find.
        /// </summary>
        private Vector3? groundPoint;

        /// <summary>
        /// Before the object is actually grounded, other entities may wish to know when it is. This
        /// queue holds callbacks to those entities to execute once the object is grounded.
        /// </summary>
        private Queue<Action> q;

        /// <summary>
        /// The raycasting mask for the ground mesh.
        /// </summary>
        private int GroundMask;

        /// <summary>
        /// The timestamp of when the last ground test was performed.
        /// </summary>
        private float lastTime;

#endif

        /// <summary>
        /// The stage that holds the user is really the thing that needs to be grounded.
        /// </summary>
        private StageExtensions stage;

        /// <summary>
        /// How many seconds to wait in between ground tests, to avoid pegging the system with ground tests.
        /// </summary>
        public float testTimeout = 0.25f;

        /// <summary>
        /// The minimum amount of horizontalness to consider for the "ground".
        /// </summary>
        public float horizontalnessThreshold = 0.85f;

#if UNITY_MODULES_PHYSICS

        /// <summary>
        /// Get the physics components of the gameObject, find information about the ground mesh, and
        /// freeze the object in place until the ground is found.
        /// </summary>
        public void Awake()
        {
            stage = ComponentExt.FindAny<StageExtensions>();
            body = GetComponent<Rigidbody>();
            dragger = GetComponent<Draggable>();
            GroundMask = LayerMask.GetMask("Ground");
            lastTime = Time.time;
            Freeze();
        }

        /// <summary>
        /// Freezing the object turns off any physics and dragging capability that might have been on
        /// the object.
        /// </summary>
        private void Freeze()
        {
            if (body != null)
            {
                if (dragger != null)
                {
                    dragger.enabled = false;
                }

                wasUsingGravity = body.useGravity;
                body.useGravity = false;
            }
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
                new Ray(center, -transform.up),
                new Ray(center, transform.up)
            };

            foreach (var ray in rays)
            {
                RaycastHit target;
                if (Physics.Raycast(ray, out target, 10, GroundMask))
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

        /// <summary>
        /// Restore physics and draggable capabilities.
        /// </summary>
        private void Unfreeze()
        {
            if (body)
            {
                body.useGravity = wasUsingGravity;

                if (dragger)
                {
                    dragger.enabled = true;
                }
            }
        }

        /// <summary>
        /// Execute any <see cref="WhenGrounded(Action)"/> callbacks that were registered before the
        /// object was grounded.
        /// </summary>
        private void EmptyJobQueue()
        {
            if (q != null)
            {
                while (q.Count > 0)
                {
                    var act = q.Dequeue();
                    act();
                }

                q = null;
            }
        }

#if UNITY_EDITOR

        /// <summary>
        /// Delete the saved ground point, starting the work of looking for the ground again.
        /// </summary>
        public void Reset()
        {
            groundPoint = null;
        }

#endif

#endif

        /// <summary>
        /// If the test timeout has passed, raycast for the ground mesh under and over the object. If
        /// it's found, move the object to that point and unfreeze it.
        /// </summary>
        public void Update()
        {
#if UNITY_MODULES_PHYSICS
            if (groundPoint == null && Time.time - lastTime > testTimeout)
            {
                lastTime = Time.time;
                groundPoint = GetGroundUnderObject();
                if (groundPoint == null)
                {
                    Freeze();
                }
                else
                {
                    transform.position = groundPoint.Value + stage.defaultAvatarHeight * Vector3.up;

                    Unfreeze();
                    EmptyJobQueue();
                }
            }
#endif
        }

        /// <summary>
        /// Check to see if the object is grounded, and call the callback if it is. If it's not, put
        /// the callback into a queue for execution later in <see cref="EmptyJobQueue"/>.
        /// </summary>
        /// <param name="p">P.</param>
        public void WhenGrounded(Action p)
        {
#if UNITY_MODULES_PHYSICS
            if (groundPoint != null)
            {
                p();
            }
            else
            {
                if (q == null)
                {
                    q = new Queue<Action>();
                }
                q.Enqueue(p);
            }
#endif
        }
    }
}