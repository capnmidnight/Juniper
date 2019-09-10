using System.Linq;

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
        private bool ForwardPressed
        {
            get
            {
                return input.PrimaryPointer?.IsButtonPressed(InputButton.Left) == true;
            }
        }

        private bool BackPressed
        {
            get
            {
                return input.PrimaryPointer?.IsButtonPressed(InputButton.Right) == true;
            }
        }

        public void Update()
        {
            if (input.PrimaryPointer != null)
            {
                float thrust = 0;
                if (input.PrimaryPointer.EventTarget == null)
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

                stage.SetVelocity(thrust * MOVEMENT_SCALE * input.PrimaryPointer.Direction);
            }
        }
    }
}
