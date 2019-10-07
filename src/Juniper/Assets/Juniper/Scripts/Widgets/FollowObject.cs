using Juniper.Units;

using UnityEngine;

namespace Juniper.Widgets
{
    /// <summary>
    /// Make one object's pose follow the pose of another object.
    /// </summary>
    public class FollowObject : MonoBehaviour
    {
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

        public float MaxDistance = 5;

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

        /// <summary>
        /// The object following is performed in the LateUpdate function to make sure that it occurs
        /// after the target's own Update function, where it is most likely to have its pose updated.
        /// </summary>
        public void LateUpdate()
        {
            if (FollowRotation != CartesianAxisFlags.None)
            {
                var endEul = GetEndRotationEuler();

                if (interpolate)
                {
                    targetRotationRate = Vector3.zero;
                    var deltaEul = endEul - transform.eulerAngles;

                    targetRotationRate = deltaEul / Time.deltaTime;
                    var speed = targetRotationRate.magnitude;
                    if (speed > maxRotationRate)
                    {
                        targetRotationRate *= maxRotationRate / speed;
                    }

                    if (speed < 0.2f)
                    {
                        targetRotationRate = Vector3.zero;
                    }

                    rotationRate = Vector3.Lerp(rotationRate, targetRotationRate, 0.5f);
                    transform.rotation = Quaternion.Euler(rotationRate * Time.deltaTime) * transform.rotation;
                }
                else
                {
                    transform.rotation = Quaternion.Euler(endEul);
                }
            }

            if (FollowPosition != CartesianAxisFlags.None)
            {
                var end = GetEndPosition();
                if (interpolate)
                {
                    targetVelocity = Vector3.zero;
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

                    var distance = delta.magnitude;
                    if (distance > MaxDistance)
                    {
                        velocity = Vector3.zero;
                        transform.position = end;
                    }
                    else
                    {
                        if (distance > FollowThreshold)
                        {
                            targetVelocity = delta / Time.deltaTime;

                            if (interpolate)
                            {
                                var speed = targetVelocity.magnitude;
                                if (speed > maxSpeed)
                                {
                                    targetVelocity *= maxSpeed / speed;
                                }
                                else if (speed < 0.2f)
                                {
                                    targetVelocity = Vector3.zero;
                                }
                            }
                        }

                        velocity = Vector3.Lerp(velocity, targetVelocity, 0.5f);
                        transform.position += velocity * Time.deltaTime;
                    }
                }
                else
                {
                    transform.position = end;
                }
            }
        }

        private Vector3 GetEndPosition()
        {
            return followObject.position + (Distance * transform.forward);
        }

        private Vector3 GetEndRotationEuler()
        {
            var startEul = transform.eulerAngles;
            var endEul = followObject.eulerAngles;
            var deltaEul = endEul - startEul;

            if ((FollowRotation & CartesianAxisFlags.X) == 0 || Mathf.Abs(deltaEul.x) < Mathf.Abs(RotationThreshold.x))
            {
                deltaEul.x = 0;
            }

            if ((FollowRotation & CartesianAxisFlags.Y) == 0 || Mathf.Abs(deltaEul.y) < Mathf.Abs(RotationThreshold.y))
            {
                deltaEul.y = 0;
            }

            if ((FollowRotation & CartesianAxisFlags.Z) == 0 || Mathf.Abs(deltaEul.z) < Mathf.Abs(RotationThreshold.z))
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
