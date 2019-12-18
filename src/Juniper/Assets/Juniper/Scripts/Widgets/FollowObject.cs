using Juniper.Units;

using UnityEngine;

namespace Juniper.Widgets
{
    /// <summary>
    /// Make one object's pose follow the pose of another object.
    /// </summary>
    public class FollowObject : MonoBehaviour
    {
        public float closeThreshold = 0.01f;

        /// <summary>
        /// The target object. Set <see cref="followMainCamera"/> to false or else it will be
        /// overwritten at runtime.
        /// </summary>
        public Transform followObject;

        public bool interpolate = true;

        [Header("Position")]
        public CartesianAxisFlags FollowPosition = CartesianAxisFlags.XZ;

        /// <summary>
        /// The maximum allowable deviation in distance from the target's position.
        /// </summary>
        public float FollowThreshold = 0;

        public float maxSpeed = 1;

        /// <summary>
        /// The distance to maintain from the target object.
        /// </summary>
        public float Distance = 0;

        /// <summary>
        /// Follow the target's rotation in the X-Axis.
        /// </summary>
        [Header("Rotation")]
        public CartesianAxisFlags FollowRotation;

        /// <summary>
        /// The maximum allowable deviation in angles from the target's rotation..
        /// </summary>
        public Vector3 RotationThreshold = 10 * Vector3.up;

        public float maxRotationRate = 10;

        /// <summary>
        /// Retrieve the main system configuration, find the main camera (if requested), and
        /// initialize the tracking state.
        /// </summary>
        public void Start()
        {
            if (FollowRotation != CartesianAxisFlags.None)
            {
                transform.eulerAngles = GetEndRotationEuler();
            }

            if (FollowPosition != CartesianAxisFlags.None)
            {
                transform.position = GetEndPosition();
            }
        }

        /// <summary>
        /// Change the follow target at runtime.
        /// </summary>
        /// <param name="target">Target.</param>
        public void SetTarget(Transform target)
        {
            followObject = target;
        }

        /// <summary>
        /// Achieve the target following by re-parenting this gameObject to the target object. The
        /// FollowObject component will be removed in the process.
        /// </summary>
        /// <param name="target">Target.</param>
        public void Reparent(Transform target)
        {
            transform.Reparent(target, false);
            enabled = false;
            this.DestroyImmediate();
        }

        public void Update()
        {
            if (interpolate)
            {
                if (FollowRotation != CartesianAxisFlags.None)
                {
                    var endRotation = GetEndRotationEuler();
                    var delta = endRotation - transform.eulerAngles;
                    if (delta.magnitude < closeThreshold)
                    {
                        SkipToRotation();
                    }
                    else
                    {
                        targetRotationRate = delta / Time.deltaTime;

                        var speed = targetRotationRate.magnitude;
                        if (speed > maxRotationRate)
                        {
                            targetRotationRate *= maxRotationRate / speed;
                        }

                        rotationRate = Vector3.Lerp(rotationRate, targetRotationRate, 0.5f);
                    }
                }

                if (FollowPosition != CartesianAxisFlags.None)
                {
                    var endPosition = GetEndPosition();
                    var delta = endPosition - transform.position;
                    if (delta.magnitude < closeThreshold)
                    {
                        SkipToPosition();
                    }
                    else
                    {
                        targetVelocity = delta / Time.deltaTime;

                        var speed = targetVelocity.magnitude;
                        if (speed > maxSpeed)
                        {
                            targetVelocity *= maxSpeed / speed;
                        }

                        velocity = Vector3.Lerp(velocity, targetVelocity, 0.5f);
                    }
                }
            }
            else
            {
                Skip();
            }
        }

        /// <summary>
        /// The object following is performed in the LateUpdate function to make sure that it occurs
        /// after the target's own Update function, where it is most likely to have its pose updated.
        /// </summary>
        public void LateUpdate()
        {
            if (interpolate)
            {
                transform.position = NextPosition(Time.deltaTime);
                transform.rotation = NextRotation(Time.deltaTime);
            }
        }

        private Vector3 NextPosition(float dt)
        {
            return transform.position + velocity * dt;
        }

        private Quaternion NextRotation(float dt)
        {
            return Quaternion.Euler(rotationRate * dt) * transform.rotation;
        }

        public void Skip()
        {
            SkipToPosition();
            SkipToRotation();
        }

        private void SkipToRotation()
        {
            if (FollowRotation != CartesianAxisFlags.None)
            {
                var endEul = GetEndRotationEuler();
                transform.rotation = Quaternion.Euler(endEul);
                targetRotationRate
                    = rotationRate
                    = Vector3.zero;
            }
        }

        private void SkipToPosition()
        {
            if (FollowPosition != CartesianAxisFlags.None)
            {
                var end = GetEndPosition();
                transform.position = end;
                targetVelocity
                    = velocity
                    = Vector3.zero;
            }
        }

        private Vector3 GetEndPosition()
        {
            var end = followObject.position + (Distance * transform.forward);
            var delta = end - transform.position;

            if ((FollowPosition & CartesianAxisFlags.X) == 0)
            {
                delta.x = 0;
            }

            if ((FollowPosition & CartesianAxisFlags.Y) == 0)
            {
                delta.y = 0;
            }

            if ((FollowPosition & CartesianAxisFlags.Z) == 0)
            {
                delta.z = 0;
            }

            if (delta.magnitude < FollowThreshold && velocity.magnitude == 0)
            {
                delta = Vector3.zero;
            }

            return delta + transform.position;
        }

        private Vector3 GetEndRotationEuler()
        {
            var startEul = transform.eulerAngles;
            var endEul = followObject.eulerAngles;
            var deltaEul = endEul - startEul;
            var rotationSpeed = rotationRate.magnitude;

            if ((FollowRotation & CartesianAxisFlags.X) == 0
                || rotationSpeed == 0 
                    && Mathf.Abs(deltaEul.x) < Mathf.Abs(RotationThreshold.x))
            {
                deltaEul.x = 0;
            }

            if ((FollowRotation & CartesianAxisFlags.Y) == 0
                || rotationSpeed == 0
                    && Mathf.Abs(deltaEul.y) < Mathf.Abs(RotationThreshold.y))
            {
                deltaEul.y = 0;
            }

            if ((FollowRotation & CartesianAxisFlags.Z) == 0
                || rotationSpeed == 0
                    && Mathf.Abs(deltaEul.z) < Mathf.Abs(RotationThreshold.z))
            {
                deltaEul.z = 0;
            }

            var angX = new Angle(startEul.x);
            var angY = new Angle(startEul.y);
            var angZ = new Angle(startEul.z);

            angX += deltaEul.x;
            angY += deltaEul.y;
            angZ += deltaEul.z;

            return new Vector3(angX, angY, angZ);
        }

        private Vector3 targetVelocity, velocity;
        private Vector3 targetRotationRate, rotationRate;
    }
}
