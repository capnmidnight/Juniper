using Juniper.Input.Pointers;

using UnityEngine;

using InputButton = UnityEngine.EventSystems.PointerEventData.InputButton;

namespace Juniper.Input
{
#if UNITY_MODULES_PHYSICS
    [RequireComponent(typeof(Rigidbody))]
#endif
    public class HoverCraft : AbstractVelocityLocomotion
    {
        public IPointerDevice Pointer;

        /// <summary>
        /// Retrieves configuration values for the default height of the avatar.
        /// </summary>
        public override void Awake()
        {
            base.Awake();

            if (Pointer == null)
            {
                enabled = false;
                var mgr = ComponentExt.FindAny<UnifiedInputModule>();
                mgr.WithPointer((pointer) =>
                {
                    Pointer = pointer;
                    enabled = true;
                });
            }
        }

        private bool ForwardPressed =>
            Pointer?.IsButtonPressed(InputButton.Left) == true;

        private bool BackPressed =>
            Pointer?.IsButtonPressed(InputButton.Right) == true;

        public override void Update()
        {
            if (Pointer.IsDisabled)
            {
                velocity = Vector3.zero;
            }
            else
            {
                float thrust = 0;
                if (Pointer.EventTarget == null)
                {
                    if (ForwardPressed)
                    {
                        thrust = moveSpeed;
                    }
                    else if (BackPressed)
                    {
                        thrust = -moveSpeed;
                    }
                }

                velocity = thrust * MOVEMENT_SCALE * Pointer.transform.forward;
            }

            base.Update();
        }
    }
}
