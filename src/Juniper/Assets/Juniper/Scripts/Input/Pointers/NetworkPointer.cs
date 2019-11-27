using System;

using Juniper.Haptics;

using UnityEngine;

namespace Juniper.Input.Pointers
{
    public class NetworkPointer 
        : AbstractPointerDevice<Unary, UnaryPointerConfiguration>
    {
        private static readonly TimeSpan CONNECTION_TIMEOUT = TimeSpan.FromSeconds(5);

        private DateTime lastUpdate = DateTime.MinValue;
        private Ray ray;
        
        public Ray Ray
        {
            get
            {
                return ray;
            }
            set
            {
                ray = value;
                lastUpdate = DateTime.Now;
            }
        }

        public DateTime lastClickTime;

        private bool pressed, wasPressed;

        public override bool IsConnected { get { return (DateTime.Now - lastUpdate) < CONNECTION_TIMEOUT; } }

        public override bool IsButtonPressed(Unary button)
        {
            return pressed;
        }

        public override bool IsButtonDown(Unary button)
        {
            return pressed && !wasPressed;
        }

        public override bool IsButtonUp(Unary button)
        {
            return !pressed && wasPressed;
        }

        protected override void InternalUpdate()
        {
            var delta = DateTime.UtcNow - lastClickTime;
            wasPressed = pressed;

            pressed = delta.TotalMilliseconds <= 0.25;
            transform.position = ray.origin;
            transform.rotation = Quaternion.LookRotation(ray.direction);
        }

        protected override AbstractHapticDevice MakeHapticsDevice()
        {
            return this.Ensure<NoHaptics>();
        }

        public override Vector3 WorldPoint
        {
            get
            {
                return transform.position 
                    + transform.forward * MinimumPointerDistance;
            }
        }
    }
}
