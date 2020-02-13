using System;

using Juniper.Units;

using UnityEngine;

namespace Juniper.Widgets
{
    /// <summary>
    /// Make one object's pose follow the pose of another object.
    /// </summary>
    public class FollowObject : AbstractFollowSettings
    {
        private const float CLOSE_THRESHOLD = 0.01f;
        private const float MAX_TIME = 1.5f;

        /// <summary>
        /// The target object. Set <see cref="followMainCamera"/> to false or else it will be
        /// overwritten at runtime.
        /// </summary>
        public Transform followObject;

        private Vector3 targetVelocity, velocity;
        private Vector3 targetRotationRate, rotationRate;

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

        private void SetRate(CartesianAxisFlags follow, bool interpolate, Vector3 current, Vector3 end, ref Vector3 actual, ref Vector3 target, float maxRate, Action skip)
        {
            if (follow != CartesianAxisFlags.None)
            {
                if (interpolate)
                {
                    var delta = end - current;
                    var displacement = delta.magnitude;
                    var time = displacement / maxRate;
                    if (displacement < CLOSE_THRESHOLD || time > MAX_TIME)
                    {
                        skip();
                    }
                    else
                    {
                        target = delta / Time.deltaTime;

                        var speed = target.magnitude;
                        if (speed > maxRate)
                        {
                            target *= maxRate / speed;
                        }

                        actual = Vector3.Lerp(actual, target, 0.5f);
                    }
                }
                else
                {
                    skip();
                }
            }
        }

        public void Update()
        {
            SetRate(
                FollowPosition, interpolatePosition,
                transform.position, GetEndPosition(),
                ref velocity, ref targetVelocity,
                maxSpeed,
                SkipToPosition);

            SetRate(
                FollowRotation, interpolateRotation,
                transform.eulerAngles, GetEndRotationEuler(),
                ref rotationRate, ref targetRotationRate,
                maxRotationRate,
                SkipToRotation);
        }

        /// <summary>
        /// The object following is performed in the LateUpdate function to make sure that it occurs
        /// after the target's own Update function, where it is most likely to have its pose updated.
        /// </summary>
        public void LateUpdate()
        {
            if (FollowPosition != CartesianAxisFlags.None
                && interpolatePosition)
            {
                transform.position = NextPosition(Time.deltaTime);
            }

            if (FollowRotation != CartesianAxisFlags.None
                && interpolateRotation)
            {
                transform.rotation = NextRotation(Time.deltaTime);
            }
        }

        public void Skip()
        {
            if (FollowPosition != CartesianAxisFlags.None)
            {
                SkipToPosition();
            }

            if (FollowRotation != CartesianAxisFlags.None)
            {
                SkipToRotation();
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

        private void SkipToRotation()
        {
            var endEul = GetEndRotationEuler();
            transform.rotation = Quaternion.Euler(endEul);
            targetRotationRate
                = rotationRate
                = Vector3.zero;
        }

        private void SkipToPosition()
        {
            var end = GetEndPosition();
            transform.position = end;
            targetVelocity
                = velocity
                = Vector3.zero;
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

            if (delta.magnitude < FollowPositionThreshold && velocity.magnitude == 0)
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
                    && Mathf.Abs(deltaEul.x) < Mathf.Abs(FollowRotationThreshold.x))
            {
                deltaEul.x = 0;
            }

            if ((FollowRotation & CartesianAxisFlags.Y) == 0
                || rotationSpeed == 0
                    && Mathf.Abs(deltaEul.y) < Mathf.Abs(FollowRotationThreshold.y))
            {
                deltaEul.y = 0;
            }

            if ((FollowRotation & CartesianAxisFlags.Z) == 0
                || rotationSpeed == 0
                    && Mathf.Abs(deltaEul.z) < Mathf.Abs(FollowRotationThreshold.z))
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
    }
}
