using UnityEngine;

namespace Juniper.Unity.Input
{
    /// <summary>
    /// Controlling the camera from a desktop system. Very useful for quickly debugging things.
    /// </summary>
    [DisallowMultipleComponent]
    public class RunningMovement : AbstractVelocityLocomotion
    {
        public override void Update()
        {
            var horiz = jink * UnityEngine.Input.GetAxisRaw("Horizontal");
            var vert = moveSpeed * UnityEngine.Input.GetAxisRaw("Vertical");
            var moveDirection = (horiz * Vector3.right) + (vert * Vector3.forward);

            // don't allow keyboard movement to change the y-velocity.
            moveDirection.y = 0;

            // provide a max-limit to move speed, but allow analog values for lower speeds.
            if (moveDirection.sqrMagnitude > 1)
            {
                moveDirection.Normalize();
            }

            velocity = moveDirection * MOVEMENT_SCALE;

            base.Update();
        }
    }
}