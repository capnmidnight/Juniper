using System.Linq;

using Juniper.Unity.Input.Pointers;

using UnityEngine;

using InputButton = UnityEngine.EventSystems.PointerEventData.InputButton;

namespace Juniper.Unity.Input
{
#if UNITY_MODULES_PHYSICS

    [RequireComponent(typeof(Rigidbody))]
#endif
    public class HoverCraft : AbstractVelocityLocomotion
    {
        public IPointerDevice Pointer;

        private bool ForwardPressed
        {
            get
            {
                return Pointer?.IsButtonPressed(InputButton.Left) == true;
            }
        }

        private bool BackPressed
        {
            get
            {
                return Pointer?.IsButtonPressed(InputButton.Right) == true;
            }
        }

        public override void Update()
        {
            if (Pointer == null)
            {
                Pointer = input.Devices.FirstOrDefault(p => p.IsEnabled);
            }
            else if (Pointer.IsDisabled)
            {
                velocity = Vector3.zero;
                Pointer = null;
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

                velocity = thrust * MOVEMENT_SCALE * Pointer.Direction;
            }

            base.Update();
        }
    }
}