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
        private const float MAX_TIME = 5f;

        /// <summary>
        /// The target object. Set <see cref="followMainCamera"/> to false or else it will be
        /// overwritten at runtime.
        /// </summary>
        public Transform followObject;

        private Vector3 targetVelocity, velocity;

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
            if (FollowPosition != CartesianAxisFlags.None)
            {
                if (interpolatePosition)
                {
                    var delta = EndPosition - transform.position;
                    var displacement = delta.magnitude;
                    var direction = delta.normalized;

                    targetVelocity = speedConstant * direction
                        + speedProportion * displacement * direction;

                    var speed = targetVelocity.magnitude;
                    var time = 2 * displacement / Mathf.Sqrt(speed);

                    if (displacement < CLOSE_THRESHOLD || time > MAX_TIME)
                    {
                        SkipToPosition();
                    }
                    else
                    {
                        velocity = targetVelocity; // Vector3.Lerp(velocity, targetVelocity, 0.5f);
                    }
                }
                else
                {
                    SkipToPosition();
                }
            }
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

            if (FollowRotation != CartesianAxisFlags.None)
            {
                transform.rotation = EndRotation;
            }
        }

        public void Skip()
        {
            if (FollowPosition != CartesianAxisFlags.None)
            {
                SkipToPosition();
            }
        }

        private Vector3 NextPosition(float dt)
        {
            return transform.position + velocity * dt;
        }

        private void SkipToPosition()
        {
            targetVelocity
                = velocity
                = Vector3.zero;

            transform.position = EndPosition;

            if (FollowRotation != CartesianAxisFlags.None)
            {
                transform.rotation = EndRotation;
            }
        }

        private Vector3 EndPosition
        {
            get
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
        }

        private Quaternion EndRotation
        {
            get
            {
                var startEul = transform.eulerAngles;
                var endEul = followObject.eulerAngles;
                var deltaEul = endEul - startEul;

                if ((FollowRotation & CartesianAxisFlags.X) == 0)
                {
                    deltaEul.x = 0;
                }

                if ((FollowRotation & CartesianAxisFlags.Y) == 0)
                {
                    deltaEul.y = 0;
                }

                if ((FollowRotation & CartesianAxisFlags.Z) == 0)
                {
                    deltaEul.z = 0;
                }

                var angX = new Angle(startEul.x);
                var angY = new Angle(startEul.y);
                var angZ = new Angle(startEul.z);

                angX += deltaEul.x;
                angY += deltaEul.y;
                angZ += deltaEul.z;

                return Quaternion.Euler(angX, angY, angZ);
            }
        }
    }
}
